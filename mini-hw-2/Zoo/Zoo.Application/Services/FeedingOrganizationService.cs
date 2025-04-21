using MediatR;
using Zoo.Application.Interfaces;
using Zoo.Domain.Events;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Application.Services
{
	public class FeedingOrganizationService : IFeedingOrganizationService
	{
        private readonly IAnimalRepository _animalStore;
        private readonly IFeedingScheduleRepository _scheduleStore;
        private readonly IMediator _mediatR;

        public FeedingOrganizationService(IAnimalRepository animals, IFeedingScheduleRepository schedules, IMediator mediatR)
        {
            _animalStore = animals;
            _scheduleStore = schedules;
            _mediatR = mediatR;
        }

        public async Task FeedAnimalAsync(Guid animalId, Guid scheduleId)
        {
            var animal = await _animalStore.GetAnimalByIdAsync(animalId)
                ?? throw new ArgumentException("Animal not found.");

            var schedule = await _scheduleStore.GetScheduleByIdAsync(scheduleId)
                ?? throw new ArgumentException("Schedule not found.");
            animal.Feed();
            schedule.Complete();
            await _mediatR.Publish(new FeedingTimeEvent(animalId, scheduleId));
        }
    }
}

