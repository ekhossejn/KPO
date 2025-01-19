namespace task;
public class Car{
    private static readonly Random _random = new();
    public required int Number{get; set;}
    public Engine Engine{get; init;}

    public Car(){
        Engine = new Engine{Size = _random.Next(1, 5) };
    }
    public override string ToString()
    {
        return $"Авто с номером: {Number} и размером педалей: {Engine.Size}";
    }
}