
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Задание_2
{
    public partial class MainWindow : Window
    {
        // Путь к файлу базы данных SQLite
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 6\Задание 2\DB_2.db";

        // ID выбранной задачи.
        // Значение -1 означает, что задача не выбрана
        private int selectedId = -1;

        // Строка подключения к базе данных
        private string ConnectionString =>
            $"Data Source={dbPath}";

        // Конструктор главного окна
        public MainWindow()
        {
            InitializeComponent(); // Загружаем элементы интерфейса из XAML

            try
            {
                InitializeDatabase(); // Проверяем наличие базы и таблицы
                LoadTasks();           // Загружаем список задач
            }
            catch (Exception ex)
            {
                ShowError(ex); // Показываем ошибку, если запуск не удался
            }
        }

        // Проверка существования базы данных
        // и таблицы Tasks
        private void InitializeDatabase()
        {
            // Проверяем, существует ли файл базы данных
            if (!File.Exists(dbPath))
            {
                throw new FileNotFoundException(
                    "База данных не найдена. Проверь путь:\n" + dbPath);
            }

            // Создаём подключение к SQLite
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open(); // Открываем соединение

            // Запрос проверяет наличие таблицы Tasks
            string sql = @"
                SELECT COUNT(*)
                FROM sqlite_master
                WHERE type = 'table'
                  AND name = 'Tasks';";

            using var command =
                new SqliteCommand(sql, connection);

            // Получаем количество найденных таблиц с таким именем
            long count = Convert.ToInt64(
                command.ExecuteScalar() ?? 0L);

            // Если таблицы нет, выводим сообщение об ошибке
            if (count == 0)
            {
                throw new Exception(
                    "В базе DB_2.db нет таблицы Tasks. " +
                    "Создай её в DB Browser for SQLite.");
            }
        }

        // Загрузка задач из базы данных
        private void LoadTasks()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Получаем все основные поля задач
            // Сначала отображаются задачи с наибольшим ID
            string sql = @"
                SELECT Id, Title, Description, DueDate, Status
                FROM Tasks
                ORDER BY Id DESC;";

            using var command =
                new SqliteCommand(sql, connection);

            // Выполняем запрос для чтения записей
            using var reader = command.ExecuteReader();

            // Сохраняем полученные данные в таблицу DataTable
            DataTable table = new DataTable();
            table.Load(reader);

            // Отображаем данные в таблице интерфейса
            dgvTasks.ItemsSource = table.DefaultView;

            // Обновляем строку состояния с количеством задач
            lblStatus.Text =
                $"Количество задач: {table.Rows.Count}";
        }

        // Проверка данных, введённых пользователем
        // Возвращает true, если данные корректны
        private bool TryGetTaskData(
            out string title,
            out string description,
            out string dueDate,
            out string status)
        {
            // Получаем название и описание задачи
            // Trim удаляет пробелы в начале и конце текста
            title = txtTitle.Text.Trim();
            description = txtDescription.Text.Trim();

            // Получаем выбранную дату в формате ГГГГ-ММ-ДД
            // Если дата не выбрана, сохраняем пустую строку
            dueDate = dpDueDate.SelectedDate.HasValue
                ? dpDueDate.SelectedDate.Value.ToString("yyyy-MM-dd")
                : "";

            // Получаем статус из выпадающего списка
            // Если статус не выбран, используем значение "Новая"
            status =
                (cmbStatus.SelectedItem as ComboBoxItem)
                ?.Content?.ToString() ?? "Новая";

            // Проверяем, заполнено ли название задачи
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название задачи.");
                return false;
            }

            // Все проверки пройдены
            return true;
        }

        // Добавление новой задачи
        private void btnAdd_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем введённые данные
                if (!TryGetTaskData(
                    out string title,
                    out string description,
                    out string dueDate,
                    out string status))
                    return; // Прерываем добавление при ошибке

                using var connection =
                    new SqliteConnection(ConnectionString);

                connection.Open();

                // SQL-запрос для добавления задачи
                string sql = @"
                    INSERT INTO Tasks
                        (Title, Description, DueDate, Status)
                    VALUES
                        (@title, @description, @dueDate, @status);";

                using var command =
                    new SqliteCommand(sql, connection);

                // Передаём значения параметрам SQL-запроса
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@dueDate", dueDate);
                command.Parameters.AddWithValue("@status", status);

                command.ExecuteNonQuery(); // Сохраняем задачу в базе

                LoadTasks();  // Обновляем таблицу задач
                ClearFields(); // Очищаем поля ввода

                MessageBox.Show("Задача добавлена.");
            }
            catch (Exception ex)
            {
                ShowError(ex); // Показываем ошибку добавления
            }
        }

        // Редактирование выбранной задачи
        private void btnEdit_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, выбрана ли задача для изменения
                if (selectedId == -1)
                {
                    MessageBox.Show(
                        "Сначала выбери задачу в таблице.");
                    return;
                }

                // Проверяем новые значения перед сохранением
                if (!TryGetTaskData(
                    out string title,
                    out string description,
                    out string dueDate,
                    out string status))
                    return;

                using var connection =
                    new SqliteConnection(ConnectionString);

                connection.Open();

                // Обновляем данные задачи с выбранным ID
                string sql = @"
                    UPDATE Tasks
                    SET Title = @title,
                        Description = @description,
                        DueDate = @dueDate,
                        Status = @status
                    WHERE Id = @id;";

                using var command =
                    new SqliteCommand(sql, connection);

                // Передаём новые значения и ID изменяемой задачи
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@dueDate", dueDate);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@id", selectedId);

                // Выполняем запрос и получаем количество изменённых строк
                int affected = command.ExecuteNonQuery();

                LoadTasks();   // Обновляем список задач
                ClearFields(); // Сбрасываем поля ввода

                // Сообщаем результат операции
                MessageBox.Show(
                    affected > 0
                        ? "Задача изменена."
                        : "Задача не найдена.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // Удаление выбранной задачи
        private void btnDelete_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, выбрана ли задача
                if (selectedId == -1)
                {
                    MessageBox.Show(
                        "Сначала выбери задачу в таблице.");
                    return;
                }

                // Спрашиваем подтверждение удаления
                MessageBoxResult result = MessageBox.Show(
                    "Удалить выбранную задачу?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                // Если пользователь отказался, отменяем операцию
                if (result != MessageBoxResult.Yes)
                    return;

                using var connection =
                    new SqliteConnection(ConnectionString);

                connection.Open();

                // Удаляем задачу по её уникальному ID
                string sql =
                    "DELETE FROM Tasks WHERE Id = @id;";

                using var command =
                    new SqliteCommand(sql, connection);

                command.Parameters.AddWithValue("@id", selectedId);

                // Выполняем удаление и проверяем результат
                int affected = command.ExecuteNonQuery();

                LoadTasks();   // Обновляем таблицу
                ClearFields(); // Очищаем форму

                MessageBox.Show(
                    affected > 0
                        ? "Задача удалена."
                        : "Задача не найдена.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // Обработчик выбора задачи в таблице
        private void dgvTasks_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка с данными
            if (dgvTasks.SelectedItem is not DataRowView row)
                return;

            // Запоминаем ID выбранной задачи
            selectedId = Convert.ToInt32(row["Id"]);

            // Заполняем поля формы данными выбранной задачи
            txtTitle.Text =
                row["Title"]?.ToString() ?? "";

            txtDescription.Text =
                row["Description"]?.ToString() ?? "";

            // Получаем дату задачи из таблицы
            string dateText =
                row["DueDate"]?.ToString() ?? "";

            // Пытаемся преобразовать дату из формата ГГГГ-ММ-ДД
            if (DateTime.TryParseExact(
                dateText,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dueDate))
            {
                // Если преобразование успешно, выбираем дату
                dpDueDate.SelectedDate = dueDate;
            }
            // Если формат отличается, пробуем обычное преобразование
            else if (DateTime.TryParse(dateText, out dueDate))
            {
                dpDueDate.SelectedDate = dueDate;
            }
            else
            {
                // Если дата некорректна или отсутствует,
                // очищаем выбор даты
                dpDueDate.SelectedDate = null;
            }

            // Получаем статус выбранной задачи
            string status =
                row["Status"]?.ToString() ?? "Новая";

            // Ищем соответствующий статус в выпадающем списке
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content?.ToString() == status)
                {
                    // Устанавливаем найденный статус
                    cmbStatus.SelectedItem = item;
                    break; // Прекращаем поиск
                }
            }
        }

        // Обработчик кнопки очистки полей
        private void btnClear_Click(
            object sender, RoutedEventArgs e)
        {
            ClearFields(); // Вызываем общий метод очистки
        }

        // Очистка полей формы и сброс выбранной задачи
        private void ClearFields()
        {
            selectedId = -1; // Сбрасываем ID выбранной задачи

            // Очищаем название и описание
            txtTitle.Clear();
            txtDescription.Clear();

            // Сбрасываем выбранную дату
            dpDueDate.SelectedDate = null;

            // Устанавливаем первый статус в списке
            cmbStatus.SelectedIndex = 0;

            // Снимаем выделение с таблицы
            dgvTasks.SelectedItem = null;
        }

        // Универсальный метод отображения ошибок
        private void ShowError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,             // Текст ошибки
                "Ошибка",                // Заголовок окна
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
