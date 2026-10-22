using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web; // Пространство имен для управления кодированием символов в JSON

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
        // Формирование строки с данными студента
        return $"ID: {Id} | {LastName} {FirstName} | " +
               $"Возраст: {Age} | Группа: {Group}";
    }
}

// Основной класс программы
class Program
{
    // Список, в котором хранятся все студенты
    static List<Student> students = new List<Student>();

    // Имя файла для хранения данных студентов (оставлено расширение .txt)
    static string filePath = "students.txt";

    // Главный метод, с которого начинается выполнение программы
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8; // Установка кодировки вывода для русского текста
        Console.InputEncoding = Encoding.UTF8; // Установка кодировки ввода для русского текста

        LoadStudents(); // Загрузка студентов из файла при запуске программы

        // Бесконечный цикл, обеспечивающий работу меню
        while (true)
        {
            Console.WriteLine("\nУПРАВЛЕНИЕ СТУДЕНТАМИ"); // Вывод заголовка меню
            Console.WriteLine("1. Показать всех студентов"); // Пункт просмотра списка
            Console.WriteLine("2. Добавить студента"); // Пункт добавления студента
            Console.WriteLine("3. Редактировать студента"); // Пункт изменения данных
            Console.WriteLine("4. Удалить студента"); // Пункт удаления студента
            Console.WriteLine("5. Найти студента"); // Пункт поиска студента
            Console.WriteLine("6. Сортировать студентов"); // Пункт сортировки списка
            Console.WriteLine("7. Сохранить список в файл"); // Пункт сохранения данных
            Console.WriteLine("8. Загрузить список из файла"); // Пункт повторной загрузки данных
            Console.WriteLine("0. Выход"); // Пункт завершения программы
            Console.Write("Выберите действие: "); // Запрос выбора пользователя

            string choice = Console.ReadLine() ?? ""; // Чтение выбора пользователя; если значение отсутствует, используется пустая строка

            // Проверка выбранного пользователем пункта меню
            switch (choice)
            {
                case "1": // Если выбрано действие 1
                    ShowStudents(); // Вызов метода отображения студентов
                    break; // Завершение текущего варианта switch

                case "2": // Если выбрано действие 2
                    AddStudent(); // Вызов метода добавления студента
                    break; // Завершение текущего варианта switch

                case "3": // Если выбрано действие 3
                    EditStudent(); // Вызов метода редактирования студента
                    break; // Завершение текущего варианта switch

                case "4": // Если выбрано действие 4
                    DeleteStudent(); // Вызов метода удаления студента
                    break; // Завершение текущего варианта switch

                case "5": // Если выбрано действие 5
                    SearchStudent(); // Вызов метода поиска студента
                    break; // Завершение текущего варианта switch

                case "6": // Если выбрано действие 6
                    SortStudents(); // Вызов метода сортировки студентов
                    break; // Завершение текущего варианта switch

                case "7": // Если выбрано действие 7
                    SaveStudents(); // Сохранение списка студентов в файл
                    break; // Завершение текущего варианта switch

                case "8": // Если выбрано действие 8
                    LoadStudents(); // Загрузка списка студентов из файла
                    break; // Завершение текущего варианта switch

                case "0": // Если выбрано действие 0
                    SaveStudents(); // Сохранение данных перед завершением работы
                    Console.WriteLine("Данные сохранены. Выход из программы."); // Вывод сообщения о выходе
                    return; // Завершение главного метода и программы

                default: // Если введён неизвестный пункт меню
                    Console.WriteLine("Неверный выбор!"); // Сообщение об ошибке выбора
                    break; // Возврат к началу цикла
            }
        }
    }

    // Метод отображения всех студентов
    static void ShowStudents()
    {
        Console.WriteLine("\nСПИСОК СТУДЕНТОВ"); // Вывод заголовка списка

        // Проверка, есть ли студенты в коллекции
        if (students.Count == 0)
        {
            Console.WriteLine("Список студентов пуст."); // Сообщение о пустом списке
            return; // Завершение метода, поскольку отображать нечего
        }

        // Перебор всех студентов в списке
        foreach (Student student in students)
            Console.WriteLine(student); // Вывод информации о текущем студенте
    }

    // Метод добавления нового студента
    static void AddStudent()
    {
        Console.WriteLine("\nДОБАВЛЕНИЕ СТУДЕНТА"); // Вывод заголовка раздела

        Student student = new Student(); // Создание нового объекта студента

        // Определение ID: если список пуст, используется 1, иначе максимальный ID увеличивается на 1
        student.Id = students.Count == 0
            ? 1 // Начальный ID для первого студента
            : students.Max(s => s.Id) + 1; // Получение максимального ID и увеличение на единицу

        Console.Write("Введите имя: "); // Запрос имени
        student.FirstName = ReadRequiredText(); // Чтение непустого имени

        Console.Write("Введите фамилию: "); // Запрос фамилии
        student.LastName = ReadRequiredText(); // Чтение непустой фамилии

        Console.Write("Введите возраст: "); // Запрос возраста
        student.Age = ReadAge(); // Чтение и проверка возраста

        Console.Write("Введите группу: "); // Запрос названия группы
        student.Group = ReadRequiredText(); // Чтение непустого названия группы

        students.Add(student); // Добавление нового студента в список
        SaveStudents(); // Сохранение обновлённого списка в файл

        Console.WriteLine("Студент успешно добавлен!"); // Сообщение об успешном добавлении
    }

    // Метод редактирования существующего студента
    static void EditStudent()
    {
        Console.Write("\nВведите ID студента для редактирования: "); // Запрос идентификатора студента

        // Попытка преобразовать введённый текст в целое число
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Некорректный ID."); // Сообщение об ошибке ввода
            return; // Завершение метода при неправильном ID
        }

        // Поиск студента с указанным ID
        Student? student = students.FirstOrDefault(s => s.Id == id);

        // Проверка, найден ли студент
        if (student == null)
        {
            Console.WriteLine("Студент не найден."); // Сообщение, если совпадений нет
            return; // Завершение метода
        }

        Console.WriteLine("Текущие данные: " + student); // Отображение текущих данных студента

        Console.Write($"Имя ({student.FirstName}): "); // Показ текущего имени и запрос нового
        string value = Console.ReadLine() ?? ""; // Чтение нового значения
        if (!string.IsNullOrWhiteSpace(value)) // Проверка, что новое имя не пустое
            student.FirstName = value.Trim(); // Установка имени без лишних пробелов по краям

        Console.Write($"Фамилия ({student.LastName}): "); // Показ текущей фамилии и запрос новой
        value = Console.ReadLine() ?? ""; // Чтение новой фамилии
        if (!string.IsNullOrWhiteSpace(value)) // Проверка, что фамилия введена
            student.LastName = value.Trim(); // Обновление фамилии

        Console.Write($"Возраст ({student.Age}): "); // Показ текущего возраста и запрос нового
        value = Console.ReadLine() ?? ""; // Чтение нового возраста
        if (!string.IsNullOrWhiteSpace(value)) // Проверка, что пользователь ввёл значение
        {
            // Проверка, что возраст является числом от 1 до 120
            if (int.TryParse(value, out int age) && age > 0 && age <= 120)
                student.Age = age; // Сохранение нового возраста
            else
            {
                Console.WriteLine("Некорректный возраст. Оставлен прежний."); // Сообщение об ошибке; старый возраст сохраняется
            }
        }

        Console.Write($"Группа ({student.Group}): "); // Показ текущей группы и запрос новой
        value = Console.ReadLine() ?? ""; // Чтение новой группы
        if (!string.IsNullOrWhiteSpace(value)) // Проверка, что группа не пустая
            student.Group = value.Trim(); // Обновление группы

        SaveStudents(); // Сохранение изменений в файл
        Console.WriteLine("Данные студента обновлены!"); // Сообщение об успешном редактировании
    }

    // Метод удаления студента
    static void DeleteStudent()
    {
        Console.Write("\nВведите ID студента для удаления: "); // Запрос ID удаляемого студента

        // Проверка правильности введённого ID
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Некорректный ID."); // Сообщение о неправильном вводе
            return; // Завершение метода
        }

        // Поиск студента по идентификатору
        Student? student = students.FirstOrDefault(s => s.Id == id);

        // Проверка, существует ли такой студент
        if (student == null)
        {
            Console.WriteLine("Студент не найден."); // Сообщение, если студент отсутствует
            return; // Завершение метода
        }

        Console.WriteLine(student); // Отображение данных найденного студента
        Console.Write("Удалить этого студента? (да/нет): "); // Запрос подтверждения удаления

        // Проверка ответа пользователя без учёта пробелов и регистра букв
        if ((Console.ReadLine() ?? "").Trim().ToLower() == "да")
        {
            students.Remove(student); // Удаление студента из списка
            SaveStudents(); // Сохранение обновлённого списка
            Console.WriteLine("Студент удалён."); // Сообщение об успешном удалении
        }
        else
        {
            Console.WriteLine("Удаление отменено."); // Сообщение об отмене удаления
        }
    }

    // Метод поиска студентов по имени, фамилии или группе
    static void SearchStudent()
    {
        Console.Write("\nВведите имя, фамилию или группу: "); // Запрос поискового значения
        string query = (Console.ReadLine() ?? "").Trim(); // Чтение запроса и удаление пробелов по краям

        // Поиск всех студентов, у которых совпадает имя, фамилия или группа
        var results = students.Where(s =>
            s.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) || // Проверка имени без учёта регистра
            s.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) || // Проверка фамилии без учёта регистра
            s.Group.Contains(query, StringComparison.OrdinalIgnoreCase)) // Проверка группы без учёта регистра
            .ToList(); // Преобразование результатов в список

        Console.WriteLine("\nРЕЗУЛЬТАТЫ ПОИСКА"); // Вывод заголовка результатов

        // Проверка наличия найденных студентов
        if (results.Count == 0)
        {
            Console.WriteLine("Студенты не найдены."); // Сообщение об отсутствии результатов
            return; // Завершение метода
        }

        // Перебор найденных студентов
        foreach (Student student in results)
            Console.WriteLine(student); // Вывод данных каждого найденного студента
    }

    // Метод сортировки списка студентов
    static void SortStudents()
    {
        Console.WriteLine("\nСОРТИРОВКА"); // Вывод заголовка раздела
        Console.WriteLine("1. По фамилии"); // Сортировка по фамилии
        Console.WriteLine("2. По возрасту"); // Сортировка по возрасту
        Console.WriteLine("3. По группе"); // Сортировка по группе
        Console.Write("Выберите критерий: "); // Запрос способа сортировки

        string choice = Console.ReadLine() ?? ""; // Чтение выбранного критерия

        IEnumerable<Student> sorted; // Объявление переменной для отсортированной последовательности

        // Выбор способа сортировки
        switch (choice)
        {
            case "1": // Сортировка по фамилии
                sorted = students.OrderBy(s => s.LastName); // Расположение студентов по фамилии
                break; // Завершение варианта

            case "2": // Сортировка по возрасту
                sorted = students.OrderBy(s => s.Age); // Расположение студентов от младших к старшим
                break; // Завершение варианта

            case "3": // Сортировка по группе
                sorted = students.OrderBy(s => s.Group); // Расположение студентов по названию группы
                break; // Завершение варианта

            default: // Если выбран неизвестный критерий
                Console.WriteLine("Неверный выбор."); // Сообщение об ошибке
                return; // Завершение метода
        }

        Console.WriteLine("\nОТСОРТИРОВАННЫЙ СПИСОК"); // Вывод заголовка результата

        // Перебор отсортированных студентов
        foreach (Student student in sorted)
            Console.WriteLine(student); // Вывод данных каждого студента
    }

    // Метод сохранения списка студентов в файл
    static void SaveStudents()
    {
        try // Начало блока обработки возможных ошибок
        {
            // Настройка параметров сериализации JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Включение форматирования JSON с отступами для удобного чтения
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Отключение принудительного экранирования (чтобы русские буквы не превращались в \uXXXX)
            };

            // Преобразование списка студентов в JSON с заданными настройками
            string json = JsonSerializer.Serialize(students, options);

            // Запись JSON в файл в кодировке UTF-8
            File.WriteAllText(filePath, json, Encoding.UTF8);
            Console.WriteLine("Список сохранён в файл."); // Сообщение об успешном сохранении
        }
        catch (Exception ex) // Перехват ошибок, возникших при сохранении
        {
            Console.WriteLine("Ошибка сохранения: " + ex.Message); // Вывод описания ошибки
        }
    }

    // Метод загрузки списка студентов из файла
    static void LoadStudents()
    {
        try // Начало блока обработки ошибок
        {
            // Проверка существования файла
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл пока не создан. Будет создан при сохранении."); // Сообщение об отсутствии файла
                return; // Завершение метода, если файла нет
            }

            // Чтение содержимого файла в кодировке UTF-8
            string json = File.ReadAllText(filePath, Encoding.UTF8);

            // Преобразование JSON-текста обратно в список студентов
            students = JsonSerializer.Deserialize<List<Student>>(json)
                       ?? new List<Student>(); // Создание пустого списка, если результат загрузки отсутствует

            Console.WriteLine($"Загружено студентов: {students.Count}"); // Вывод количества загруженных студентов
        }
        catch (Exception ex) // Перехват ошибок чтения или разбора JSON
        {
            Console.WriteLine("Ошибка загрузки: " + ex.Message); // Вывод описания ошибки
        }
    }

    // Метод чтения обязательного текстового значения
    static string ReadRequiredText()
    {
        while (true) // Повторение ввода до получения непустого значения
        {
            string value = (Console.ReadLine() ?? "").Trim(); // Чтение текста и удаление пробелов по краям

            if (!string.IsNullOrWhiteSpace(value)) // Проверка, что введён хотя бы один символ
                return value; // Возврат введённого значения

            Console.Write("Поле не может быть пустым. Повторите ввод: "); // Запрос повторного ввода
        }
    }

    // Метод чтения и проверки возраста
    static int ReadAge()
    {
        while (true) // Повторение ввода до получения правильного возраста
        {
            // Проверка, что введено целое число от 1 до 120
            if (int.TryParse(Console.ReadLine(), out int age)
                && age > 0 && age <= 120)
                return age; // Возврат корректного возраста

            Console.Write("Введите корректный возраст (1–120): "); // Запрос повторного ввода
        }
    }
}