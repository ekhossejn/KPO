using System;
using Zoo.Domain.Entities;
namespace Zoo.Infrastructure.Interfaces
{
	public interface IAnimalRepository
	{
        public Task<IEnumerable<Animal>> GetAnimalsAsync();
        public Task<IEnumerable<Animal>> GetHealthyAnimalsAsync();
        public Task<Animal?> GetAnimalByIdAsync(Guid id);

        public Task<bool> AddAnimalAsync(Animal animal);
        public Task<bool> UpdateAnimalAsync(Animal animal);
        public Task RemoveAnimalAsync(Guid id);
    }
}

