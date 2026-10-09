using System;

// Класс "Уведомление" с тремя событиями
public class Notification
{
    // События для сообщений, звонков и писем
    public event Action<string> OnMessage;
    public event Action<string> OnCall;
    public event Action<string> OnEmail;


    // Метод для запуска события OnMessage. Если на СМС кто-то подписан (?.), отправляет им текст.
    public void SendMessage(string text) => OnMessage?.Invoke(text);

    // Метод для запуска события OnCall. Проверяет подписки и передает имя контакта для звонка.
    public void SendCall(string contact) => OnCall?.Invoke(contact);

    // Метод для запуска события OnEmail. Проверяет подписки и передает тему пришедшего письма.
    public void SendEmail(string subject) => OnEmail?.Invoke(subject);

}

class Program
{
    static void Main()
    {

        Notification app = new Notification();

        // Подписываемся на методы 
        app.OnMessage += text => Console.WriteLine($"[SMS] На экране: {text}");
        app.OnMessage += text => Console.WriteLine($"[Вибрация] Жжж!"); 

        app.OnCall += contact => Console.WriteLine($"[Звонок] Звонит: {contact}");
        app.OnEmail += subject => Console.WriteLine($"[Email] Тема: {subject}");

        app.SendMessage("Привет!");
        app.SendCall("Мама");
        app.SendEmail("Отчет");
    }
}
