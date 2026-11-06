namespace Workers;

class Program
{
    static void Main()
    {
        var render = new HtmlTemplate();
        String template = @"<p>Имя:${user.Name}</p>
                          <p>Группа:${user.Group}</p>  
                          <P>Возраст:${user.Age}</p>";

        User user = new User(name: "Вася", group: "11-507", age: 19, gender: true);
        Console.WriteLine(render.RenderFromString(template,user));
        
        String template1 = @"<p>Автор:${book.Name}</p>
                          <p>Титул:${book.title}</p>  
                          <P>Дата:${book.year}</p>";
            
        Book book = new Book("Война и мир", "Толстой", 1869);
        Console.WriteLine(render.RenderFromString(template1,book));
        
        String template2 = @"<p>Дата:${car.Year}</p>
                          <p>Название:${car.Name}</p>  
                          <P>Марка:${car.Marka}</p>";
        Car car = new Car(2027, "Машина", "Ferrari");
        Console.WriteLine(render.RenderFromString(template2,car));
        
        String template3 = @"<p>Название:${bag.name}</p>
                          <p>Размер:${bag.size}</p>  
                          <P>Бренд:${bag.brand}</p>
                          <P>Цвет:${bag.color}</p>";

        Brand chanelBrand = new Brand { Country = "Franch", Name = "Chanel" }; 
        Size bigSize = new Size
        {
            Width = 30, Depth = 20, Height = 50
        };
        Bag bag = new Bag(size: bigSize, name: "Сумка", brand: chanelBrand, color: "Черный");
        Console.WriteLine(render.RenderFromString(template3,bag));

    }
}
