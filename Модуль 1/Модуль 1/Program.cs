using System;
Console.Write("Введите число ");


if (int.TryParse(Console.ReadLine(), out int number))
{

    if (number % 2 == 0)
    {

        Console.WriteLine($"Число {number} - четное ");
    }
    else
    {
        Console.WriteLine($"Число {number} - нечёт ");
    }

}
