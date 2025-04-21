using System;
using Zoo.Domain.Entities;

namespace Zoo.Domain.Events
{
	public record FeedingTimeEvent: BaseDomainEvent
    {
        public Guid AnimalId { get; }
        public Guid FeedingScheduleId { get; }

        public FeedingTimeEvent(Guid animalId, Guid scheduleId)
        {
            AnimalId = animalId;
            FeedingScheduleId = scheduleId;
        }
    }
}

