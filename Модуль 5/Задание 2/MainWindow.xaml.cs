using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Задание_2
{
    public partial class MainWindow : Window
    {
        private string currentFile = null; // Путь к текущему открытому файлу (null, если файл новый)

        public MainWindow()
        {
            InitializeComponent(); // Инициализация интерфейса текстового редактора из XAML
        }

        // Обработчик клика по кнопке "Создать" (новый документ)
        private void New_Click(object sender, RoutedEventArgs e)
        {
            Editor.Clear(); // Очищаем текстовое поле редактора
            currentFile = null; // Сбрасываем путь к файлу, так как документ еще не сохранен
            Title = "Новый документ — Текстовый редактор"; // Меняем заголовок окна
            StatusText.Text = "Создан новый документ"; // Выводим статусное сообщение в нижнюю панель
        }

        // Обработчик клика по кнопке "Открыть"
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и настраиваем диалоговое окно открытия файла
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Открыть файл", // Заголовок диалогового окна
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" // Ограничение по типам файлов
            };

            // Если пользователь выбрал файл и нажал "Открыть"
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Считываем весь текст из выбранного файла в кодировке UTF-8 и записываем в редактор
                    Editor.Text = File.ReadAllText(dialog.FileName, Encoding.UTF8);

                    currentFile = dialog.FileName; // Запоминаем путь к открытому файлу
                    Title = Path.GetFileName(currentFile) + " — Текстовый редактор"; // Выводим имя файла в заголовок окна
                    StatusText.Text = "Файл открыт"; // Обновляем текст в строке состояния
                }
                catch (Exception ex) // Если произошла ошибка (например, файл занят или нет прав доступа)
                {
                    MessageBox.Show("Ошибка открытия файла:\n" + ex.Message); // Показываем окно с ошибкой
                }
            }
        }

        // Обработчик клика по кнопке "Сохранить"
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Если файл еще ни разу не сохранялся (путь пустой)
            if (string.IsNullOrEmpty(currentFile))
            {
                SaveAs_Click(sender, e); // Перенаправляем в метод "Сохранить как...", чтобы пользователь выбрал место
                return; // Прерываем выполнение текущего метода
            }

            SaveToFile(currentFile); // Если путь есть, просто перезаписываем этот файл
        }

        // Обработчик клика по кнопке "Сохранить как..."
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и настраиваем диалоговое окно сохранения файла
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Сохранить файл", // Заголовок окна
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", // Доступные расширения
                DefaultExt = ".txt", // Расширение по умолчанию, если пользователь его не указал
                AddExtension = true, // Автоматически добавлять расширение к имени файла
                FileName = "Документ.txt" // Имя файла, предлагаемое по умолчанию
            };

            // Если пользователь выбрал место, ввел имя и нажал "Сохранить"
            if (dialog.ShowDialog() == true)
            {
                SaveToFile(dialog.FileName); // Вызываем метод записи данных в выбранный файл
            }
        }

        // Вспомогательный метод для непосредственной записи текста в файл на диске
        private void SaveToFile(string fileName)
        {
            try
            {
                // Записываем текст из редактора в файл, добавляя маркер UTF-8 (BOM) для корректного чтения в Блокноте
                File.WriteAllText(fileName, Editor.Text, new UTF8Encoding(true));

                currentFile = fileName; // Обновляем путь к текущему файлу
                Title = Path.GetFileName(currentFile) + " — Текстовый редактор"; // Обновляем имя файла в заголовке окна
                StatusText.Text = "Файл сохранён"; // Обновляем строку состояния

                // Показываем всплывающее окно об успешном завершении операции
                MessageBox.Show(
                    "Документ успешно сохранён!",
                    "Сохранение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex) // Если при записи на диск произошел сбой
            {
                MessageBox.Show("Ошибка сохранения файла:\n" + ex.Message); // Выводим сообщение об ошибке
            }
        }

        // Обработчик клика по кнопке "Выход"
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Закрываем главное окно приложения (что приводит к завершению программы)
        }
    }
}
