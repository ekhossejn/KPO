namespace Zoo.Application.Interfaces
{
	public interface IAnimalTransferService
	{
        public Task TransferAnimalAsync(Guid animalId, Guid finalEnclosureId);
    }
}

