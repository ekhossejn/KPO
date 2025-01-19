namespace task;
public class Program
{
    static void Main()
    {
        var customers = new List<Customer>
        {
            new() { Name = "Minion 1" },
            new() { Name = "Minion 2" },
            new() { Name = "Minion 3" },
            new() { Name = "Minion 4" },
            new() { Name = "Minion 5" },
        };
        var factory = new FactoryAF(customers);
        for (int i = 0; i < 3; ++i){
            factory.AddCar();
        }
        Console.WriteLine("Before SaleCar(): ");
        Console.WriteLine(string.Join(Environment.NewLine, factory));
        factory.SaleCar();
        Console.WriteLine("After SaleCar():");
        Console.WriteLine(string.Join(Environment.NewLine, factory));
    }
}