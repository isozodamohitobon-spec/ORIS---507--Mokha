namespace Workers;

public class Bag
{
    public Size Size { get; set;}
    public string Name { get; set;}
    public Brand Brand { get; set;}
    public string Color { get; set;}

    public Bag( Size size ,string name , Brand brand , string color )
    {
        Size = size;
        Name = name;
        Brand = brand;
        Color = color;
    }
}
public class Brand
{ 
    public string Country { get; set; }
    public string Name { get; set; }
}
public class Size
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }
}   