namespace Zoo.Application.Interfaces
{
	public interface IFeedingOrganizationService
	{
        Task FeedAnimalAsync(Guid animalId, Guid feedingScheduleId);
    }
}

