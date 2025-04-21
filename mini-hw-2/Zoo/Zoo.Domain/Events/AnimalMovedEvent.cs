using System;
using Zoo.Domain.Entities;

namespace Zoo.Domain.Events
{
	public record AnimalMovedEvent : BaseDomainEvent
    {
        public Guid AnimalId { get; }
        public Guid InitialEnclosureId { get; }
        public Guid FinalEnclosureId { get; }

        public AnimalMovedEvent(Guid animalId, Guid initialEnclosureId, Guid finalEnclosureId)
		{
			AnimalId = animalId;
			InitialEnclosureId = initialEnclosureId;
			FinalEnclosureId = finalEnclosureId;
		}
	}
}

