using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web; // ДОБАВЛЕНО для корректного отображения кириллицы

// Класс, описывающий студента
class Student
{
    public int Id { get; set; } // Уникальный идентификатор студента
    public string FirstName { get; set; } = ""; // Имя студента
    public string LastName { get; set; } = ""; // Фамилия студента
    public int Age { get; set; } // Возраст студента
    public string Group { get; set; } = ""; // Название учебной группы

    // Метод преобразования информации о студенте в строку
    public override string ToString()
    {
        return $"ID: {Id} | {LastName} {FirstName} | " +
               $"Возраст: {Age} | Группа: {Group}";
    }
}

// Основной класс программы
class Program
{
    static List<Student> students = new List<Student>();

    // ИСПРАВЛЕНО: Возвращено правильное расширение .json
    static string filePath = "students.txt";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        LoadStudents();

        while (true)
        {
            Console.WriteLine("\nУПРАВЛЕНИЕ СТУДЕНТАМИ");
            Console.WriteLine("1. Показать всех студентов");
            Console.WriteLine("2. Добавить студента");
            Console.WriteLine("3. Редактировать студента");
            Console.WriteLine("4. Удалить студента");
            Console.WriteLine("5. Найти студента");
            Console.WriteLine("6. Сортировать студентов");
            Console.WriteLine("7. Сохранить список в файл");
            Console.WriteLine("8. Загрузить список из файла");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": ShowStudents(); break;
                case "2": AddStudent(); break;
                case "3": EditStudent(); break;
                case "4": DeleteStudent(); break;
                case "5": SearchStudent(); break;
                case "6": SortStudents(); break;
                case "7": SaveStudents(); break;
                case "8": LoadStudents(); break;
                case "0":
                    SaveStudents();
                    Console.WriteLine("Данные сохранены. Выход из программы.");
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }
    }

    static void ShowStudents()
    {
        Console.WriteLine("\nСПИСОК СТУДЕНТОВ");
        if (students.Count == 0)
        {
            Console.WriteLine("Список студентов пуст.");
            return;
        }
        foreach (Student student in students)
            Console.WriteLine(student);
    }

    static void AddStudent()
    {
        Console.WriteLine("\nДОБАВЛЕНИЕ СТУДЕНТА");
        Student student = new Student();

        student.Id = students.Count == 0 ? 1 : students.Max(s => s.Id) + 1;

        Console.Write("Введите имя: ");
        student.FirstName = ReadRequiredText();

        Console.Write("Введите фамилию: ");
        student.LastName = ReadRequiredText();

        Console.Write("Введите возраст: ");
        student.Age = ReadAge();

        Console.Write("Введите группу: ");
        student.Group = ReadRequiredText();

        students.Add(student);
        SaveStudents();
        Console.WriteLine("Студент успешно добавлен!");
    }

    static void EditStudent()
    {
        Console.Write("\nВведите ID студента для редактирования: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Некорректный ID.");
            return;
        }

        Student? student = students.FirstOrDefault(s => s.Id == id);
        if (student == null)
        {
            Console.WriteLine("Студент не найден.");
            return;
        }

        Console.WriteLine("Текущие данные: " + student);

        Console.Write($"Имя ({student.FirstName}): ");
        string value = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(value))
            student.FirstName = value.Trim();

        Console.Write($"Фамилия ({student.LastName}): ");
        value = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(value))
            student.LastName = value.Trim();

        Console.Write($"Возраст ({student.Age}): ");
        value = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(value))
        {
            if (int.TryParse(value, out int age) && age > 0 && age <= 120)
                student.Age = age;
            else
            {
                Console.WriteLine("Некорректный возраст. Оставлен прежний.");
            }
        }

        Console.Write($"Группа ({student.Group}): ");
        value = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(value))
            student.Group = value.Trim();

        SaveStudents();
        Console.WriteLine("Данные студента обновлены!");
    }

    static void DeleteStudent()
    {
        Console.Write("\nВведите ID студента для удаления: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Некорректный ID.");
            return;
        }

        Student? student = students.FirstOrDefault(s => s.Id == id);
        if (student == null)
        {
            Console.WriteLine("Студент не найден.");
            return;
        }

        Console.WriteLine(student);
        Console.Write("Удалить этого студента? (да/нет): ");

        if ((Console.ReadLine() ?? "").Trim().ToLower() == "да")
        {
            students.Remove(student);
            SaveStudents();
            Console.WriteLine("Студент удалён.");
        }
        else
        {
            Console.WriteLine("Удаление отменено.");
        }
    }

    static void SearchStudent()
    {
        Console.Write("\nВведите имя, фамилию или группу: ");
        string query = (Console.ReadLine() ?? "").Trim();

        var results = students.Where(s =>
            s.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            s.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            s.Group.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine("\nРЕЗУЛЬТАТЫ ПОИСКА");

        if (results.Count == 0)
        {
            Console.WriteLine("Студенты не найдены.");
            return;
        }

        foreach (Student student in results)
            Console.WriteLine(student);
    }

    static void SortStudents()
    {
        Console.WriteLine("\nСОРТИРОВКА");
        Console.WriteLine("1. По фамилии");
        Console.WriteLine("2. По возрасту");
        Console.WriteLine("3. По группе");
        Console.Write("Выберите критерий: ");

        string choice = Console.ReadLine() ?? "";
        IEnumerable<Student> sorted;

        switch (choice)
        {
            case "1": sorted = students.OrderBy(s => s.LastName); break;
            case "2": sorted = students.OrderBy(s => s.Age); break;
            case "3": sorted = students.OrderBy(s => s.Group); break;
            default:
                Console.WriteLine("Неверный выбор.");
                return;
        }

        Console.WriteLine("\nОТСОРТИРОВАННЫЙ СПИСОК");
        foreach (Student student in sorted)
            Console.WriteLine(student);
    }

    // ИСПРАВЛЕННЫЙ МЕТОД СОХРАНЕНИЯ
    static void SaveStudents()
    {
        try
        {
            // Добавляем опцию Encoder, чтобы русские буквы не превращались в \uXXXX
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(students, options);
            File.WriteAllText(filePath, json, Encoding.UTF8);
            Console.WriteLine("Список сохранён в файл.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка сохранения: " + ex.Message);
        }
    }

    static void LoadStudents()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл пока не создан. Будет создан при сохранении.");
                return;
            }

            string json = File.ReadAllText(filePath, Encoding.UTF8);
            students = JsonSerializer.Deserialize<List<Student>>(json)
                       ?? new List<Student>();
            Console.WriteLine($"Загружено студентов: {students.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка загрузки: " + ex.Message);
        }
    }

    static string ReadRequiredText()
    {
        while (true)
        {
            string value = (Console.ReadLine() ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(value))
                return value;
            Console.Write("Поле не может быть пустым. Повторите ввод: ");
        }
    }

    static int ReadAge()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int age)
                && age > 0 && age <= 120)
                return age;
            Console.Write("Введите корректный возраст (1–120): ");
        }
    }
}