using System;
using System.Linq;

class Program
{
    static void Main()
    {
        // Создаем и заполняем массив из 10 студентов случайными данными
        Student[] students = GetSampleStudents();

        // Сортируем студентов по возрастанию среднего балла через LINQ
        var sortedStudents = students.OrderBy(s => s.Grades.Average()).ToArray();

        Console.WriteLine("Все студенты (по возрастанию среднего балла):");
        foreach (var s in sortedStudents)
            Console.WriteLine($"{s.Name}  Группа: {s.GroupNumber}  Ср. балл: {s.Grades.Average():F1}");

        // Фильтруем: выводим тех, у кого ВСЕ оценки равны либо 4, либо 5
        Console.WriteLine("\nСтуденты, имеющие оценки только 4 или 5:");
        var smartStudents = sortedStudents.Where(s => s.Grades.All(g => g == 4 || g == 5));

        foreach (var s in smartStudents)
            Console.WriteLine($"Фамилия: {s.Name}, Группа: {s.GroupNumber}");
    }

    // Вспомогательный метод для быстрой генерации данных
    static Student[] GetSampleStudents()
    {
        Random r = new Random();
        string[] names = { "Иванов", "Петров", "Сидоров", "Козлов", "Новиков", "Морозов", "Павлов", "Соколов", "Попов", "Васильев" };
        return names.Select(name => new Student
        {
            Name = name,
            GroupNumber = r.Next(101, 105),
            Grades = Enumerable.Range(0, 5).Select(_ => r.Next(3, 6)).ToArray()
        }).ToArray();
    }
}

// Компактная структура Student
struct Student
{
    public string Name;
    public int GroupNumber;
    public int[] Grades;
}
