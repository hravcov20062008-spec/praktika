using System;

class Program
{
    static void Main()
    {
        // Исходный список данных (массив строк)
        string[] items = { "Купить яблоки", "Сдать лабораторную", "Купить хлеб", "Помыть полы" };

        // Создаем два разных фильтра с помощью коротких лямбд "=>"
        // Фильтр 1: строка должна содержать слово "Купить"
        Predicate<string> buyFilter = text => text.Contains("Купить");

        // Фильтр 2: длина строки должна быть больше 15 символов
        Predicate<string> longFilter = text => text.Length > 15;

        // Выбираем, какой фильтр применить (например, фильтр по ключевому слову "Купить")
        Predicate<string> chosenFilter = longFilter;

        // Пробегаем по всем элементам массива
        foreach (string item in items)
        {
            // Динамически вызываем выбранный делегат для каждого элемента.
            // Если элемент подходит под условие фильтра (вернул true) — выводим его.
            if (chosenFilter(item))
            {
                Console.WriteLine(item);
            }
        }
    }
}
