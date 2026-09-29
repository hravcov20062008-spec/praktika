using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите размер массива");
        if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
        {
            Console.WriteLine("Введи нормальный массив");
            return;
        }

        double[] array = new double[n];

        Console.WriteLine($"Введите {n} элементов");
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"Элемент [{i}]");
            while (!double.TryParse(Console.ReadLine(), out array[i]))
            {
                Console.WriteLine("Неправильно");
            }
        }
        double maxMod = Math.Abs(array[0]);
        for (int i = 1; i < n; i++)
            {
            if (Math.Abs(array[i])>maxMod)
            {
                maxMod = Math.Abs(array[i]);
            }
    }
        if (maxMod == 0)
        {
            Console.WriteLine("\nВсе элементы массива равны нулю. Нормирование невозможно.");
            return;
        }

        //  Нормирование и вывод измененного массива
        Console.WriteLine($"\nМаксимальный по модулю элемент: {maxMod}");
        Console.WriteLine("Нормированный массив:");
        for (int i = 0; i < n; i++)
        {
            array[i] = array[i] / maxMod;
            // Выводим с округлением до 4 знаков после запятой для красоты
            Console.Write($"{Math.Round(array[i], 4)} ");
        }
        Console.WriteLine();
    }
}