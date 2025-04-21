using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
	public class FeedingSchedule
	{
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool IsCompleted { get; private set; } = false;

        public Guid AnimalId { get; private set; }
        public TimeField FeedingTime { get; private set; }
        public string Food { get; private set; }

        public FeedingSchedule(Guid animalId, TimeField feedingTime, string food)
        {
            AnimalId = animalId;
            FeedingTime = feedingTime;
            Food = food;
        }

        public void UpdateSchedule(TimeField newFeedingTime) => FeedingTime = newFeedingTime;

        public void Complete() => IsCompleted = true;
    }
}

