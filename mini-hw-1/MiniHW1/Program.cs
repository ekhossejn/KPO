using Microsoft.Extensions.DependencyInjection;
using MiniHW1.Models;
using MiniHW1.Interfaces;
using MiniHW1.Services;

namespace MiniHW1
{
    public class Program
    {
        private static int _number = 0;
        private static int NextNumber
        {
            get
            {
                _number++;
                return _number;
            }
        }

        static void Main()
        {
            var manager = new ServiceCollection()
                .AddTransient<IHealthChecker, VeterinaryClinic>()
                .AddTransient<Zoo>()
                .BuildServiceProvider();
            var zoo = manager.GetRequiredService<Zoo>();

            Console.WriteLine("Добавление Herbo");
            // Herbo                [food, health (>5), number, kind (>5)].
            zoo.AddAnimal(new Monkey(1, 7, NextNumber, 10)); // 1 - health(+) kind(+).
            zoo.AddAnimal(new Rabbit(1, 6, NextNumber, 2)); // 2 - health(+) kind(-).
            zoo.AddAnimal(new Monkey(1, 5, NextNumber, 7)); // 3 - health(-) kind(+).
            zoo.AddAnimal(new Rabbit(1, 5, NextNumber, 5)); // 4 - health(-) kind(-).
            Console.WriteLine();

            Console.WriteLine("Добавление Predator");
            // Predator            [food, health (>5), number].
            zoo.AddAnimal(new Tiger(1, 7, NextNumber)); // 5 - health(+).
            zoo.AddAnimal(new Wolf(1, 6, NextNumber)); // 6 - health(+).
            zoo.AddAnimal(new Tiger(1, 5, NextNumber)); // 7 - health(-).
            zoo.AddAnimal(new Wolf(1, 5, NextNumber)); // 8 - health(-).
            Console.WriteLine();

            Console.WriteLine("Добавление Thing");
            zoo.AddThing(new Table(NextNumber)); // 9
            zoo.AddThing(new Computer(NextNumber)); // 10
            zoo.AddThing(new Table(NextNumber)); // 11
            zoo.AddThing(new Table(NextNumber)); // 12
            Console.WriteLine();

            Console.WriteLine("Всего еды:");
            Console.WriteLine(zoo.GetNeededFood());
            Console.WriteLine();

            Console.WriteLine("Animals:");
            foreach (var animal in zoo.GetAnimals())
            {
                Console.WriteLine(animal);
            }
            Console.WriteLine();

            Console.WriteLine("Things:");
            foreach (var thing in zoo.GetThings())
            {
                Console.WriteLine(thing);
            }
            Console.WriteLine();

            Console.WriteLine("Контактный зоопарк:");
            foreach (var animal in zoo.GetInteractiveZooAnimals())
            {
                Console.WriteLine(animal);
            }
            Console.WriteLine();
        }
    }
}