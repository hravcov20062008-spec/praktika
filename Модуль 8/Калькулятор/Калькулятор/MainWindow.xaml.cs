
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Калькулятор
{
    public partial class MainWindow : Window
    {
        // Переменная для хранения первого числа выражения
        private double firstNumber;

        // Переменная для хранения выбранной операции (+, -, *, /)
        private string operation = "";

        // Флаг, который определяет, нужно ли начинать ввод нового числа
        private bool startNewNumber = true;

        public MainWindow()
        {
            InitializeComponent(); // Загружаем элементы интерфейса из XAML
        }

        // Метод для преобразования текста в число
        // Поддерживает ввод дробных чисел с точкой и запятой
        private bool TryGetNumber(string text, out double number)
        {
            // Заменяем запятую на точку для единообразного преобразования
            text = text.Replace(',', '.');

            // Преобразуем строку в число и возвращаем результат преобразования
            return double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out number);
        }

        // Обработчик нажатия кнопок с цифрами от 0 до 9
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            // Получаем цифру с нажатой кнопки
            string digit = ((Button)sender).Content.ToString();

            // Если начинается ввод нового числа или ранее возникла ошибка
            if (startNewNumber || Display.Text == "Ошибка")
            {
                Display.Text = digit; // Выводим новую цифру
                startNewNumber = false; // Разрешаем продолжать ввод числа
            }
            // Если на экране только ноль
            else if (Display.Text == "0")
            {
                Display.Text = digit; // Заменяем ноль новой цифрой
            }
            else
            {
                Display.Text += digit; // Добавляем цифру в конец числа
            }
        }

        // Обработчик нажатия кнопок математических операций
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            // Получаем первое число с экрана
            if (!TryGetNumber(Display.Text, out firstNumber))
                return; // Если преобразование не удалось, прекращаем выполнение

            // Получаем знак операции из свойства Tag кнопки
            operation = ((Button)sender).Tag.ToString();

            // Следующая введённая цифра начнёт второе число
            startNewNumber = true;
        }

        // Обработчик нажатия кнопки равно (=)
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли операция и корректно ли введено второе число
            if (operation == "" ||
                !TryGetNumber(Display.Text, out double secondNumber))
                return;

            // Переменная для хранения результата вычисления
            double result;

            // Выбираем действие в зависимости от знака операции
            switch (operation)
            {
                case "+":
                    result = firstNumber + secondNumber; // Сложение
                    break;

                case "-":
                    result = firstNumber - secondNumber; // Вычитание
                    break;

                case "*":
                    result = firstNumber * secondNumber; // Умножение
                    break;

                case "/":
                    // Проверяем, не равен ли делитель нулю
                    if (secondNumber == 0)
                    {
                        Display.Text = "Ошибка"; // Выводим сообщение об ошибке
                        operation = ""; // Сбрасываем операцию
                        startNewNumber = true; // Готовимся к новому вводу
                        return;
                    }

                    result = firstNumber / secondNumber; // Деление
                    break;

                default:
                    return; // Если операция неизвестна, выходим из метода
            }

            // Проверяем, является ли результат корректным числом
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                Display.Text = "Ошибка"; // Выводим ошибку
            }
            else
            {
                // Преобразуем результат в строку без явного формата "G"
                Display.Text = result.ToString(
                    CultureInfo.InvariantCulture);
            }

            // Сбрасываем операцию после вычисления
            operation = "";

            // Следующий ввод начнёт новое число
            startNewNumber = true;
        }

        // Обработчик кнопки полной очистки (C)
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0"; // Возвращаем экран к нулю
            firstNumber = 0; // Обнуляем первое число
            operation = ""; // Удаляем выбранную операцию
            startNewNumber = true; // Подготавливаем калькулятор к новому вводу
        }

        // Обработчик удаления последнего символа
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            // Ничего не удаляем, если ввод нового числа ещё не начат
            // или на экране отображается ошибка
            if (startNewNumber || Display.Text == "Ошибка")
                return;

            // Если в строке больше одного символа, удаляем последний
            if (Display.Text.Length > 1)
            {
                Display.Text = Display.Text.Substring(
                    0, Display.Text.Length - 1);

                // Если после удаления остался только минус,
                // заменяем его нулём
                if (Display.Text == "-")
                    Display.Text = "0";
            }
            else
            {
                // Если осталась одна цифра, возвращаем ноль
                Display.Text = "0";
            }
        }

        // Обработчик смены знака числа (+/-)
        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            // Пытаемся получить число с экрана
            if (TryGetNumber(Display.Text, out double number))
            {
                // Меняем знак числа на противоположный
                Display.Text = (-number).ToString(
                    CultureInfo.InvariantCulture);
            }
        }

        // Обработчик ввода десятичного разделителя
        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            // Если начинается новое число или на экране ошибка
            if (startNewNumber || Display.Text == "Ошибка")
            {
                Display.Text = "0."; // Начинаем дробное число с нуля
                startNewNumber = false; // Фиксируем начало ввода
            }
            // Проверяем, что разделитель ещё не введён
            else if (!Display.Text.Contains(".") &&
                     !Display.Text.Contains(","))
            {
                Display.Text += "."; // Добавляем десятичный разделитель
            }
        }
    }
}
