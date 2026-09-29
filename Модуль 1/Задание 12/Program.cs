using System;

class Program
{
    static void Main()
    {

        // Ввод числителя (неотрицательное число)
        Console.Write("Введите неотрицательный числитель: ");
        if (!int.TryParse(Console.ReadLine(), out int numerator) || numerator < 0)
        {
            Console.WriteLine("Ошибка: числитель должен быть целым неотрицательным числом.");
            return;
        }

        // Ввод знаменателя (положительное число, больше нуля)
        Console.Write("Введите положительный знаменатель: ");
        if (!int.TryParse(Console.ReadLine(), out int denominator) || denominator <= 0)
        {
            Console.WriteLine("Ошибка: знаменатель должен быть целым положительным числом (больше 0).");
            return;
        }

        // Вывод исходной дроби
        Console.WriteLine($"\nИсходная дробь: {numerator}/{denominator}");

        // Обработка частного случая, если числитель равен 0
        if (numerator == 0)
        {
            Console.WriteLine("Сокращенная дробь: 0");
            return;
        }

        // Вычисление НОД с помощью нашего статического метода
        int gcd = GetGcd(numerator, denominator);

        // Сокращение дроби (деление числителя и знаменателя на НОД)
        int finalNumerator = numerator / gcd;
        int finalDenominator = denominator / gcd;

        // Вывод результата
        Console.WriteLine($"Наибольший общий делитель (НОД): {gcd}");

        if (finalDenominator == 1)
        {
            // Если знаменатель стал равен 1, выводим просто как целое число
            Console.WriteLine($"Сокращенная дробь: {finalNumerator}");
        }
        else
        {
            Console.WriteLine($"Сокращенная дробь: {finalNumerator}/{finalDenominator}");
        }
    }
    static int GetGcd(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
