using MediatR;
using Zoo.Domain.Events;

namespace Zoo.Application.EventHandler
{
    public class AnimalMovedEventHandler : INotificationHandler<AnimalMovedEvent>
    {
        public Task Handle(AnimalMovedEvent notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (notification.InitialEnclosureId == Guid.Empty)
                {
                    Console.WriteLine($"[{notification.Time}] Animal with id {notification.AnimalId} is put to " +
                        $"enclosure with id {notification.FinalEnclosureId}");
                }
                else if (notification.InitialEnclosureId != notification.FinalEnclosureId)
                {
                    Console.WriteLine($"[{notification.Time}] Animal with id {notification.AnimalId} transfered " +
                        $"from enclosure with id {notification.InitialEnclosureId} to enclosure with id {notification.FinalEnclosureId}");
                }
            }, cancellationToken);
        }
    }
}

