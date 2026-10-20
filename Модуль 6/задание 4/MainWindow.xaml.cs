
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
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 6\Задание 4\DB_4.db";

        private string ConnectionString =>
            $"Data Source={dbPath}";

        public MainWindow()
        {
            InitializeComponent();

            InitializeDatabase();

            dpDate.SelectedDate = DateTime.Today;

            LoadFinance();
        }

        private void InitializeDatabase()
        {
            string folder = Path.GetDirectoryName(dbPath);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Finance (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Date TEXT NOT NULL,
                    Description TEXT
                );";

            command.ExecuteNonQuery();
        }

        private void LoadFinance()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT Id, Type, Category, Amount, Date, Description
                FROM Finance
                ORDER BY Date DESC, Id DESC;";

            using var reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            dgvFinance.ItemsSource = table.DefaultView;

            CalculateSummary();
        }

        private void CalculateSummary()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    COALESCE(SUM(CASE
                        WHEN Type = 'Доход' THEN Amount
                        ELSE 0 END), 0),
                    COALESCE(SUM(CASE
                        WHEN Type = 'Расход' THEN Amount
                        ELSE 0 END), 0)
                FROM Finance;";

            using var reader = command.ExecuteReader();
            reader.Read();

            decimal income = Convert.ToDecimal(reader.GetDouble(0));
            decimal expense = Convert.ToDecimal(reader.GetDouble(1));
            decimal balance = income - expense;

            lblSummary.Text =
                $"Доходы: {income:F2} | " +
                $"Расходы: {expense:F2} | " +
                $"Баланс: {balance:F2}";
        }

        private int SelectedId()
        {
            if (dgvFinance.SelectedItem is DataRowView row)
                return Convert.ToInt32(row["Id"]);

            return -1;
        }

        private bool ValidateFields(
            out string type,
            out decimal amount,
            out DateTime date)
        {
            type = "";
            amount = 0;
            date = DateTime.Today;

            if (cmbType.SelectedItem is not ComboBoxItem item)
            {
                MessageBox.Show("Выберите тип операции.");
                return false;
            }

            type = item.Content.ToString();

            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Введите категорию операции.");
                return false;
            }

            string amountText = txtAmount.Text.Trim()
                .Replace(',', '.');

            if (!decimal.TryParse(
                    amountText,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму больше нуля.");
                return false;
            }

            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату операции.");
                return false;
            }

            date = dpDate.SelectedDate.Value;

            return true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields(
                    out string type,
                    out decimal amount,
                    out DateTime date))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Finance
                    (Type, Category, Amount, Date, Description)
                VALUES
                    (@type, @category, @amount, @date, @description);";

            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
            command.Parameters.AddWithValue("@amount", (double)amount);
            command.Parameters.AddWithValue(
                "@date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue(
                "@description", txtDescription.Text.Trim());

            command.ExecuteNonQuery();

            LoadFinance();
            ClearFields();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            int id = SelectedId();

            if (id == -1)
            {
                MessageBox.Show("Выберите запись для изменения.");
                return;
            }

            if (!ValidateFields(
                    out string type,
                    out decimal amount,
                    out DateTime date))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Finance
                SET Type = @type,
                    Category = @category,
                    Amount = @amount,
                    Date = @date,
                    Description = @description
                WHERE Id = @id;";

            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
            command.Parameters.AddWithValue("@amount", (double)amount);
            command.Parameters.AddWithValue(
                "@date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue(
                "@description", txtDescription.Text.Trim());
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();

            LoadFinance();
            ClearFields();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = SelectedId();

            if (id == -1)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            if (MessageBox.Show(
                    "Удалить выбранную запись?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText =
                "DELETE FROM Finance WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();

            LoadFinance();
            ClearFields();
        }

        private void dgvFinance_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (dgvFinance.SelectedItem is not DataRowView row)
                return;

            cmbType.SelectedIndex =
                row["Type"].ToString() == "Доход" ? 0 : 1;

            txtCategory.Text = row["Category"].ToString();

            txtAmount.Text = Convert.ToDecimal(row["Amount"])
                .ToString("0.00", CultureInfo.InvariantCulture);

            if (DateTime.TryParse(
                    row["Date"].ToString(), out DateTime date))
            {
                dpDate.SelectedDate = date;
            }

            txtDescription.Text =
                row["Description"] == DBNull.Value
                    ? ""
                    : row["Description"].ToString();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            dgvFinance.SelectedItem = null;

            cmbType.SelectedIndex = 0;
            txtCategory.Clear();
            txtAmount.Clear();
            txtDescription.Clear();
            dpDate.SelectedDate = DateTime.Today;
        }
    }
}
