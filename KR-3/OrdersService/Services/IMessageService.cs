using Common.Models;

namespace OrdersService.Services
{
    public interface IMessageService
    {
        Task SaveOutboxMessageAsync<T>(T message, string messageType);
        Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken);
        Task SendOrderPaymentRequestAsync(OrderPaymentRequestMessage message);
        Task HandlePaymentResultAsync(PaymentResultMessage message);
        Task StartConsumingPaymentResultsAsync();
    }
}
