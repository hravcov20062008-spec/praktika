using System;

// Делегат (указатель на метод)
public delegate double AreaDelegate();

// Базовый класс
public class Figure
{
    public virtual string Name => "Фигура";
    public virtual double GetArea() => 0;
}

// Три фигуры (Круг, Прямоугольник, Треугольник)
public class Circle : Figure
{
    private double r;
    public override string Name => "Круг";
    public Circle(double radius) => r = radius;
    public override double GetArea() => Math.PI * r * r;
}

public class Rectangle : Figure
{
    private double w, h;
    public override string Name => "Прямоугольник";
    public Rectangle(double width, double height) { w = width; h = height; }
    public override double GetArea() => w * h;
}

public class Triangle : Figure
{
    private double b, h;
    public override string Name => "Треугольник";
    public Triangle(double baseLen, double height) { b = baseLen; h = height; }
    public override double GetArea() => 0.5 * b * h;
}

class Program
{
    static void Main()
    {
        // Создаем массив и сразу кладем туда все три фигуры с их размерами
        Figure[] figures = {
            new Circle(5),          // Круг с радиусом 5
            new Rectangle(4, 5),    // Прямоугольник 4 на 5
            new Triangle(6, 4)      // Треугольник с основанием 6 и высотой 4
        };

        foreach (Figure f in figures)
        {
            AreaDelegate calc = f.GetArea;
            // Берем метод GetArea текущей фигуры из цикла и записываем в делегат

            double result = calc();
            // Динамически вызываем этот метод через делегат (каждый раз выполнится разная формула)

            Console.WriteLine($"Фигура: {f.Name} : Площадь: {result:F2}");
            // Выводим имя текущей фигуры и ее посчитанную площадь 
        }
    }
}
