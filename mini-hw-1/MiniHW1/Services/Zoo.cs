using MiniHW1.Interfaces;
using MiniHW1.Models;

namespace MiniHW1.Services
{
    public class Zoo
    {
        private readonly IHealthChecker _healthChecker;
        private readonly List<Animal> _animals = new();
        private readonly List<Thing> _things = new();

        public Zoo(IHealthChecker healthChecker) {
            _healthChecker = healthChecker;
        }

        public void AddAnimal(Animal animal) {
            if (!_healthChecker.IsHealthy(animal))
            {
                Console.WriteLine($"{animal.GetType().Name} с номером {animal.Number} недостаточно здоров. Его нельзя взять в зоопарк");
                return;
            }
            _animals.Add(animal);
            Console.WriteLine($"{animal.GetType().Name} с номером {animal.Number} берется в зоопарк");
        }

        public void AddThing(Thing thing)
        {
            _things.Add(thing);
            Console.WriteLine($"{thing.GetType().Name} с номером {thing.Number} берется в зоопарк");
        }
        
        public int GetNeededFood()
        {
            return _animals.Sum(animal => animal.Food);
        }

        public List<Animal> GetAnimals()
        {
            return _animals;
        }

        public List<Thing> GetThings()
        {
            return _things;
        }

        public List<Herbo> GetInteractiveZooAnimals()
        {
            return _animals.OfType<Herbo>().Where(a => a.IsAbleToBeInInteractiveZoo()).ToList();
        }
    }
}

