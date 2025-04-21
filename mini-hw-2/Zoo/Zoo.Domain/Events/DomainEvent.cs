using System;
using MediatR;
namespace Zoo.Domain.Events
{
    public abstract record BaseDomainEvent : INotification
    {
        public DateTime Time { get; init; }
        public BaseDomainEvent(DateTime time = default)
        {
            if (time == default)
            {
                Time = DateTime.UtcNow;
            }
            else
            {
                Time = time;
            }
        }
    }
}

