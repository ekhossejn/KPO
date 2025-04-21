using System;
using Zoo.Domain.Entities;

namespace Zoo.Infrastructure.Interfaces
{
	public interface IFeedingScheduleRepository
	{
        Task<IEnumerable<FeedingSchedule>> GetSchedulesAsync();
        Task<FeedingSchedule?> GetScheduleByIdAsync(Guid id);
        Task<IEnumerable<FeedingSchedule>> GetScheduleByAnimalIdAsync(Guid animalId);

        Task<bool> AddScheduleAsync(FeedingSchedule feedingSchedule);
        public Task<bool> UpdateScheduleAsync(FeedingSchedule feedingSchedule);
        Task RemoveScheduleAsync(Guid id);
    }
}

