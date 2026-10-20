
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Задание_3
{
    public partial class MainWindow : Window
    {
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 6\Задание 3\DB_3.db";

        private string ConnectionString =>
            $"Data Source={dbPath}";

        public MainWindow()
        {
            InitializeComponent();

            InitializeDatabase();
            LoadBooks();
        }

        private void InitializeDatabase()
        {
            string folder = System.IO.Path.GetDirectoryName(dbPath);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                PRAGMA foreign_keys = ON;

                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    Year INTEGER,
                    TotalCopies INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS Rentals (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookId INTEGER NOT NULL,
                    RentDate TEXT NOT NULL,
                    ReturnDate TEXT,
                    FOREIGN KEY (BookId) REFERENCES Books(Id)
                );";

            command.ExecuteNonQuery();
        }

        private void LoadBooks()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Year,
                    b.TotalCopies,
                    COUNT(r.Id) AS RentedCopies,
                    b.TotalCopies - COUNT(r.Id) AS AvailableCopies
                FROM Books b
                LEFT JOIN Rentals r
                    ON b.Id = r.BookId
                    AND r.ReturnDate IS NULL
                GROUP BY
                    b.Id, b.Title, b.Author, b.Year, b.TotalCopies
                ORDER BY b.Title;";

            using var reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            dgvBooks.ItemsSource = table.DefaultView;
        }

        private int SelectedId()
        {
            if (dgvBooks.SelectedItem is DataRowView row)
                return Convert.ToInt32(row["Id"]);

            return -1;
        }

        private bool ValidateBook(out int copies, out object year)
        {
            copies = 0;
            year = DBNull.Value;

            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Введите название книги и автора.");
                return false;
            }

            if (!int.TryParse(txtCopies.Text, out copies) || copies < 1)
            {
                MessageBox.Show(
                    "Количество экземпляров должно быть не меньше 1.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtYear.Text))
            {
                if (!int.TryParse(txtYear.Text, out int parsedYear))
                {
                    MessageBox.Show("Введите корректный год издания.");
                    return false;
                }

                year = parsedYear;
            }

            return true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateBook(out int copies, out object year))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Books (Title, Author, Year, TotalCopies)
                VALUES (@title, @author, @year, @copies);";

            command.Parameters.AddWithValue(
                "@title", txtTitle.Text.Trim());

            command.Parameters.AddWithValue(
                "@author", txtAuthor.Text.Trim());

            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@copies", copies);

            command.ExecuteNonQuery();

            LoadBooks();
            ClearFields();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            int id = SelectedId();

            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для изменения.");
                return;
            }

            if (!ValidateBook(out int copies, out object year))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var check = connection.CreateCommand();

            check.CommandText = @"
                SELECT COUNT(*)
                FROM Rentals
                WHERE BookId = @id AND ReturnDate IS NULL;";

            check.Parameters.AddWithValue("@id", id);

            int rented = Convert.ToInt32(check.ExecuteScalar());

            if (copies < rented)
            {
                MessageBox.Show(
                    "Количество экземпляров не может быть меньше числа выданных книг.");
                return;
            }

            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Books
                SET Title = @title,
                    Author = @author,
                    Year = @year,
                    TotalCopies = @copies
                WHERE Id = @id;";

            command.Parameters.AddWithValue(
                "@title", txtTitle.Text.Trim());

            command.Parameters.AddWithValue(
                "@author", txtAuthor.Text.Trim());

            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@copies", copies);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();

            LoadBooks();
            ClearFields();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = SelectedId();

            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для удаления.");
                return;
            }

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var check = connection.CreateCommand();

            check.CommandText = @"
                SELECT COUNT(*)
                FROM Rentals
                WHERE BookId = @id AND ReturnDate IS NULL;";

            check.Parameters.AddWithValue("@id", id);

            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
            {
                MessageBox.Show(
                    "Сначала верните все выданные экземпляры книги.");
                return;
            }

            if (MessageBox.Show(
                "Удалить выбранную книгу?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            using var command = connection.CreateCommand();

            command.CommandText =
                "DELETE FROM Books WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();

            LoadBooks();
            ClearFields();
        }

        private void btnRent_Click(object sender, RoutedEventArgs e)
        {
            int id = SelectedId();

            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для выдачи.");
                return;
            }

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var check = connection.CreateCommand();

            check.CommandText = @"
                SELECT b.TotalCopies - COUNT(r.Id)
                FROM Books b
                LEFT JOIN Rentals r
                    ON b.Id = r.BookId
                    AND r.ReturnDate IS NULL
                WHERE b.Id = @id
                GROUP BY b.Id, b.TotalCopies;";

            check.Parameters.AddWithValue("@id", id);

            object result = check.ExecuteScalar();

            if (result == null || result == DBNull.Value ||
                Convert.ToInt32(result) <= 0)
            {
                MessageBox.Show("Нет доступных экземпляров этой книги.");
                return;
            }

            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Rentals (BookId, RentDate, ReturnDate)
                VALUES (@id, @date, NULL);";

            command.Parameters.AddWithValue("@id", id);

            command.Parameters.AddWithValue(
                "@date", DateTime.Now.ToString("yyyy-MM-dd"));

            command.ExecuteNonQuery();

            LoadBooks();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            int id = SelectedId();

            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для возврата.");
                return;
            }

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Rentals
                SET ReturnDate = @date
                WHERE Id = (
                    SELECT Id
                    FROM Rentals
                    WHERE BookId = @id AND ReturnDate IS NULL
                    ORDER BY Id DESC
                    LIMIT 1
                );";

            command.Parameters.AddWithValue(
                "@date", DateTime.Now.ToString("yyyy-MM-dd"));

            command.Parameters.AddWithValue("@id", id);

            int affected = command.ExecuteNonQuery();

            if (affected == 0)
                MessageBox.Show("У этой книги нет выданных экземпляров.");

            LoadBooks();
        }

        private void dgvBooks_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (dgvBooks.SelectedItem is not DataRowView row)
                return;

            txtTitle.Text = row["Title"]?.ToString() ?? "";
            txtAuthor.Text = row["Author"]?.ToString() ?? "";

            txtYear.Text = row["Year"] == DBNull.Value
                ? ""
                : row["Year"].ToString();

            txtCopies.Text = row["TotalCopies"].ToString();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            dgvBooks.SelectedItem = null;

            txtTitle.Clear();
            txtAuthor.Clear();
            txtYear.Clear();
            txtCopies.Text = "1";
        }
    }
}
