using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Задание_5
{
    public partial class MainWindow : Window
    {
        private double firstNumber; // Переменная для хранения первого числа математического выражения
        private string operation = ""; // Строка для хранения знака выбранной математической операции (+, -, *, /)
        private bool startNewNumber = true; // Флаг-индикатор: нужно ли вводить новое число (или продолжать текущий ввод)

        // Получаем текущие региональные настройки операционной системы (для правильной работы точки/запятой в числах)
        private readonly CultureInfo culture = CultureInfo.CurrentCulture;

        public MainWindow()
        {
            InitializeComponent(); // Инициализация графических интерфейсных элементов калькулятора из XAML
        }

        // Обработчик клика по кнопкам с цифрами (0-9)
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            string digit = ((Button)sender).Content.ToString(); // Получаем текст цифры с нажатой кнопки

            if (startNewNumber || Display.Text == "Ошибка") // Если ожидается ввод нового числа или на экране была ошибка
            {
                Display.Text = digit; // Перезаписываем текст экрана новой цифрой
                startNewNumber = false; // Сбрасываем флаг, теперь цифры будут дописываться
            }
            else if (Display.Text == "0") // Если на экране сейчас одиночный ноль
            {
                Display.Text = digit; // Заменяем этот ноль на нажатую цифру (чтобы не было чисел вида "05")
            }
            else // Во всех остальных случаях
            {
                Display.Text += digit; // Дописываем нажатую цифру в конец текущего числа на экране
            }
        }

        // Обработчик клика по кнопкам арифметических операций (+, -, *, /)
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            // Пытаемся преобразовать текст с экрана в число типа double с учетом региональной культуры
            if (!double.TryParse(Display.Text, culture, out firstNumber))
                return; // Если преобразование не удалось, прерываем выполнение метода

            operation = ((Button)sender).Tag.ToString(); // Сохраняем знак операции, который прописан в свойстве Tag кнопки
            startNewNumber = true; // Устанавливаем флаг, что следующее нажатие цифры начнет ввод нового (второго) числа
        }

        // Обработчик клика по кнопке «Равно» (=)
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, задана ли операция и успешно ли переведено второе число с экрана в формат double
            if (operation == "" || !double.TryParse(Display.Text, culture, out double secondNumber))
                return; // Если операции нет или число некорректно, прерываем выполнение метода

            double result; // Переменная для хранения итогового результата вычислений

            // Выбираем действие на основе сохраненного знака операции
            switch (operation)
            {
                case "+": // Если сложение
                    result = firstNumber + secondNumber; // Складываем первое и второе число
                    break;
                case "-": // Если вычитание
                    result = firstNumber - secondNumber; // Вычитаем из первого числа второе
                    break;
                case "*": // Если умножение
                    result = firstNumber * secondNumber; // Перемножаем числа
                    break;
                case "/": // Если деление
                    if (secondNumber == 0) // Важная проверка: если второе число равно нулю
                    {
                        Display.Text = "Ошибка"; // Выводим на экран сообщение о невозможности деления на ноль
                        operation = ""; // Сбрасываем операцию
                        startNewNumber = true; // Переводим калькулятор в режим ожидания нового ввода
                        return; // Прерываем выполнение метода
                    }
                    result = firstNumber / secondNumber; // Если делитель не ноль, производим деление
                    break;
                default: // Если операция не распознана
                    return;
            }

            // Переводим числовой результат в строку формата общего вида ("G") с учетом локальных настроек и выводим на экран
            Display.Text = result.ToString("G", culture);
            operation = ""; // Очищаем операцию после завершения подсчета
            startNewNumber = true; // Готовимся к тому, что следующее число сотрет старый результат
        }

        // Обработчик клика по кнопке полной очистки (C)
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0"; // Возвращаем дисплей в исходное нулевое состояние
            firstNumber = 0; // Обнуляем сохраненное первое число
            operation = ""; // Сбрасываем выбранную операцию
            startNewNumber = true; // Переводим в состояние ввода нового числа с нуля
        }

        // Обработчик клика по кнопке удаления последнего символа (Backspace / стёрка)
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (startNewNumber || Display.Text == "Ошибка") // Если мы только начали вводить число или на экране ошибка
                return; // Удалять нечего, прерываем выполнение метода

            if (Display.Text.Length > 1) // Если на экране строка длиной больше одного символа
                Display.Text = Display.Text.Substring(0, Display.Text.Length - 1); // Отрезаем последний символ строки
            else // Если осталась всего одна цифра
                Display.Text = "0"; // Заменяем её на стандартный ноль
        }

        // Обработчик клика по кнопке смены знака числа (+/-)
        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            // Пытаемся распарсить текущее значение экрана в число
            if (double.TryParse(Display.Text, culture, out double number))
            {
                // Меняем знак числа на противоположный (инвертируем) и выводим обратно на экран в формате "G"
                Display.Text = (-number).ToString("G", culture);
            }
        }

        // Обработчик клика по кнопке ввода десятичного разделителя (запятая или точка)
        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            string separator = culture.NumberFormat.NumberDecimalSeparator; // Получаем системный разделитель (запятую или точку)

            if (startNewNumber || Display.Text == "Ошибка") // Если мы только начинаем ввод нового числа с дроби
            {
                Display.Text = "0" + separator; // Автоматически подставляем "0," на экран
                startNewNumber = false; // Фиксируем, что ввод числа уже начат
            }
            else if (!Display.Text.Contains(separator)) // Если текущее число еще НЕ содержит в себе разделитель
            {
                Display.Text += separator; // Дописываем разделитель к числу (защита от повторного ввода разделителя, например "5,,5")
            }
        }
    }
}
