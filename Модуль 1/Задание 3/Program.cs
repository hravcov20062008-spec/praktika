using System;
class Program

{
    static void Main()
    {
        Console.WriteLine("Введите ваше Имя");
        string Name = Console.ReadLine();

        Console.WriteLine("Введите вашу Фамилию");
        string Fam = Console.ReadLine();

        Console.WriteLine($"Итог - {Name} {Fam}");
    }
}
