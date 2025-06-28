using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OrdersService.Data;
using OrdersService.Models;
using Common.Models;

namespace OrdersService.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly OrdersDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageService> _logger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;
        private readonly string _paymentRequestsQueue = "payment_requests";
        private readonly string _paymentResultsExchange = "payment_results";

        public MessageService(
            OrdersDbContext dbContext,
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

        public async Task SaveOutboxMessageAsync<T>(T message, string messageType)
        {
            var outboxMessage = new OutboxMessage
            {
                MessageType = messageType,
                MessageContent = JsonSerializer.Serialize(message)
            };

            _dbContext.OutboxMessages.Add(outboxMessage);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Saved outbox message {MessageId} of type {MessageType}",
                outboxMessage.Id, messageType);
        }

        public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            try
            {
                var messages = await _dbContext.OutboxMessages
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
                                await SendOrderPaymentRequestAsync(paymentRequest);
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
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in outbox message processor");
            }
        }

        public Task SendOrderPaymentRequestAsync(OrderPaymentRequestMessage message)
        {
            try
            {
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = Guid.NewGuid().ToString();

                var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: _paymentRequestsQueue,
                    basicProperties: properties,
                    body: messageBytes);

                _logger.LogInformation("Published payment request for order {OrderId}", message.OrderId);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment request for order {OrderId}", message.OrderId);
                throw;
            }
        }

        public async Task HandlePaymentResultAsync(PaymentResultMessage message)
        {
            try
            {
                _logger.LogInformation("Processing payment result for order {OrderId}, Success: {IsSuccessful}, FailureReason: {FailureReason}",
                    message.OrderId, message.IsSuccessful, message.FailureReason);

                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                var status = message.IsSuccessful ? OrderStatus.Finished : OrderStatus.Cancelled;
                await orderService.UpdateOrderStatusAsync(message.OrderId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment result for order {OrderId}", message.OrderId);
                throw;
            }
        }

        public Task StartConsumingPaymentResultsAsync()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);

                try
                {
                    var message = JsonSerializer.Deserialize<PaymentResultMessage>(messageString);

                    if (message != null)
                    {
                        await HandlePaymentResultAsync(message);
                    }
                    else
                    {
                        _logger.LogWarning("Received null PaymentResultMessage after deserialization: {MessageString}", messageString);
                    }
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment result message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: "payment_results_queue",
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("Started consuming payment results");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _rabbitConnection?.Close();
        }
    }
}
