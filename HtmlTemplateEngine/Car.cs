namespace Workers;

public class Car
{
    public int Year { get; set;}
    public string Name { get; set;}
    public string Marka { get; set;}

    public Car(int year, string name, string marka)
    {
        Year = year;
        Name = name;
        Marka = marka;
    }
}