using System;

class Program
{
    static void Main()
    {
        // 1. Создаем объекты авторов
        Author author1 = new Author { Name = "Александр Пушкин", BirthYear = 1799 };
        Author author2 = new Author { Name = "Лев Толстой", BirthYear = 1828 };

        // 2. Создаем объекты книг и привязываем авторов внутрь (композиция)
        Book book1 = new Book { Title = "Капитанская дочка", ReleaseYear = 1836, Writer = author1 };
        Book book2 = new Book { Title = "Война и мир", ReleaseYear = 1869, Writer = author2 };
        Book book3 = new Book { Title = "Руслан и Людмила", ReleaseYear = 1820, Writer = author1 };

        // 3. Выводим информацию о книгах и их авторах
        Console.WriteLine("Список книг в библиотеке");
        PrintBookInfo(book1);
        PrintBookInfo(book2);
        PrintBookInfo(book3);
    }

    // Вспомогательный метод для красивого вывода информации
    static void PrintBookInfo(Book book)
    {
        Console.WriteLine($"Книга: \"{book.Title}\" ({book.ReleaseYear} г.)");
        // Через точку заходим внутрь объекта Writer, чтобы достать данные автора
        Console.WriteLine($"Автор: {book.Writer.Name} (род. в {book.Writer.BirthYear} г.)");
        Console.WriteLine();
    }
}

// Класс Автора
class Author
{
    public string Name { get; set; }
    public int BirthYear { get; set; }
}

// Класс Книги
class Book
{
    public string Title { get; set; }
    public int ReleaseYear { get; set; }

    // Композиция: свойство имеет тип созданного нами класса Author
    public Author Writer { get; set; }
}
