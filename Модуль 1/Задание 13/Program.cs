using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {

        // Ввод целевого числа от пользователя
        Console.Write("Введите пороговое число (целое положительное): ");
        if (!int.TryParse(Console.ReadLine(), out int maxSum) || maxSum <= 0)
        {
            Console.WriteLine("Ошибка: нужно ввести целое число больше 0.");
            return;
        }

        // Инициализируем генератор случайных чисел и динамический список
        Random random = new Random();
        List<int> elementsList = new List<int>();

        int currentSum = 0;

        // Цикл генерации элементов
        while (true)
        {
            // Генерируем случайное число от 1 до 9 включительно
            int nextValue = random.Next(1, 10);

            // Проверяем: если добавить это число, превысит ли сумма лимит?
            if (currentSum + nextValue > maxSum)
            {
                // Если превышает, останавливаем генерацию
                break;
            }

            // Если всё в порядке, добавляем элемент и обновляем сумму
            elementsList.Add(nextValue);
            currentSum += nextValue;
        }

        // Переводим список в обычный массив (как требует условие задачи)
        int[] resultArray = elementsList.ToArray();

        // Вывод результатов
        Console.WriteLine($"\nЗаданное число: {maxSum}");
        Console.WriteLine($"Количество элементов в массиве: {resultArray.Length}");
        Console.WriteLine($"Итоговая сумма элементов: {currentSum}");

        if (resultArray.Length > 0)
        {
            Console.WriteLine("Элементы массива:");
            Console.WriteLine(string.Join(" ", resultArray));
        }
        else
        {
            Console.WriteLine("Массив пуст (даже первое случайное число превысило заданный лимит).");
        }
    }
}
