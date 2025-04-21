using Zoo.Application.Interfaces;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Application.Services
{
	public class AnimalReleaseService : IAnimalReleaseService
	{
        private readonly IAnimalRepository _animalStore;
        private readonly IEnclosureRepository _enclosureStore;
        private readonly IFeedingScheduleRepository _scheduleStore;

        public AnimalReleaseService(IAnimalRepository animals,
            IEnclosureRepository enclosures, IFeedingScheduleRepository schedules)
        {
            _animalStore = animals;
            _enclosureStore = enclosures;
            _scheduleStore = schedules;
        }

        public async Task ReleaseAnimalAsync(Guid id)
        {
            var animal = await _animalStore.GetAnimalByIdAsync(id) ?? throw new ArgumentException($"Animal not found.");

            // Удаление всех расписаний кормления
            var feedingSchedules = await _scheduleStore.GetScheduleByAnimalIdAsync(id);
            if (feedingSchedules != null)
            {
                foreach (var schedule in feedingSchedules)
                {
                    await _scheduleStore.RemoveScheduleAsync(schedule.Id);
                }
            }

            // Удаление животного из вольера
            var enclosure = await _enclosureStore.GetEnclosureByAnimalIdAsync(id);
            enclosure?.RemoveAnimal(animal.Id);

            // Удаление животного из зоопарка
            await _animalStore.RemoveAnimalAsync(id);
        }
    }
}

