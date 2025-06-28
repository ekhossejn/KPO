using Common.Models;
using PaymentsService.Models;

namespace PaymentsService.Services
{
    public interface IMessageService
    {
        Task SaveOutboxMessageAsync<T>(T message, string messageType);
        Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken);
        Task SaveInboxMessageAsync<T>(T message, string messageType, Guid messageId);
        Task<bool> HasProcessedMessageAsync(Guid messageId);
        Task MarkInboxMessageAsProcessedAsync(Guid messageId);
        Task ProcessInboxMessagesAsync(CancellationToken cancellationToken);

        Task HandleOrderPaymentRequestAsync(OrderPaymentRequestMessage message);
        Task SendPaymentResultAsync(PaymentResultMessage message);
    }
}