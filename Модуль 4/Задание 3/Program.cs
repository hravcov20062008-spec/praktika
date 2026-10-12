using System;
using System.Linq;

// Интерфейс-контракт для любого студента
public interface IStudent
{
    string GetName();       // Имя студента
    int GetCourse();        // Номер курса
    double GetAverageGrade(); // Расчет среднего балла
}

// Класс для студентов младших курсов (1-2 курс)
public class JuniorStudent : IStudent
{
    private string name;
    private int course;
    private int[] grades; // Оценки за домашние работы

    public JuniorStudent(string name, int course, int[] grades)
    {
        this.name = name;
        this.course = course;
        this.grades = grades;
    }

    public string GetName() => name;
    public int GetCourse() => course;
    // Считаем среднее через встроенный метод .Average()
    public double GetAverageGrade() => grades.Average();
}

// Класс для студентов старших курсов (3-4 курс)
public class SeniorStudent : IStudent
{
    private string name;
    private int course;
    private double examGrade; // Оценка строго за сложный экзамен

    public SeniorStudent(string name, int course, double examGrade)
    {
        this.name = name;
        this.course = course;
        this.examGrade = examGrade;
    }

    public string GetName() => name;
    public int GetCourse() => course;
    // У старших курсников средний балл равен экзаменационному
    public double GetAverageGrade() => examGrade;
}

class Program
{
    static void Main()
    {
        // Общая база данных университета (массив интерфейсов)
        IStudent[] university = {
            new JuniorStudent("Иван", 1, new int[] { 4, 5, 4, 3 }),
            new JuniorStudent("Анна", 2, new int[] { 5, 5, 4, 5 }),
            new SeniorStudent("Олег", 4, 4.8),
            new SeniorStudent("Мария", 3, 4.2)
        };

        Console.WriteLine("СПИСОК СТУДЕНТОВ");

        // Выводим данные каждого студента через интерфейс
        foreach (IStudent student in university)
        {
            Console.WriteLine($"Студент: {student.GetName()}  Курс: {student.GetCourse()}  Средний балл: {student.GetAverageGrade():F1}");
        }
    }
}
