namespace task;

public class FactoryAF
{
    public List<Car> Cars { get; private set; }
    public List<Customer> Customers { get; private set; }
    
    public FactoryAF(List<Customer> customers)
    {
        Cars = new List<Car>();
        Customers = customers;
    }

    internal void SaleCar()
    {
        List<Customer> LeftCustomers = new List<Customer>();
        for (int i = 0; i < Customers.Count; ++i){
            if (i < Cars.Count){
                Customers[i].Car = Cars[i];
            }
            else {
                LeftCustomers.Add(Customers[i]);
            }
        }
        Customers = LeftCustomers;
        Cars.Clear();
    }

    internal void AddCar()
    {
        var car = new Car { Number = Cars.Count + 1 };
        Cars.Add(car);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, Cars) + "\n" + string.Join(Environment.NewLine, Customers);
    }
}