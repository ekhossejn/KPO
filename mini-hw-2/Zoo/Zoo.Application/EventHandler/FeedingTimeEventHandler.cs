using MediatR;
using Zoo.Domain.Events;

namespace Zoo.Application.EventHandler
{
    public class FeedingTimeEventHandler : INotificationHandler<FeedingTimeEvent>
    {
        public Task Handle(FeedingTimeEvent notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"[{notification.Time}] Feed for animal with id {notification.AnimalId} " +
                    $"due to schedule with id {notification.FeedingScheduleId}");
            }, cancellationToken);
        }
    }
}