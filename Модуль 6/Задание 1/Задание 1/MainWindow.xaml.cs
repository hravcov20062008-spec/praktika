
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Задание_1
{
    public partial class MainWindow : Window
    {
        // Путь к файлу базы данных SQLite
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 6\Задание 1\Задание 1\BD_1.db";

        // Хранит ID выбранного сотрудника.
        // Значение -1 означает, что сотрудник не выбран.
        private int selectedId = -1;

        // Строка подключения к базе данных SQLite.
        // Data Source указывает путь к файлу базы данных.
        private string ConnectionString =>
            $"Data Source={dbPath}";

        // Конструктор главного окна программы
        public MainWindow()
        {
            InitializeComponent(); // Создание элементов интерфейса из XAML

            try
            {
                // Проверяем, существует ли файл базы данных
                if (!File.Exists(dbPath))
                {
                    MessageBox.Show(
                        "Файл базы данных не найден:\n" + dbPath,
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Close(); // Закрываем окно, если база не найдена
                    return;
                }

                InitializeDatabase(); // Проверяем подключение и наличие таблицы
                LoadEmployees();      // Загружаем сотрудников в таблицу интерфейса
            }
            catch (Exception ex)
            {
                ShowError(ex); // Показываем сообщение при возникновении ошибки
            }
        }

        // Проверка подключения к базе данных
        // и существования таблицы Employees
        private void InitializeDatabase()
        {
            // Создаём подключение к SQLite
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open(); // Открываем соединение с базой

            // Запрос проверяет наличие таблицы Employees
            string sql = @"
                SELECT COUNT(*)
                FROM sqlite_master
                WHERE type = 'table'
                  AND name = 'Employees';";

            using var command =
                new SqliteCommand(sql, connection);

            // Выполняем запрос и получаем количество найденных таблиц
            object? result = command.ExecuteScalar();
            long count = Convert.ToInt64(result ?? 0L);

            // Если таблица отсутствует, сообщаем об ошибке
            if (count == 0)
            {
                throw new Exception(
                    "Таблица Employees не найдена в базе данных.");
            }
        }

        // Загрузка списка сотрудников из базы данных
        private void LoadEmployees()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Получаем ID, ФИО, должность и зарплату сотрудников
            // Сначала отображаются записи с наибольшим ID
            string sql = @"
                SELECT Id, Name, Position, Salary
                FROM Employees
                ORDER BY Id DESC;";

            using var command =
                new SqliteCommand(sql, connection);

            // Выполняем запрос для чтения записей
            using var reader = command.ExecuteReader();

            // Переносим полученные данные в таблицу DataTable
            DataTable table = new DataTable();
            table.Load(reader);

            // Отображаем данные в элементе DataGrid
            dgvEmployees.ItemsSource = table.DefaultView;

            // Показываем количество сотрудников в строке состояния
            lblStatus.Text =
                $"Количество сотрудников: {table.Rows.Count}";
        }

        // Проверка данных, введённых пользователем
        // Метод возвращает true, если все данные корректны
        private bool TryGetEmployeeData(
            out string name,
            out string position,
            out double salary)
        {
            // Получаем ФИО и должность, убирая лишние пробелы
            name = txtFullName.Text.Trim();
            position = txtPosition.Text.Trim();
            salary = 0;

            // Проверяем, заполнено ли поле ФИО
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите ФИО сотрудника.");
                return false;
            }

            // Проверяем, заполнено ли поле должности
            if (string.IsNullOrWhiteSpace(position))
            {
                MessageBox.Show("Введите должность сотрудника.");
                return false;
            }

            // Получаем зарплату и заменяем запятую на точку
            string salaryText =
                txtSalary.Text.Trim().Replace(',', '.');

            // Проверяем, что зарплата является числом
            // и не имеет отрицательного значения
            if (!double.TryParse(
                    salaryText,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out salary)
                || salary < 0)
            {
                MessageBox.Show(
                    "Введите корректную зарплату, например 50000 или 50000,50.");
                return false;
            }

            // Все проверки пройдены
            return true;
        }

        // Добавление нового сотрудника в базу данных
        private void btnAdd_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем правильность введённых данных
                if (!TryGetEmployeeData(
                    out string name,
                    out string position,
                    out double salary))
                {
                    return; // Прерываем добавление при ошибке
                }

                using var connection =
                    new SqliteConnection(ConnectionString);

                connection.Open();

                // SQL-запрос добавляет новую запись в таблицу
                string sql = @"
                    INSERT INTO Employees (Name, Position, Salary)
                    VALUES (@name, @position, @salary);";

                using var command =
                    new SqliteCommand(sql, connection);

                // Передаём значения параметрам запроса
                // Это помогает избежать ошибок при обработке текста
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@position", position);
                command.Parameters.AddWithValue("@salary", salary);

                command.ExecuteNonQuery(); // Выполняем добавление записи

                LoadEmployees(); // Обновляем таблицу сотрудников
                ClearFields();   // Очищаем поля ввода

                MessageBox.Show("Сотрудник успешно добавлен.");
            }
            catch (Exception ex)
            {
                ShowError(ex); // Выводим сообщение при ошибке добавления
            }
        }

        // Изменение данных выбранного сотрудника
        private void btnEdit_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, выбран ли сотрудник в таблице
                if (selectedId == -1)
                {
                    MessageBox.Show(
                        "Выберите сотрудника в таблице.");
                    return;
                }

                // Проверяем новые данные перед сохранением
                if (!TryGetEmployeeData(
                    out string name,
                    out string position,
                    out double salary))
                {
                    return;
                }

                using var connection =
                    new SqliteConnection(ConnectionString);

                connection.Open();

                // Обновляем данные сотрудника с указанным ID
                string sql = @"
                    UPDATE Employees
                    SET Name = @name,
                        Position = @position,
                        Salary = @salary
                    WHERE Id = @id;";

                using var command =
                    new SqliteCommand(sql, connection);

                // Передаём новые значения и ID сотрудника
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@position", position);
                command.Parameters.AddWithValue("@salary", salary);
                command.Parameters.AddWithValue("@id", selectedId);

                command.ExecuteNonQuery(); // Сохраняем изменения

                LoadEmployees(); // Обновляем список
                ClearFields();   // Очищаем форму

                MessageBox.Show("Данные сотрудника изменены.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // Удаление выбранного сотрудника
        private void btnDelete_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, выбран ли сотрудник для удаления
                if (selectedId == -1)
                {
                    MessageBox.Show(
                        "Сначала выберите сотрудника в таблице.");
                    return;
                }

                // Запрашиваем подтверждение перед удалением
                MessageBoxResult result = MessageBox.Show(
                    "Удалить выбранного сотрудника?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                // Если пользователь выбрал "Нет", отменяем удаление
                if (result != MessageBoxResult.Yes)
                    return;

                using var connection =
                    new SqliteConnection(ConnectionString);

                connection.Open();

                // Удаляем запись по уникальному ID сотрудника
                string sql =
                    "DELETE FROM Employees WHERE Id = @id;";

                using var command =
                    new SqliteCommand(sql, connection);

                command.Parameters.AddWithValue("@id", selectedId);
                command.ExecuteNonQuery(); // Выполняем удаление

                LoadEmployees(); // Обновляем таблицу
                ClearFields();   // Сбрасываем поля и выбор

                MessageBox.Show("Сотрудник удалён.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // Обработчик выбора строки в таблице сотрудников
        private void dgvEmployees_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка с данными
            if (dgvEmployees.SelectedItem is not DataRowView row)
                return;

            // Сохраняем ID выбранного сотрудника
            selectedId = Convert.ToInt32(row["Id"]);

            // Переносим данные выбранной строки в поля ввода
            // Это позволяет просмотреть и изменить информацию
            txtFullName.Text = row["Name"]?.ToString() ?? "";
            txtPosition.Text = row["Position"]?.ToString() ?? "";

            // Отображаем зарплату в поле ввода
            txtSalary.Text =
                Convert.ToDouble(row["Salary"])
                .ToString(CultureInfo.InvariantCulture);
        }

        // Обработчик кнопки очистки полей
        private void btnClear_Click(
            object sender, RoutedEventArgs e)
        {
            ClearFields(); // Вызываем общий метод очистки
        }

        // Очистка полей ввода и сброс выбранного сотрудника
        private void ClearFields()
        {
            selectedId = -1; // Сбрасываем ID выбранной записи

            // Очищаем текстовые поля
            txtFullName.Clear();
            txtPosition.Clear();
            txtSalary.Clear();

            // Снимаем выделение со строки таблицы
            dgvEmployees.SelectedItem = null;
        }

        // Универсальный метод отображения ошибок
        // Используется при ошибках подключения и работы с БД
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
