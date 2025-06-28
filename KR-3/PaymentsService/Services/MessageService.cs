using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using PaymentsService.Data;
using PaymentsService.Models;
using Common.Models;

namespace PaymentsService.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly PaymentsDbContext _dbContext;
        private IAccountService? _accountService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageService> _logger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;
        private readonly string _paymentRequestsQueue = "payment_requests";
        private readonly string _paymentResultsExchange = "payment_results";

        public MessageService(
            PaymentsDbContext dbContext,
            IServiceProvider serviceProvider,
            ILogger<MessageService> logger,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "rabbitmq",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
            };

            try
            {
                _rabbitConnection = factory.CreateConnection();
                _channel = _rabbitConnection.CreateModel();

                _channel.QueueDeclare(
                    queue: _paymentRequestsQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.ExchangeDeclare(
                    exchange: _paymentResultsExchange,
                    type: ExchangeType.Fanout,
                    durable: true);

                _channel.QueueDeclare(
                    queue: "payment_results_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.QueueBind(
                    queue: "payment_results_queue",
                    exchange: _paymentResultsExchange,
                    routingKey: "");

                _logger.LogInformation("RabbitMQ connection established");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                throw;
            }
        }

        public void SetAccountService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task SaveOutboxMessageAsync<T>(T message, string messageType)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                var outboxMessage = new OutboxMessage
                {
                    MessageType = messageType,
                    MessageContent = JsonSerializer.Serialize(message)
                };

                dbContext.OutboxMessages.Add(outboxMessage);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Saved outbox message {MessageId} of type {MessageType}", 
                    outboxMessage.Id, messageType);
            }
        }

        public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.Status == MessageStatus.Pending)
                        .OrderBy(m => m.CreatedAt)
                        .Take(10)
                        .ToListAsync(cancellationToken);

                    if (!messages.Any())
                    {
                        return;
                    }

                    foreach (var message in messages)
                    {
                        try
                        {
                            if (message.MessageType == nameof(PaymentResultMessage))
                            {
                                var paymentResult = JsonSerializer.Deserialize<PaymentResultMessage>(message.MessageContent);
                                if (paymentResult != null)
                                {
                                    await SendPaymentResultAsync(paymentResult);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Unknown message type: {MessageType}", message.MessageType);
                            }
                            message.Status = MessageStatus.Processed;
                            message.ProcessedAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            message.Status = MessageStatus.Failed;
                            message.Error = ex.Message;
                            message.RetryCount++;
                            
                            _logger.LogError(ex, "Failed to process outbox message {MessageId}: {Error}", message.Id, ex.Message);
                            int delayMs = Math.Min(1000 * (int)Math.Pow(2, message.RetryCount), 60000);
                            await Task.Delay(delayMs, cancellationToken);
                        }
                    }
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in outbox message processor");
                await Task.Delay(5000, cancellationToken);
            }
        }

        public async Task SaveInboxMessageAsync<T>(T message, string messageType, Guid messageId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                var exists = await dbContext.InboxMessages
                    .AnyAsync(m => m.MessageId == messageId);

                if (exists)
                {
                    _logger.LogInformation("Message {MessageId} already exists in inbox", messageId);
                    return;
                }

                var inboxMessage = new InboxMessage
                {
                    MessageId = messageId,
                    MessageType = messageType,
                    MessageContent = JsonSerializer.Serialize(message)
                };

                dbContext.InboxMessages.Add(inboxMessage);
                await dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Saved inbox message {MessageId} of type {MessageType}", 
                    inboxMessage.Id, messageType);
            }
        }

        public async Task<bool> HasProcessedMessageAsync(Guid messageId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                return await dbContext.InboxMessages
                    .AnyAsync(m => m.MessageId == messageId && m.Status == MessageStatus.Processed);
            }
        }

        public async Task MarkInboxMessageAsProcessedAsync(Guid messageId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                var message = await dbContext.InboxMessages
                    .FirstOrDefaultAsync(m => m.MessageId == messageId);

                if (message != null)
                {
                    message.Status = MessageStatus.Processed;
                    message.ProcessedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task ProcessInboxMessagesAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                    var messages = await dbContext.InboxMessages
                        .Where(m => m.Status == MessageStatus.Pending)
                        .OrderBy(m => m.CreatedAt)
                        .Take(10)
                        .ToListAsync(cancellationToken);

                    if (!messages.Any())
                    {
                        return;
                    }
                    foreach (var message in messages)
                    {
                        try
                        {
                            if (message.MessageType == nameof(OrderPaymentRequestMessage))
                            {
                                var paymentRequest = JsonSerializer.Deserialize<OrderPaymentRequestMessage>(message.MessageContent);
                                if (paymentRequest != null)
                                {
                                    await HandleOrderPaymentRequestAsync(paymentRequest);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Unknown message type: {MessageType}", message.MessageType);
                            }
                            message.Status = MessageStatus.Processed;
                            message.ProcessedAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            message.Status = MessageStatus.Failed;
                            message.Error = ex.Message;
                            message.RetryCount++;
                            
                            _logger.LogError(ex, "Failed to process inbox message {MessageId}: {Error}", message.Id, ex.Message);
                            int delayMs = Math.Min(1000 * (int)Math.Pow(2, message.RetryCount), 60000);
                            await Task.Delay(delayMs, cancellationToken);
                        }
                    }
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in inbox message processor");
            }
        }

        public async Task HandleOrderPaymentRequestAsync(OrderPaymentRequestMessage message)
        {
            _logger.LogInformation("Processing payment request for order {OrderId}", message.OrderId);

            if (_accountService == null)
            {
                _logger.LogError("AccountService is not initialized. Cannot process payment.");
                throw new InvalidOperationException("AccountService is not initialized");
            }
            var success = await _accountService.ProcessPaymentAsync(
                message.OrderId, 
                message.UserId, 
                message.Amount, 
                message.Description);
            var resultMessage = new PaymentResultMessage
            {
                OrderId = message.OrderId,
                IsSuccessful = success,
                FailureReason = success ? "There was no failure" : "Insufficient funds"
            };
            await SaveOutboxMessageAsync(resultMessage, nameof(PaymentResultMessage));
        }

        public Task SendPaymentResultAsync(PaymentResultMessage message)
        {
            try
            {
                var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: _paymentResultsExchange,
                    routingKey: "",
                    basicProperties: null,
                    body: messageBytes);

                _logger.LogInformation("Published payment result for order {OrderId}, Success: {IsSuccessful}", 
                    message.OrderId, message.IsSuccessful);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment result for order {OrderId}", message.OrderId);
                throw;
            }
        }

        public void StartConsumingPaymentRequests()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);
                try
                {
                    var message = JsonSerializer.Deserialize<OrderPaymentRequestMessage>(messageString);
                    var messageId = Guid.Parse(ea.BasicProperties.MessageId);
                    await SaveInboxMessageAsync(message, nameof(OrderPaymentRequestMessage), messageId);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment request message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: _paymentRequestsQueue,
                autoAck: false,
                consumer: consumer);
            _logger.LogInformation("Started consuming payment requests");
        }

        public void Dispose()
        {
            _channel?.Close();
            _rabbitConnection?.Close();
        }
    }
}