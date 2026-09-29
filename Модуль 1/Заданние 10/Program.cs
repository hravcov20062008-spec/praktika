using System;

class Program
{
    static void Main()
    {
        // Ввод размера массива K
        Console.Write("Введите размер массива K: ");
        if (!int.TryParse(Console.ReadLine(), out int k) || k <= 0)
        {
            Console.WriteLine("Ошибка: K должно быть целым положительным числом.");
            return;
        }

        // Строка со всеми буквами русского алфавита (в нижнем регистре)
        string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        // Строка, содержащая только согласные буквы русского алфавита
        string consonantsList = "бвгджзйклмнпрстфхцчшщ";

        char[] firstArray = new char[k];
        Random random = new Random();

        // Заполнение первого массива случайными буквами
        for (int i = 0; i < k; i++)
        {
            // Выбираем случайный индекс из строки-алфавита
            int randomIndex = random.Next(0, alphabet.Length);
            firstArray[i] = alphabet[randomIndex];
        }

        // Подсчет количества согласных букв для определения размера второго массива
        int consonantsCount = 0;
        for (int i = 0; i < k; i++)
        {
            // Если согласные буквы содержат текущий символ первого массива
            if (consonantsList.Contains(firstArray[i].ToString()))
            {
                consonantsCount++;
            }
        }

        // Создание и заполнение второго массива только согласными буквами
        char[] secondArray = new char[consonantsCount];
        int nextIndex = 0;

        for (int i = 0; i < k; i++)
        {
            if (consonantsList.Contains(firstArray[i].ToString()))
            {
                secondArray[nextIndex] = firstArray[i];
                nextIndex++; // Сдвигаем указатель для второго массива
            }
        }

        // Вывод результатов
        Console.WriteLine("\nПервый массив (все случайные буквы):");
        Console.WriteLine(string.Join(" ", firstArray));

        Console.WriteLine($"\nВторой массив (только согласные буквы, всего {consonantsCount} шт.):");
        if (consonantsCount > 0)
        {
            Console.WriteLine(string.Join(" ", secondArray));
        }
        else
        {
            Console.WriteLine("[В первом массиве не оказалось согласных букв]");
        }
    }
}
