namespace Zoo.Application.Interfaces
{
	public interface IZooStatisticsService
	{
        Task<int> GetAnimalsNumberAsync();
        Task<int> GetHealthyAnimalsNumberAsync();
        Task<int> GetEnclosuresNumberAsync();
        Task<int> GetFeedingSchedulesNumberAsync();
    }
}
