using Zoo.Infrastructure.Interfaces;
using Zoo.Application.Interfaces;
using Zoo.Domain.Entities;
using Zoo.Domain.Events;
using MediatR;

namespace Zoo.Application.Services
{
	public class AnimalTransferService : IAnimalTransferService
	{
        private readonly IAnimalRepository _animalStore;
        private readonly IEnclosureRepository _enclosureStore;
        private readonly IMediator _mediatR;

        public AnimalTransferService(IAnimalRepository animals, IEnclosureRepository enclosures, IMediator mediatR)
		{
			_animalStore = animals;
			_enclosureStore = enclosures;
            _mediatR = mediatR;
		}

        public async Task TransferAnimalAsync(Guid animalId, Guid finalEnclosureId)
        {
            var animal = await _animalStore.GetAnimalByIdAsync(animalId)
                ?? throw new ArgumentException("Animal not found.");

            var initialEnclosure = await _enclosureStore.GetEnclosureByIdAsync(animal.EnclosureId);

            var finalEnclosure = await _enclosureStore.GetEnclosureByIdAsync(finalEnclosureId)
               ?? throw new ArgumentException("Final enclosure not found.");

            if (initialEnclosure != null && initialEnclosure == finalEnclosure)
            {
                throw new InvalidOperationException("Animal is already in this enclosure.");
            }

            if (finalEnclosure.IsFull())
            {
                throw new InvalidOperationException("Final enclosure is full.");
            }

            initialEnclosure?.RemoveAnimal(animal.Id);
            finalEnclosure.AddAnimal(animal);
            animal.MoveToNewEnclosure(finalEnclosureId);
            await _mediatR.Publish(new AnimalMovedEvent(animal.Id, initialEnclosure?.Id ?? Guid.Empty, finalEnclosure.Id));
        }
    }
}

