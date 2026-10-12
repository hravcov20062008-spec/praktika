using System;

// Интерфейс-контракт для любого инструмента рисования
public interface IDrawable
{
    void DrawLine(int x1, int y1, int x2, int y2); // Метод рисования линии по координатам
    void DrawCircle(int x, int y, int radius);     // Метод рисования круга (центр и радиус)
    void DrawRectangle(int x, int y, int w, int h); // Метод рисования прямоугольника (точка, ширина, высота)
}

// Класс Холст, который реализует интерфейс рисования
public class Canvas : IDrawable
{
    private string name; // Название холста (например, "Главный холст")

    // Конструктор: задаем имя холста
    public Canvas(string name) => this.name = name;

    // Реализация рисования линии
    public void DrawLine(int x1, int y1, int x2, int y2) =>
        Console.WriteLine($"[{name}] Рисуем ЛИНИЮ из ({x1}, {y1}) в ({x2}, {y2})");

    // Реализация рисования круга
    public void DrawCircle(int x, int y, int r) =>
        Console.WriteLine($"[{name}] Рисуем КРУГ с центром ({x}, {y}) и радиусом {r}");

    // Реализация рисования прямоугольника
    public void DrawRectangle(int x, int y, int w, int h) =>
        Console.WriteLine($"[{name}] Рисуем ПРЯМОУГОЛЬНИК в ({x}, {y}) размером {w}x{h}");
}

class Program
{
    static void Main()
    {
        // Создаем объект холста через интерфейсную переменную
        IDrawable canvas = new Canvas("Холст");


        // Вызываем методы рисования различных фигур
        canvas.DrawLine(0, 0, 10, 10);     // Рисуем линию
        canvas.DrawCircle(5, 5, 20);       // Рисуем круг
        canvas.DrawRectangle(2, 2, 40, 50); // Рисуем прямоугольник
    }
}
