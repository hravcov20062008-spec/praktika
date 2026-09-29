using System;

class Program
{
    static void Main()
    {
        // Ввод размера квадратной матрицы N x N
        Console.Write("Введите размер квадратной матрицы N: ");
        if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
        {
            Console.WriteLine("Ошибка: размер матрицы должен быть целым положительным числом.");
            return;
        }

        // Инициализация двумерного массива и генератора чисел
        int[,] matrix = new int[n, n];
        Random random = new Random();

        // Заполнение матрицы случайными числами от -50 до 50
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = random.Next(-50, 51); // Включая -50 и 50
            }
        }

        Console.WriteLine("\nИсходная матрица:");
        PrintMatrixWithSums(matrix, n);

        // Сортировка строк методом пузырька по суммам элементов
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                // Вычисляем суммы для текущей и следующей строк
                int sumCurrent = GetRowSum(matrix, j, n);
                int sumNext = GetRowSum(matrix, j + 1, n);

                // Если сумма текущей строки больше, чем у следующей — меняем их местами
                if (sumCurrent > sumNext)
                {
                    SwapRows(matrix, j, j + 1, n);
                }
            }
        }

        Console.WriteLine("\nМатрица после упорядочивания строк по возрастанию сумм:");
        PrintMatrixWithSums(matrix, n);
    }

    /// Метод для подсчета суммы элементов конкретной строки
    static int GetRowSum(int[,] matrix, int rowIndex, int cols)
    {
        int sum = 0;
        for (int j = 0; j < cols; j++)
        {
            sum += matrix[rowIndex, j];
        }
        return sum;
    }
    /// Метод для обмена двух строк матрицы местами
    static void SwapRows(int[,] matrix, int row1, int row2, int cols)
    {
        for (int j = 0; j < cols; j++)
        {
            int temp = matrix[row1, j];
            matrix[row1, j] = matrix[row2, j];
            matrix[row2, j] = temp;
        }
    }
    /// Вспомогательный метод для красивого вывода матрицы вместе с суммами строк
    static void PrintMatrixWithSums(int[,] matrix, int n)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write($"{matrix[i, j]}\t");
            }
            // В конце строки выводим её сумму в квадратных скобках
            int sum = GetRowSum(matrix, i, n);
            Console.WriteLine($"| Сумма = {sum}");
        }
    }
}
