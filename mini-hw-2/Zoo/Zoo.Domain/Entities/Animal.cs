using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
	public class Animal
	{
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool IsHungry { get; private set; } = true;

        public AnimalSpecies Species { get; init; }
        public string Name { get; init; }
        public DateField BirthDate { get; init; }
        public AnimalGender Gender { get; init; }
        public string FavoriteFood { get; set; }
        public AnimalHealthStatus HealthStatus { get; set; }
        public EnclosureType SuitableEnclosureType { get; init; }
        public Guid EnclosureId { get; private set; } = Guid.Empty; // Изначально нигде не сидит

        public Animal(AnimalSpecies species, string name, DateField birthDate, AnimalGender gender,
            string favoriteFood, AnimalHealthStatus healthStatus, EnclosureType suitableEnclosureType)
        {
            Species = species;
            Name = name;
            BirthDate = birthDate;
            Gender = gender;
            FavoriteFood = favoriteFood;
            HealthStatus = healthStatus;
            SuitableEnclosureType = suitableEnclosureType;
        }

        public void Feed() => IsHungry = false;

        public void Cure() => HealthStatus = AnimalHealthStatus.Healthy;

        public bool IsHealthy() => HealthStatus == AnimalHealthStatus.Healthy;

        public void BecameHungry() => IsHungry = true;

        public void BecameSick() => HealthStatus = AnimalHealthStatus.Sick;

        public void MoveToNewEnclosure(Guid newEnclosureId) => EnclosureId = newEnclosureId;
    }
}

