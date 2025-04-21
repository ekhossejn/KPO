using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
    public class Enclosure
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool IsClean { get; private set; } = true;
        public int CurrentAnimalsNumber => CurrentAnimals.Count;
        public List<Guid> CurrentAnimals { get; private set; } = new List<Guid>();

        public EnclosureType Type { get; init; }
        public PositiveInt AreaM2 { get; init; }
        public PositiveInt Capacity { get; init; }

        public Enclosure(EnclosureType type, PositiveInt area, PositiveInt capacity)
        {
            Type = type;
            AreaM2 = area;
            Capacity = capacity;
        }

        public bool IsFull() => Capacity.Value == CurrentAnimalsNumber;

        public void AddAnimal(Animal animal)
        {
            if (CurrentAnimals.Contains(animal.Id))
            {
                throw new InvalidOperationException("Animal is already added.");
            }

            if (IsFull())
            {
                throw new InvalidOperationException("Enclosure is full.");
            }

            if (animal.SuitableEnclosureType != Type)
            {
                throw new InvalidOperationException("Unsuitable enclosure type.");
            }

            CurrentAnimals.Add(animal.Id);
        }

        public void RemoveAnimal(Guid animalId)
        {
            bool success = CurrentAnimals.Remove(animalId);
            if (!success)
            {
                throw new InvalidOperationException("No such animal in this enclosure.");
            }
        }

        public void CleanUp() => IsClean = true;

        public void BecameDirty() => IsClean = false;
    }
}

