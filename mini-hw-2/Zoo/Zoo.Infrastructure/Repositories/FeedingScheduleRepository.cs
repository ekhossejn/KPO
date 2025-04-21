using System;
using System.Collections.Concurrent;
using Zoo.Domain.Entities;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Infrastructure.Repositories
{
    public class FeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly ConcurrentDictionary<Guid, FeedingSchedule> _store = new();

        public Task<IEnumerable<FeedingSchedule>> GetSchedulesAsync() =>
            Task.FromResult<IEnumerable<FeedingSchedule>>(_store.Values);

        public Task<FeedingSchedule?> GetScheduleByIdAsync(Guid id) =>
            Task.FromResult(_store.TryGetValue(id, out var schedule) ? schedule : null);

        public Task<IEnumerable<FeedingSchedule>> GetScheduleByAnimalIdAsync(Guid animalId) =>
            Task.FromResult(_store.Values.Where(schedule => schedule.AnimalId == animalId).AsEnumerable());

        public Task<bool> AddScheduleAsync(FeedingSchedule feedingSchedule) =>
            Task.FromResult(_store.TryAdd(feedingSchedule.Id, feedingSchedule));

        public Task<bool> UpdateScheduleAsync(FeedingSchedule feedingSchedule)
        {
            if (_store.ContainsKey(feedingSchedule.Id))
            {
                _store[feedingSchedule.Id] = feedingSchedule;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task RemoveScheduleAsync(Guid id) => Task.FromResult(_store.TryRemove(id, out _));
    }
}

