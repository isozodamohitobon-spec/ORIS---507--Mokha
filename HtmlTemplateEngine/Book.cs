namespace Workers;

public class Book
{
    public string Title { get; set;}
    public string Autor { get; set;}
    public int Year { get; set;}

    public Book(string title , string autor, int year )
    {
        Title = title;
        Autor = autor;
        Year = year;
    }
}