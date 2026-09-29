using System;

class Program
{
    static void Main()
    {
        //Создаем массив на 20 элементов и генератор случайных чисел
        int[] numbers = new int[20];
        Random random = new Random();

        //Заполняем массив случайными числами от 1 до 100 и выводим их на экран
        Console.WriteLine("Сгенерированный массив чисел:");
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = random.Next(1, 101); // генерирует числа от 1 до 100
            Console.Write(numbers[i] + " ");
        }
        Console.WriteLine("\n"); // Отступ для читаемости

        //Инициализируем переменные для поиска минимума и максимума первого элемента
        int min = numbers[0];
        int max = numbers[0];

        //Проходим по массиву и ищем экстремумы
        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
            {
                max = numbers[i]; // если нашли число больше текущего max, обновляем его
            }

            if (numbers[i] < min)
            {
                min = numbers[i]; // если нашли число меньше текущего min, обновляем его
            }
        }

      
        Console.WriteLine($"Максимальное значение: {max}");
        Console.WriteLine($"Минимальное значение: {min}");
    }
}
