using Zoo.Application.Interfaces;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Application.Services
{
	public class ZooStatisticsService : IZooStatisticsService
	{
        private readonly IAnimalRepository _animalStore;
        private readonly IEnclosureRepository _enclosureStore;
        private readonly IFeedingScheduleRepository _feedingScheduleStore;

        public ZooStatisticsService(IAnimalRepository animals, IEnclosureRepository enclosures,
            IFeedingScheduleRepository schedules)
        {
            _animalStore = animals;
            _enclosureStore = enclosures;
            _feedingScheduleStore = schedules;
        }

        public async Task<int> GetAnimalsNumberAsync()
        {
            var animals = await _animalStore.GetAnimalsAsync();
            return animals.Count();
        }

        public async Task<int> GetHealthyAnimalsNumberAsync()
        {
            var animals = await _animalStore.GetHealthyAnimalsAsync();
            return animals.Count();
        }

        public async Task<int> GetEnclosuresNumberAsync()
        {
            var enclosures = await _enclosureStore.GetEnclosuresAsync();
            return enclosures.Count();
        }

        public async Task<int> GetFeedingSchedulesNumberAsync()
        {
            var enclosures = await _feedingScheduleStore.GetSchedulesAsync();
            return enclosures.Count();
        }
    }
}

