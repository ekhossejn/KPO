using System;
using System.Collections.Concurrent;
using Zoo.Domain.Entities;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Infrastructure.Repositories
{
    public class EnclosureRepository : IEnclosureRepository
    {
        private readonly ConcurrentDictionary<Guid, Enclosure> _store = new();

        public Task<IEnumerable<Enclosure>> GetEnclosuresAsync() =>
            Task.FromResult<IEnumerable<Enclosure>>(_store.Values);

        public Task<Enclosure?> GetEnclosureByIdAsync(Guid id) =>
            Task.FromResult(_store.TryGetValue(id, out var enclosure) ? enclosure : null);

        public Task<Enclosure?> GetEnclosureByAnimalIdAsync(Guid animalId)
        {
            var enclosure = _store.Values.FirstOrDefault(enclosure => enclosure.CurrentAnimals.Contains(animalId));
            return Task.FromResult(enclosure);
        }

        public Task<bool> AddEnclosureAsync(Enclosure enclosure) =>
            Task.FromResult(_store.TryAdd(enclosure.Id, enclosure));

        public Task<bool> UpdateEnclosureAsync(Enclosure enclosure)
        {
            if (_store.ContainsKey(enclosure.Id))
            {
                _store[enclosure.Id] = enclosure;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task RemoveEnclosureAsync(Guid id) => Task.FromResult(_store.TryRemove(id, out _));
    }
}

