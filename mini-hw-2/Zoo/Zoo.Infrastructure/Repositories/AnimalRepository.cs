using System;
using System.Collections.Concurrent;
using Zoo.Domain.Entities;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ConcurrentDictionary<Guid, Animal> _store = new();

        public Task<IEnumerable<Animal>> GetAnimalsAsync() => Task.FromResult<IEnumerable<Animal>>(_store.Values);

        public Task<IEnumerable<Animal>> GetHealthyAnimalsAsync()
        {
            var healthyAnimals = _store.Values.Where(animal => animal.IsHealthy());
            return Task.FromResult(healthyAnimals);
        }

        public Task<Animal?> GetAnimalByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out Animal? animal);
            return Task.FromResult(animal);
        }

        public Task<bool> AddAnimalAsync(Animal animal) => Task.FromResult(_store.TryAdd(animal.Id, animal));

        public Task<bool> UpdateAnimalAsync(Animal animal)
        {
            if (_store.ContainsKey(animal.Id))
            {
                _store[animal.Id] = animal;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task RemoveAnimalAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}

