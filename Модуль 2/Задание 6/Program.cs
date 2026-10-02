using System;

class Program
{
    static void Main()
    {
        // 1. Создаем объект автомобиля
        Car car = new Car { Brand = "BMW", Model = "M5 F90", Year = 2017, Price = 200000 };

        Console.WriteLine($"Автомобиль: {car.Brand} {car.Model} ({car.Year} г.)\nБазовая цена: {car.Price} руб.");

        // 2. Считаем цену со скидкой 10% и итоговую цену с НДС 20%
        double priceWithDiscount = car.GetDiscountPrice(10);

        Console.WriteLine($"Цена со скидкой 10%: {priceWithDiscount} руб.");
        Console.WriteLine($"Итоговая цена с НДС (20%): {car.GetPriceWithVat(priceWithDiscount)} руб.");
    }
}

class Car
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public double Price { get; set; }

    // Короткие методы в одну строку через стрелочный синтаксис =>
    public double GetDiscountPrice(double pct) => Price * (1 - pct / 100);
    public double GetPriceWithVat(double currentPrice) => currentPrice * 1.20; // 1.20 — это +20% НДС
}