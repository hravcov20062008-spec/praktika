using System;

// Интерфейс — контракт для всех фигур
public interface IFigure
{
    double GetArea();
    double GetPerimeter();
}

// Круг
public class Circle : IFigure
{
    private double r;
    public Circle(double radius) => r = radius;
    public double GetArea() => Math.PI * r * r;
    public double GetPerimeter() => 2 * Math.PI * r;
}

// Прямоугольник
public class Rectangle : IFigure
{
    private double w, h;
    public Rectangle(double width, double height) { w = width; h = height; }
    public double GetArea() => w * h;
    public double GetPerimeter() => 2 * (w + h);
}

// Треугольник (простой: прямоугольный, где a и b — катеты, c — гипотенуза)
public class Triangle : IFigure
{
    private double a, b, c;
    public Triangle(double sideA, double sideB, double sideC) { a = sideA; b = sideB; c = sideC; }
    public double GetArea() => 0.5 * a * b; // Простая площадь для прямоугольного треугольника
    public double GetPerimeter() => a + b + c;
}

class Program
{
    static void Main()
    {
        IFigure[] figures = { new Circle(5), new Rectangle(4, 5), new Triangle(3, 4, 5) };

        foreach (var f in figures)
        {
            Console.WriteLine($"{f.GetType().Name} - Площадь: {f.GetArea():F1}, Периметр: {f.GetPerimeter():F1}");
        }
    }
}
