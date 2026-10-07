using System;

// Делегат (указатель на метод, который принимает текст задачи)
public delegate void TaskAction(string text);

// Класс задачи
public class MyTask
{
    public string Text;          // Название задачи
    public TaskAction Action;    // Делегат (действие), которое выполнит задачу

    // Конструктор: сразу записываем текст и нужное действие
    public MyTask(string t, TaskAction a) { Text = t; Action = a; }
}

class Program
{
    static void Main()
    {
        // Создаем два возможных действия через короткие лямбды "=>"
        TaskAction sms = msg => Console.WriteLine($"[SMS] Напоминание: {msg}");
        TaskAction log = msg => Console.WriteLine($"[Журнал] Записано в файл: {msg}");


        // Создаем две разные задачи и вручную выбираем для них делегаты (действия)
        MyTask task1 = new MyTask("Купить хлеб", sms);
        MyTask task2 = new MyTask("Помыть полы", log);

        // Запускаем делегат каждой задачи. 
        task1.Action(task1.Text); // Вызовет отправку SMS
        task2.Action(task2.Text); // Вызовет запись в журнал
    }
}
