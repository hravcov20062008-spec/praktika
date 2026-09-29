using System;

class Program
{
    static void Main()
    {
        //Ввод размера массива K и границ диапазона [A, B)
        Console.Write("Введите размер массива K: ");
        if (!int.TryParse(Console.ReadLine(), out int k) || k <= 0)
        {
            Console.WriteLine("Ошибка: K должно быть целым положительным числом.");
            return;
        }

        Console.Write("Введите нижнюю границу диапазона A (включая): ");
        int a = int.Parse(Console.ReadLine());

        Console.Write("Введите верхнюю границу диапазона B (не включая): ");
        int b = int.Parse(Console.ReadLine());

        if (a >= b)
        {
            Console.WriteLine("Ошибка: граница A должна быть меньше границы B.");
            return;
        }

        //Создание и заполнение массива случайными числами
        int[] array = new int[k];
        Random random = new Random();

        Console.WriteLine("\nСгенерированный массив:");
        for (int i = 0; i < k; i++)
        {
            array[i] = random.Next(a, b); // Генерирует числа в диапазоне [A, B)
            Console.Write(array[i] + " ");
        }
        Console.WriteLine("\n");

        //Поиск индексов минимального и максимального элементов
        int minIndex = 0;
        int maxIndex = 0;

        for (int i = 1; i < k; i++)
        {
            if (array[i] < array[minIndex])
            {
                minIndex = i; // Нашли новый минимум, запоминаем индекс
            }

            if (array[i] > array[maxIndex])
            {
                maxIndex = i; // Нашли новый максимум, запоминаем индекс
            }
        }

        Console.WriteLine($"Индекс минимального элемента: {minIndex} (значение: {array[minIndex]})");
        Console.WriteLine($"Индекс максимального элемента: {maxIndex} (значение: {array[maxIndex]})\n");

        //Определение границ для вывода элементов (кто из них левее, а кто правее)
        int startIndex = Math.Min(minIndex, maxIndex);
        int endIndex = Math.Max(minIndex, maxIndex);

        //Вывод элементов между найденными (включая их)
        Console.WriteLine("Элементы между минимальным и максимальным (включительно):");
        for (int i = startIndex; i <= endIndex; i++)
        {
            Console.Write(array[i] + " ");
        }
        Console.WriteLine();
    }
}
