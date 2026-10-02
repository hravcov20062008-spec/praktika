using System;

class Program
{
    static void Main()
    {
        TemperatureSensor sensor = new TemperatureSensor();
        Thermostat thermostat = new Thermostat();

        // Подписываем метод термостата на событие датчика через оператор +=
        sensor.TemperatureChanged += thermostat.Reaction;

        // Меняем температуру — событие автоматически срабатывает
        sensor.ChangeTemperature(15); // Холодно
        sensor.ChangeTemperature(25); // Жара
    }
}

class TemperatureSensor
{
    // Объявляем событие, которое будет передавать одно число (градусы)
    public event Action<int> TemperatureChanged;

    public void ChangeTemperature(int temp)
    {
        Console.WriteLine($"\nНа датчике: {temp}°C");
        // Запускаем событие для всех подписчиков
        TemperatureChanged?.Invoke(temp);
    }
}
class Thermostat
{
    // Метод, который автоматически вызывается при срабатывании события
    public void Reaction(int temp)
    {
        if (temp < 18) Console.WriteLine("Термостат: Холодно! ВКЛЮЧАЮ отопление.");
        else Console.WriteLine("Термостат: Тепло. ВЫКЛЮЧАЮ отопление.");
    }
}
