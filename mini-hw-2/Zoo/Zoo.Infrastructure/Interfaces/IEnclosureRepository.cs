using System;
using Zoo.Domain.Entities;

namespace Zoo.Infrastructure.Interfaces
{
	public interface IEnclosureRepository
	{
        public Task<IEnumerable<Enclosure>> GetEnclosuresAsync();
        public Task<Enclosure?> GetEnclosureByIdAsync(Guid id);
        public Task<Enclosure?> GetEnclosureByAnimalIdAsync(Guid animalId);

        public Task<bool> AddEnclosureAsync(Enclosure enclosure);
        public Task<bool> UpdateEnclosureAsync(Enclosure enclosure);
        public Task RemoveEnclosureAsync(Guid id);
    }
}

