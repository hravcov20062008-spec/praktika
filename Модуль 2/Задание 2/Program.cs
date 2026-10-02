using System;

class Program
{
    static void Main()
    {
        // Создаем круг с радиусом 5
        Circle circle = new Circle { Radius = 5 };
        Console.WriteLine($"Круг - Площадь: {Math.Round(circle.Area(), 2)}, Периметр: {Math.Round(circle.Perimeter(), 2)}");

        // Создаем прямоугольник со сторонами 4 и 6
        Rectangle rect = new Rectangle { Width = 4, Height = 6 };
        Console.WriteLine($"Прямоугольник - Площадь: {rect.Area()}, Периметр: {rect.Perimeter()}");
    }
}

// Базовый класс для всех фигур
class Shape
{
    //  используем  virtual, чтобы методы можно было переписать ниже
    public virtual double Area() => 0;
    public virtual double Perimeter() => 0;
}

// Класс Круг наследует класс Shape
class Circle : Shape
{
    public double Radius { get; set; }

    // Перезаписываем (override) методы под формулы круга
    public override double Area() => Math.PI * Radius * Radius;   // π * r²
    public override double Perimeter() => 2 * Math.PI * Radius;   // 2 * π * r
}

// Класс Прямоугольник наследует класс Shape
class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    // Перезаписываем (override) методы под формулы прямоугольника
    public override double Area() => Width * Height;              // a * b
    public override double Perimeter() => 2 * (Width + Height);   // 2 * (a + b)
}
