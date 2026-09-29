using System;

class Program
{
    static void Main()
    {
        //Инициализируем целочисленный массив из 10 элементов
        int[] array = { 12, 5, -3, 24, 0, 18, 100, 9, 31, 7 };

        // Выводим исходный массив на экран для наглядности
        Console.WriteLine("Исходный массив:");
        Console.WriteLine(string.Join(" ", array));

        //Ищем максимальный элемент и его позицию (индекс)
        int max = array[0];
        int maxIndex = 0; // Изначально считаем, что максимум на 0-й позиции

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];     // Обновляем максимальное значение
                maxIndex = i;       // Запоминаем, где оно лежит
            }
        }

        Console.WriteLine($"\nМаксимальный элемент в массиве: {max} (находится на позиции {maxIndex})");

        // Объявляем переменную 
        int newValue;

        //Запрашиваем у пользователя новое целое число
        Console.Write("Введите целое число для замены: ");
        while (!int.TryParse(Console.ReadLine(), out newValue))
        {
            Console.Write("Некорректный ввод. Пожалуйста, введите целое число: ");
        }

        //Производим замену по сохраненному индексу
        array[maxIndex] = newValue;

        
        Console.WriteLine("\nИзмененный массив:");
        Console.WriteLine(string.Join(" ", array));
    }
}
