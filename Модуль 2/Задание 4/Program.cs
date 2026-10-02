using System;

class Program
{
    static void Main()
    {

        // Создаем массив с конкретными размерами фигур
        IDrawable[] shapes =
        {
            new Circle { Radius = 5 },
            new Rectangle { Width = 4, Height = 6 },
            new Triangle { Base = 3, Height = 5 }
        };

        // Запускаем цикл для вывода информации
        foreach (var shape in shapes)
        {
            shape.Draw();
        }
    }
}

// Интерфейс, который обязывает класс иметь метод Draw
interface IDrawable
{
    void Draw();
}

// Класс Круг с одним свойством
class Circle : IDrawable
{
    public double Radius { get; set; }
    public void Draw() => Console.WriteLine($"Рисуем Круг радиусом {Radius}");
}

// Класс Прямоугольник с двумя свойствами
class Rectangle : IDrawable
{
    public double Width { get; set; }
    public double Height { get; set; }
    public void Draw() => Console.WriteLine($"Рисуем Прямоугольник размером {Width}x{Height}");
}

// Класс Треугольник с основанием и высотой
class Triangle : IDrawable
{
    public double Base { get; set; }
    public double Height { get; set; }
    public void Draw() => Console.WriteLine($"Рисуем Треугольник с основанием {Base} и высотой {Height}");
}
