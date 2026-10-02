using System;

class Program
{
    static void Main()
    {

        // Создаем массив различных фигур с конкретными размерами
        Shape[] shapes = new Shape[]
        {
            new Circle { Radius = 5 },
            new Rectangle { Width = 4, Height = 6 },
            new Triangle { Base = 3, Height = 5 }
        };

        // 2. В цикле проверяем работу метода Area() для каждого объекта
        foreach (var shape in shapes)
        {
            Console.WriteLine($"Фигура: {shape.GetName()}  Площадь = {Math.Round(shape.Area(), 2)}");
        }
    }
}

abstract class Shape
{
    // Абстрактный метод не имеет тела — его обязаны написать дочерние классы
    public abstract double Area();

    // Обычный виртуальный метод для удобного вывода названия фигуры
    public virtual string GetName() => "Неизвестная фигура";
}


// Класс Круг
class Circle : Shape
{
    public double Radius { get; set; }
    public override double Area() => Math.PI * Radius * Radius; // S = π * r²
    public override string GetName() => "Круг";
}

// Класс Прямоугольник
class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
    public override double Area() => Width * Height; // S = a * b
    public override string GetName() => "Прямоугольник";
}

// Класс Треугольник
class Triangle : Shape
{
    public double Base { get; set; } // Основание
    public double Height { get; set; } // Высота
    public override double Area() => 0.5 * Base * Height; // S = 0.5 * a * h
    public override string GetName() => "Треугольник";
}
