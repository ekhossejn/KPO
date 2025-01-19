namespace task;

public class Customer{
    public required string Name { get; set; }
    public Car? Car { get; set; }
    public override string ToString()
    {
        if (Car is not null){
            return $"Покупатель {Name} имеет машину: {Car.ToString()}";
        }
        return $"Покупатель {Name} не имеет машину";
    }
}