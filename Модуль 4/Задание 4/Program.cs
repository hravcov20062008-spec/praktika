using System;

// Интерфейс (контракт) для любой книги
public interface IBook
{
    string GetTitle();        // Метод для получения названия книги
    bool CheckAvailability(); // Метод для проверки, доступна ли книга
    void Borrow();            // Метод для выдачи книги
}

// Класс бумажной книги
public class PaperBook : IBook
{
    private string title; // Переменная для хранения названия
    private int copies;   // Переменная для количества копий на полке

    // Конструктор: сохраняем переданные данные в переменные класса
    public PaperBook(string title, int copies)
    {
        this.title = title;
        this.copies = copies;
    }

    // Возвращаем сохраненное название
    public string GetTitle()
    {
        return title;
    }

    // Книга доступна, если на полке осталось больше 0 штук
    public bool CheckAvailability()
    {
        if (copies > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Выдача книги читателю
    public void Borrow()
    {
        // Проверяем доступность через наш же метод
        if (CheckAvailability() == true)
        {
            copies = copies - 1; // Уменьшаем количество книг на полке на одну
            Console.WriteLine("[Физ] Книга " + title + " выдана. Осталось на полке: " + copies);
        }
        else
        {
            Console.WriteLine("[Ошибка] Книги " + title + " сейчас нет в наличии");
        }
    }
}

// Класс электронной книги
public class EBook : IBook
{
    private string title; // Переменная для хранения названия

    // Конструктор: сохраняем название электронной книги
    public EBook(string title)
    {
        this.title = title;
    }

    // Возвращаем название
    public string GetTitle()
    {
        return title;
    }

    // Электронная книга доступна абсолютно всегда
    public bool CheckAvailability()
    {
        return true;
    }

    // Выдача электронной книги
    public void Borrow()
    {
        Console.WriteLine("[Сайт] Ссылка на скачивание книги " + title + " отправлена на Email");
    }
}

class Program
{
    static void Main()
    {
        // Создаем массив книг (каталог нашей библиотеки)
        IBook[] library = {
            new PaperBook("C# для новичков", 1), // Бумажная (1 штука)
            new PaperBook("Чистый код", 0),       // Бумажная (0 штук)
            new EBook("Шаблоны проектирования")  // Электронная
        };

        // По очереди берем каждую книгу из массива
        foreach (IBook book in library)
        {
            // Узнаем название и проверяем доступность
            string currentTitle = book.GetTitle();
            bool isAvailableNow = book.CheckAvailability();

            // Выводим информацию на экран
            Console.WriteLine("Книга: " + currentTitle + "  Доступна: " + isAvailableNow);

            // Вызываем метод выдачи книги
            book.Borrow();

            
        }
    }
}
