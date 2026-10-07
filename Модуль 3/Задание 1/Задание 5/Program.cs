using System;

class Program
{
    // Создаем тип делегата (указатель на метод, который принимает массив чисел)
    public delegate void SortDelegate(int[] array);

    // Метод А: Сортировка пузырьком (вручную)
    static void BubbleSort(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            for (int j = 0; j < arr.Length - 1; j++)
                if (arr[j] > arr[j + 1])
                {
                    // Меняем элементы местами
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
    }

    // Метод Б: Быстрая/системная сортировка (встроенная в C#)
    static void QuickSort(int[] arr) => Array.Sort(arr);

    static void Main()
    {
        // Исходный неотсортированный массив чисел
        int[] numbers = { 5, 2, 8, 1, 9, 3 };

        // Создаем переменную делегата и кладем туда нужный метод сортировки
        // (Здесь можно легко заменить BubbleSort на QuickSort)
        SortDelegate sorter = QuickSort;

        // Запускаем метод сортировки динамически через делегат
        sorter(numbers);

        // Выводим результат на экран
        Console.WriteLine("Отсортированный массив: " + string.Join(", ", numbers));
    }
}
