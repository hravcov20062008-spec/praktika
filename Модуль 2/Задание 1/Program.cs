using System;

class Program
{
    static void Main()
    {
        // Создаем первого человека и сразу заполняем данные
        Person person1 = new Person { Name = "Алексей", Age = 25, Address = "г. Минск, ул. Ленина, д. 5" };

        // Создаем второго человека
        Person person2 = new Person { Name = "Мария", Age = 19, Address = "г. Орша, ул. Мира, д. 12" };

        // Выводим информацию на экран
        Console.WriteLine($"Человек 1: {person1.Name}, {person1.Age} лет, адрес: {person1.Address}");
        Console.WriteLine($"Человек 2: {person2.Name}, {person2.Age} лет, адрес: {person2.Address}");
    }
}

class Person
{
   
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
}
