using System;

class Program
{
    static void Main()
    {
        // Создаем вещественный массив из 10 элементов
        double[] sourceArray = new double[10];
        Random random = new Random();

        // Заполняем случайными вещественными числами в диапазоне [-10, 10)
        Console.WriteLine("Исходный вещественный массив:");
        for (int i = 0; i < sourceArray.Length; i++)
        {
            // random.NextDouble() дает от 0.0 до 1.0. 
            // Формула: min + (max - min) * random.NextDouble()
            sourceArray[i] = -10.0 + (10.0 - (-10.0)) * random.NextDouble();

            // Выводим число с округлением до 2 знаков после запятой для читаемости
            Console.Write($"{Math.Round(sourceArray[i], 2)}\t");
        }
        Console.WriteLine("\n");

        // Формируем массив индексов и заполняем его значениями от 0 до 9
        int[] indexArray = new int[10];
        for (int i = 0; i < indexArray.Length; i++)
        {
            indexArray[i] = i;
        }

        // Сортируем массив ИНДЕКСОВ на основе значений из ИСХОДНОГО массива
        // Используем простой метод пузырька для наглядности алгоритма
        for (int i = 0; i < indexArray.Length - 1; i++)
        {
            for (int j = 0; j < indexArray.Length - i - 1; j++)
            {
                // Сравниваем не сами индексы, а элементы исходного массива, на которые они указывают!
                if (sourceArray[indexArray[j]] > sourceArray[indexArray[j + 1]])
                {
                    // Меняем индексы местами
                    int temp = indexArray[j];
                    indexArray[j] = indexArray[j + 1];
                    indexArray[j + 1] = temp;
                }
            }
        }

        // Выводим полученный массив индексов
        Console.WriteLine("Сформированный массив индексов (в порядке возрастания значений):");
        Console.WriteLine(string.Join(" ", indexArray));
        Console.WriteLine();

        //  Демонстрируем, что исходный массив отсортирован по этим индексам
        Console.WriteLine("Проверка (вывод исходного массива по полученным индексам):");
        for (int i = 0; i < indexArray.Length; i++)
        {
            int idx = indexArray[i]; // берем индекс
            Console.Write($"{Math.Round(sourceArray[idx], 2)}\t"); // выводим элемент под этим индексом
        }
        Console.WriteLine();
    }
}
