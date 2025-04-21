namespace Zoo.Application.Interfaces
{
	public interface IAnimalReleaseService
	{
        public Task ReleaseAnimalAsync(Guid animalId);
    }
}

