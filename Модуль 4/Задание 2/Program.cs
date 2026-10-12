using System;

// Интерфейс (контракт) для любого товара
public interface IProduct
{
    string Name { get; }         // Свойство: Название товара
    int Stock { get; }           // Свойство: Остаток на складе
    double GetTotalValue();      // Метод: Расчет стоимости всей партии
}

// Класс Продукты питания
public class Food : IProduct
{
    private double price; // Цена за одну единицу товара
    public string Name { get; } // Реализация названия
    public int Stock { get; }   // Реализация остатка

    // Конструктор: заполняем данные товара
    public Food(string name, double price, int stock) { Name = name; this.price = price; Stock = stock; }

    // Стоимость партии = цена умножить на остаток
    public double GetTotalValue() => price * Stock;
}

// Класс Одежда
public class Clothes : IProduct
{
    private double cost; // Цена за одну вещь
    public string Name { get; } // Реализация названия
    public int Stock { get; }   // Реализация остатка

    // Конструктор: заполняем данные товара
    public Clothes(string name, double cost, int stock) { Name = name; this.cost = cost; Stock = stock; }

    // Стоимость партии = цена умножить на остаток
    public double GetTotalValue() => cost * Stock;
}

class Program
{
    static void Main()
    {
        // Создаем массив товаров на складе (массив интерфейсов)
        IProduct[] warehouse = { new Food("Яблоки", 120.5, 50), new Clothes("Куртка", 4500, 10) };

        // Пробегаем по всем товарам в массиве
        foreach (var p in warehouse)
        {
            // Выводим имя, остаток и общую стоимость партии товара
            Console.WriteLine($"Товар: {p.Name}  На складе: {p.Stock} шт.  Всего на сумму: {p.GetTotalValue():F2} руб.");
        }
    }
}
