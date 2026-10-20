
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
        // Путь к файлу базы данных SQLite
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 6\Задание 3\DB_3.db";

        // Строка подключения к базе данных
        private string ConnectionString =>
            $"Data Source={dbPath}";

        // Конструктор главного окна программы
        public MainWindow()
        {
            InitializeComponent(); // Загружаем интерфейс из XAML

            InitializeDatabase(); // Создаём базу и таблицы, если их нет
            LoadBooks();          // Загружаем список книг
        }

        // Создание базы данных и необходимых таблиц
        private void InitializeDatabase()
        {
            // Получаем путь к папке, в которой должна находиться база
            string folder = System.IO.Path.GetDirectoryName(dbPath);

            // Если папки нет, создаём её
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Открываем подключение к SQLite
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Создаём SQL-команду для настройки базы данных
            using var command = connection.CreateCommand();

            // Включаем проверку внешних ключей.
            // Создаём таблицы Books и Rentals, если их ещё нет.
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

            command.ExecuteNonQuery(); // Выполняем создание таблиц
        }

        // Загрузка книг и расчёт доступных экземпляров
        private void LoadBooks()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Получаем информацию о книгах.
            // COUNT подсчитывает количество выданных экземпляров.
            // LEFT JOIN позволяет показывать и книги без выдач.
            // Возвращённые книги не учитываются как выданные.
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

            // Переносим результат SQL-запроса в таблицу данных
            DataTable table = new DataTable();
            table.Load(reader);

            // Отображаем список книг в DataGrid
            dgvBooks.ItemsSource = table.DefaultView;
        }

        // Получение ID выбранной книги
        private int SelectedId()
        {
            // Если пользователь выбрал строку, возвращаем её ID
            if (dgvBooks.SelectedItem is DataRowView row)
                return Convert.ToInt32(row["Id"]);

            // -1 означает, что книга не выбрана
            return -1;
        }

        // Проверка данных книги перед добавлением или изменением
        private bool ValidateBook(out int copies, out object year)
        {
            // Начальные значения для количества и года издания
            copies = 0;
            year = DBNull.Value;

            // Название книги и автор должны быть заполнены
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Введите название книги и автора.");
                return false;
            }

            // Проверяем, что количество экземпляров — целое число
            // и оно не меньше одного
            if (!int.TryParse(txtCopies.Text, out copies) || copies < 1)
            {
                MessageBox.Show(
                    "Количество экземпляров должно быть не меньше 1.");
                return false;
            }

            // Год издания можно оставить пустым.
            // Если он введён, проверяем его формат.
            if (!string.IsNullOrWhiteSpace(txtYear.Text))
            {
                if (!int.TryParse(txtYear.Text, out int parsedYear))
                {
                    MessageBox.Show("Введите корректный год издания.");
                    return false;
                }

                // Сохраняем введённый год
                year = parsedYear;
            }

            // Все проверки пройдены
            return true;
        }

        // Добавление новой книги
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем поля перед добавлением
            if (!ValidateBook(out int copies, out object year))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Добавляем книгу в таблицу Books
            command.CommandText = @"
                INSERT INTO Books (Title, Author, Year, TotalCopies)
                VALUES (@title, @author, @year, @copies);";

            // Передаём введённые данные через параметры запроса
            command.Parameters.AddWithValue(
                "@title", txtTitle.Text.Trim());

            command.Parameters.AddWithValue(
                "@author", txtAuthor.Text.Trim());

            command.Parameters.AddWithValue("@year", year);
            command.Parameters.AddWithValue("@copies", copies);

            command.ExecuteNonQuery(); // Сохраняем книгу в базе

            LoadBooks();  // Обновляем список книг
            ClearFields(); // Очищаем поля ввода
        }

        // Редактирование выбранной книги
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID выбранной книги
            int id = SelectedId();

            // Проверяем, выбрана ли книга
            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для изменения.");
                return;
            }

            // Проверяем корректность новых данных
            if (!ValidateBook(out int copies, out object year))
                return;

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Считаем количество экземпляров этой книги,
            // которые сейчас находятся у читателей
            using var check = connection.CreateCommand();

            check.CommandText = @"
                SELECT COUNT(*)
                FROM Rentals
                WHERE BookId = @id AND ReturnDate IS NULL;";

            check.Parameters.AddWithValue("@id", id);

            int rented = Convert.ToInt32(check.ExecuteScalar());

            // Нельзя установить общее количество экземпляров
            // меньше числа уже выданных экземпляров
            if (copies < rented)
            {
                MessageBox.Show(
                    "Количество экземпляров не может быть меньше числа выданных книг.");
                return;
            }

            using var command = connection.CreateCommand();

            // Обновляем сведения о выбранной книге
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

            command.ExecuteNonQuery(); // Сохраняем изменения

            LoadBooks();  // Обновляем таблицу
            ClearFields(); // Очищаем поля
        }

        // Удаление выбранной книги
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID книги
            int id = SelectedId();

            // Проверяем, выбрана ли книга
            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для удаления.");
                return;
            }

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Проверяем, остались ли выданные экземпляры книги
            using var check = connection.CreateCommand();

            check.CommandText = @"
                SELECT COUNT(*)
                FROM Rentals
                WHERE BookId = @id AND ReturnDate IS NULL;";

            check.Parameters.AddWithValue("@id", id);

            // Если есть невозвращённые экземпляры,
            // удаление книги запрещается
            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
            {
                MessageBox.Show(
                    "Сначала верните все выданные экземпляры книги.");
                return;
            }

            // Запрашиваем подтверждение перед удалением
            if (MessageBox.Show(
                "Удалить выбранную книгу?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return; // Отменяем удаление, если выбран ответ "Нет"
            }

            using var command = connection.CreateCommand();

            // Удаляем книгу по её ID
            command.CommandText =
                "DELETE FROM Books WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();

            LoadBooks();  // Обновляем список книг
            ClearFields(); // Очищаем поля ввода
        }

        // Выдача книги читателю
        private void btnRent_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID выбранной книги
            int id = SelectedId();

            // Проверяем, выбрана ли книга
            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для выдачи.");
                return;
            }

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            // Проверяем количество доступных экземпляров
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

            // Получаем количество доступных экземпляров
            object result = check.ExecuteScalar();

            // Если доступных экземпляров нет, выдачу не выполняем
            if (result == null || result == DBNull.Value ||
                Convert.ToInt32(result) <= 0)
            {
                MessageBox.Show("Нет доступных экземпляров этой книги.");
                return;
            }

            using var command = connection.CreateCommand();

            // Создаём запись о выдаче книги.
            // ReturnDate равен NULL, пока книгу не вернули.
            command.CommandText = @"
                INSERT INTO Rentals (BookId, RentDate, ReturnDate)
                VALUES (@id, @date, NULL);";

            command.Parameters.AddWithValue("@id", id);

            // Сохраняем текущую дату выдачи
            command.Parameters.AddWithValue(
                "@date", DateTime.Now.ToString("yyyy-MM-dd"));

            command.ExecuteNonQuery(); // Регистрируем выдачу

            LoadBooks(); // Пересчитываем доступные экземпляры
        }

        // Возврат ранее выданной книги
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID выбранной книги
            int id = SelectedId();

            // Проверяем, выбрана ли книга
            if (id == -1)
            {
                MessageBox.Show("Выберите книгу для возврата.");
                return;
            }

            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            // Находим последнюю незакрытую запись о выдаче
            // и устанавливаем дату возврата
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

            // Записываем дату возврата книги
            command.Parameters.AddWithValue(
                "@date", DateTime.Now.ToString("yyyy-MM-dd"));

            command.Parameters.AddWithValue("@id", id);

            // Выполняем возврат и проверяем, была ли запись изменена
            int affected = command.ExecuteNonQuery();

            // Если ничего не изменилось, активной выдачи не было
            if (affected == 0)
                MessageBox.Show("У этой книги нет выданных экземпляров.");

            LoadBooks(); // Обновляем список и доступное количество
        }

        // Обработчик выбора книги в таблице
        private void dgvBooks_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка с данными книги
            if (dgvBooks.SelectedItem is not DataRowView row)
                return;

            // Переносим название и автора в поля формы
            txtTitle.Text = row["Title"]?.ToString() ?? "";
            txtAuthor.Text = row["Author"]?.ToString() ?? "";

            // Если год не указан, оставляем поле пустым
            txtYear.Text = row["Year"] == DBNull.Value
                ? ""
                : row["Year"].ToString();

            // Заполняем поле общим количеством экземпляров
            txtCopies.Text = row["TotalCopies"].ToString();
        }

        // Обработчик кнопки очистки полей
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields(); // Вызываем общий метод очистки
        }

        // Очистка полей формы и снятие выделения
        private void ClearFields()
        {
            // Снимаем выделение с выбранной книги
            dgvBooks.SelectedItem = null;

            // Очищаем текстовые поля
            txtTitle.Clear();
            txtAuthor.Clear();
            txtYear.Clear();

            // По умолчанию у новой книги один экземпляр
            txtCopies.Text = "1";
        }
    }
}
