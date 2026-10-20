
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Задание_4
{
    public partial class MainWindow : Window
    {
        // Путь к файлу базы данных SQLite
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 6\Задание 4\DB_4.db";

        // Строка подключения к базе данных
        private string ConnectionString =>
            $"Data Source={dbPath}";

        // Конструктор главного окна программы
        public MainWindow()
        {
            InitializeComponent(); // Загружаем интерфейс из XAML

            // По умолчанию устанавливаем текущую дату
            dpDate.SelectedDate = DateTime.Today;

            LoadFinance(); // Загружаем операции и рассчитываем баланс
        }

        // Создание базы данных и таблицы финансовых операций
        private void InitializeDatabase()
        {
            // Получаем путь к папке, где должна находиться база
            string folder = Path.GetDirectoryName(dbPath);

            // Если папки нет, создаём её
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Создаём подключение к SQLite
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Создаём SQL-команду
            using var command = connection.CreateCommand();

            // Создаём таблицу Finance, если её ещё нет
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Finance (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Date TEXT NOT NULL,
                    Description TEXT
                );";

            command.ExecuteNonQuery(); // Выполняем создание таблицы
        }

        // Загрузка финансовых операций из базы данных
        private void LoadFinance()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Получаем все финансовые записи.
            // Сначала отображаются операции с более поздней датой,
            // а при одинаковой дате — записи с большим ID.
            command.CommandText = @"
                SELECT Id, Type, Category, Amount, Date, Description
                FROM Finance
                ORDER BY Date DESC, Id DESC;";

            // Выполняем запрос на чтение данных
            using var reader = command.ExecuteReader();

            // Переносим результат запроса в таблицу данных
            DataTable table = new DataTable();
            table.Load(reader);

            // Отображаем записи в таблице интерфейса
            dgvFinance.ItemsSource = table.DefaultView;

            // Обновляем итоговые суммы после загрузки записей
            CalculateSummary();
        }

        // Расчёт общей суммы доходов, расходов и баланса
        private void CalculateSummary()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Суммируем доходы отдельно от расходов.
            // CASE проверяет тип каждой операции.
            // COALESCE возвращает 0, если подходящих записей нет.
            command.CommandText = @"
                SELECT
                    COALESCE(SUM(CASE
                        WHEN Type = 'Доход' THEN Amount
                        ELSE 0 END), 0),
                    COALESCE(SUM(CASE
                        WHEN Type = 'Расход' THEN Amount
                        ELSE 0 END), 0)
                FROM Finance;";

            // Получаем результаты расчёта
            using var reader = command.ExecuteReader();
            reader.Read();

            // Первая колонка содержит общую сумму доходов
            decimal income = Convert.ToDecimal(reader.GetDouble(0));

            // Вторая колонка содержит общую сумму расходов
            decimal expense = Convert.ToDecimal(reader.GetDouble(1));

            // Баланс рассчитывается как доходы минус расходы
            decimal balance = income - expense;

            // Отображаем итоги в строке состояния
            lblSummary.Text =
                $"Доходы: {income:F2} | " +
                $"Расходы: {expense:F2} | " +
                $"Баланс: {balance:F2}";
        }

        // Получение ID выбранной финансовой операции
        private int SelectedId()
        {
            // Если строка выбрана, возвращаем ID записи
            if (dgvFinance.SelectedItem is DataRowView row)
                return Convert.ToInt32(row["Id"]);

            // Значение -1 означает отсутствие выбранной записи
            return -1;
        }

        // Проверка данных перед добавлением или редактированием
        private bool ValidateFields(
            out string type,
            out decimal amount,
            out DateTime date)
        {
            // Устанавливаем начальные значения
            type = "";
            amount = 0;
            date = DateTime.Today;

            // Проверяем, выбран ли тип операции
            if (cmbType.SelectedItem is not ComboBoxItem item)
            {
                MessageBox.Show("Выберите тип операции.");
                return false;
            }

            // Получаем тип: доход или расход
            type = item.Content.ToString();

            // Проверяем, заполнена ли категория операции
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Введите категорию операции.");
                return false;
            }

            // Получаем сумму и заменяем запятую на точку
            // для корректного преобразования числа
            string amountText = txtAmount.Text.Trim()
                .Replace(',', '.');

            // Проверяем, что сумма является числом больше нуля
            if (!decimal.TryParse(
                    amountText,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму больше нуля.");
                return false;
            }

            // Проверяем, выбрана ли дата операции
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату операции.");
                return false;
            }

            // Сохраняем выбранную дату
            date = dpDate.SelectedDate.Value;

            // Все проверки пройдены
            return true;
        }

        // Добавление новой финансовой операции
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем корректность введённых данных
            if (!ValidateFields(
                    out string type,
                    out decimal amount,
                    out DateTime date))
                return; // Прерываем добавление при ошибке

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // SQL-запрос добавляет запись в таблицу Finance
            command.CommandText = @"
                INSERT INTO Finance
                    (Type, Category, Amount, Date, Description)
                VALUES
                    (@type, @category, @amount, @date, @description);";

            // Передаём значения параметрам запроса
            command.Parameters.AddWithValue("@type", type);

            command.Parameters.AddWithValue(
                "@category", txtCategory.Text.Trim());

            // SQLite получает сумму как числовое значение
            command.Parameters.AddWithValue("@amount", (double)amount);

            // Сохраняем дату в формате ГГГГ-ММ-ДД
            command.Parameters.AddWithValue(
                "@date", date.ToString("yyyy-MM-dd"));

            // Сохраняем описание операции
            command.Parameters.AddWithValue(
                "@description", txtDescription.Text.Trim());

            command.ExecuteNonQuery(); // Записываем данные в базу

            LoadFinance();  // Обновляем таблицу и итоговые суммы
            ClearFields();  // Очищаем поля формы
        }

        // Редактирование выбранной финансовой операции
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID выбранной записи
            int id = SelectedId();

            // Проверяем, выбрана ли запись для редактирования
            if (id == -1)
            {
                MessageBox.Show("Выберите запись для изменения.");
                return;
            }

            // Проверяем новые значения полей
            if (!ValidateFields(
                    out string type,
                    out decimal amount,
                    out DateTime date))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Обновляем данные существующей записи по её ID
            command.CommandText = @"
                UPDATE Finance
                SET Type = @type,
                    Category = @category,
                    Amount = @amount,
                    Date = @date,
                    Description = @description
                WHERE Id = @id;";

            // Передаём новые значения операции
            command.Parameters.AddWithValue("@type", type);

            command.Parameters.AddWithValue(
                "@category", txtCategory.Text.Trim());

            command.Parameters.AddWithValue("@amount", (double)amount);

            command.Parameters.AddWithValue(
                "@date", date.ToString("yyyy-MM-dd"));

            command.Parameters.AddWithValue(
                "@description", txtDescription.Text.Trim());

            // Указываем, какую именно запись нужно изменить
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery(); // Сохраняем изменения

            LoadFinance();  // Обновляем список и баланс
            ClearFields();  // Очищаем поля ввода
        }

        // Удаление выбранной финансовой операции
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID выбранной записи
            int id = SelectedId();

            // Проверяем, выбрана ли операция
            if (id == -1)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            // Запрашиваем подтверждение удаления
            if (MessageBox.Show(
                    "Удалить выбранную запись?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return; // Отменяем удаление, если пользователь отказался

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Удаляем запись по уникальному ID
            command.CommandText =
                "DELETE FROM Finance WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery(); // Выполняем удаление

            LoadFinance();  // Обновляем таблицу и пересчитываем баланс
            ClearFields();  // Очищаем поля формы
        }

        // Обработчик выбора записи в таблице
        private void dgvFinance_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка с данными
            if (dgvFinance.SelectedItem is not DataRowView row)
                return;

            // Устанавливаем тип операции в выпадающем списке
            // Индекс 0 соответствует доходу, индекс 1 — расходу
            cmbType.SelectedIndex =
                row["Type"].ToString() == "Доход" ? 0 : 1;

            // Заполняем поле категории
            txtCategory.Text = row["Category"].ToString();

            // Отображаем сумму с двумя знаками после запятой
            txtAmount.Text = Convert.ToDecimal(row["Amount"])
                .ToString("0.00", CultureInfo.InvariantCulture);

            // Преобразуем сохранённую дату и отображаем её
            if (DateTime.TryParse(
                    row["Date"].ToString(), out DateTime date))
            {
                dpDate.SelectedDate = date;
            }

            // Если описание отсутствует, оставляем поле пустым
            txtDescription.Text =
                row["Description"] == DBNull.Value
                    ? ""
                    : row["Description"].ToString();
        }

        // Обработчик кнопки очистки полей
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields(); // Вызываем общий метод очистки
        }

        // Очистка полей ввода и сброс выделения
        private void ClearFields()
        {
            // Снимаем выделение с выбранной записи
            dgvFinance.SelectedItem = null;

            // По умолчанию выбираем тип операции "Доход"
            cmbType.SelectedIndex = 0;

            // Очищаем категорию, сумму и описание
            txtCategory.Clear();
            txtAmount.Clear();
            txtDescription.Clear();

            // Устанавливаем текущую дату
            dpDate.SelectedDate = DateTime.Today;
        }
    }
}
