using System;
Console.WriteLine("Введите радиус круга");

if (double.TryParse(Console.ReadLine(), out double radius))
{
    if (radius >= 0)
    {
        double area = Math.PI * Math.Pow(radius, 2);

        Console.WriteLine($"Площадь круга {radius} равна:{area}");
    }
    else
    {
        Console.WriteLine("Неверное число либо ошибка");
    }
}
