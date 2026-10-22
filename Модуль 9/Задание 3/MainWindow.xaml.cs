
using Microsoft.Data.Sqlite; 
using System; 
using System.Data; 
using System.IO; 
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Media; 
using System.Windows.Threading; 

namespace Задание_3 
{
    public partial class MainWindow : Window 
    {
        // Путь к файлу базы данных SQLite
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 9\Задание 3\tasks.db";

        // Строка подключения к базе данных
        private string ConnectionString => $"Data Source={dbPath}";

        private int selectedTaskId = 0; // ID выбранной задачи; 0 означает, что задача не выбрана

        // Создание таймера для периодической проверки сроков
        private readonly DispatcherTimer notificationTimer =
            new DispatcherTimer();

        // Конструктор главного окна
        public MainWindow()
        {
            InitializeComponent(); // Загрузка элементов интерфейса из XAML

            try // Начало обработки возможных ошибок
            {
                InitializeDatabase(); // Создание базы данных и таблицы
                RefreshTasks(); // Отображение задач в таблице
                ClearFields(); // Очистка полей ввода
                CheckDeadlines(); // Первоначальная проверка сроков выполнения

                notificationTimer.Interval = TimeSpan.FromMinutes(1); // Проверка сроков каждую минуту
                notificationTimer.Tick += (s, e) => CheckDeadlines(); // Запуск проверки при срабатывании таймера
                notificationTimer.Start(); // Запуск таймера
            }
            catch (Exception ex) // Перехват ошибок запуска
            {
                MessageBox.Show(
                    "Ошибка при запуске приложения:\n" + ex.Message, // Текст ошибки
                    "Ошибка", // Заголовок окна сообщения
                    MessageBoxButton.OK, // Кнопка подтверждения
                    MessageBoxImage.Error); // Значок ошибки
            }
        }

        // Метод создания папки и таблицы SQLite
        private void InitializeDatabase()
        {
            string directory = Path.GetDirectoryName(dbPath); // Получение пути к папке базы данных

            if (!Directory.Exists(directory)) // Проверка существования папки
                Directory.CreateDirectory(directory); // Создание папки, если её нет

            using var connection = new SqliteConnection(ConnectionString); // Создание подключения к SQLite
            connection.Open(); // Открытие подключения

            using var command = connection.CreateCommand(); // Создание SQL-команды

            // SQL-запрос создания таблицы задач, если она ещё не существует
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Priority TEXT NOT NULL,
                    Deadline TEXT NOT NULL,
                    Progress INTEGER NOT NULL DEFAULT 0
                        CHECK (Progress BETWEEN 0 AND 100),
                    Status TEXT NOT NULL
                );";

            command.ExecuteNonQuery(); // Выполнение команды создания таблицы
        }

        // Метод получения данных из базы в виде DataTable
        private DataTable GetTable(string sql)
        {
            using var connection = new SqliteConnection(ConnectionString); // Создание подключения
            connection.Open(); // Открытие базы данных

            using var command = connection.CreateCommand(); // Создание SQL-команды
            command.CommandText = sql; // Установка SQL-запроса

            using var reader = command.ExecuteReader(); // Выполнение запроса на чтение

            DataTable table = new DataTable(); // Создание таблицы в памяти
            table.Load(reader); // Заполнение таблицы результатами запроса

            return table; // Возврат полученных данных
        }


        // Метод выполнения SQL-команд добавления, изменения и удаления
        private void ExecuteSql(
            string sql, // Текст SQL-запроса
            params SqliteParameter[] parameters) // Массив параметров запроса
        {
            using var connection = new SqliteConnection(ConnectionString); // Создание подключения
            connection.Open(); // Открытие подключения

            using var command = connection.CreateCommand(); // Создание команды
            command.CommandText = sql; // Установка текста SQL-запроса
            command.Parameters.AddRange(parameters); // Добавление параметров запроса

            command.ExecuteNonQuery(); // Выполнение команды без возврата таблицы
        }

        // Метод создания параметра SQL-запроса
        private SqliteParameter Param(string name, object value)
        {
            return new SqliteParameter(name, value ?? DBNull.Value); // Передача значения или SQL NULL
        }


        // Метод загрузки задач в таблицу интерфейса
        private void RefreshTasks()
        {
            dgTasks.ItemsSource = GetTable(@"
                SELECT
                    Id AS 'ID',
                    Title AS 'Название',
                    Description AS 'Описание',
                    Priority AS 'Приоритет',
                    Deadline AS 'Срок выполнения',
                    Progress AS 'Прогресс (%)',
                    Status AS 'Статус'
                FROM Tasks
                ORDER BY Deadline, Id;").DefaultView; // Сортировка задач по сроку и ID
        }


        // Метод проверки правильности введённых данных
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) // Проверка названия задачи
            {
                MessageBox.Show("Введите название задачи."); // Вывод предупреждения
                return false; // Проверка не пройдена
            }

            if (cmbPriority.SelectedItem == null) // Проверка выбранного приоритета
            {
                MessageBox.Show("Выберите приоритет."); // Вывод предупреждения
                return false; // Проверка не пройдена
            }

            if (dpDeadline.SelectedDate == null) // Проверка выбранной даты
            {
                MessageBox.Show("Выберите срок выполнения."); // Вывод предупреждения
                return false; // Проверка не пройдена
            }

            // Проверка, что прогресс является числом от 0 до 100
            if (!int.TryParse(txtProgress.Text, out int progress)
                || progress < 0 || progress > 100)
            {
                MessageBox.Show("Прогресс должен быть от 0 до 100."); // Сообщение об ошибке
                return false; // Проверка не пройдена
            }

            if (cmbStatus.SelectedItem == null) // Проверка выбранного статуса
            {
                MessageBox.Show("Выберите статус."); // Вывод предупреждения
                return false; // Проверка не пройдена
            }

            if (progress == 100) // Проверка полного выполнения задачи
                cmbStatus.SelectedIndex = 2; // Установка третьего статуса — «Завершена»

            return true; // Все проверки пройдены
        }

        // Метод получения текста выбранного элемента ComboBox
        private string SelectedComboValue(ComboBox combo)
        {
            return (combo.SelectedItem as ComboBoxItem)? // Получение выбранного элемента
                .Content?.ToString() ?? ""; // Возврат его текста или пустой строки
        }

        // Обработчик нажатия кнопки добавления
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try // Обработка ошибок добавления
            {
                if (!ValidateInput()) // Проверка введённых данных
                    return; // Прекращение добавления при ошибке

                // Добавление новой задачи в базу данных
                ExecuteSql(@"
                    INSERT INTO Tasks
                        (Title, Description, Priority, Deadline, Progress, Status)
                    VALUES
                        (@title, @description, @priority,
                         @deadline, @progress, @status);",
                    Param("@title", txtTitle.Text.Trim()), // Передача названия задачи
                    Param("@description", txtDescription.Text.Trim()), // Передача описания
                    Param("@priority", SelectedComboValue(cmbPriority)), // Передача приоритета
                    Param("@deadline",
                        dpDeadline.SelectedDate.Value.ToString("yyyy-MM-dd")), // Передача срока в формате даты
                    Param("@progress", int.Parse(txtProgress.Text)), // Передача прогресса
                    Param("@status", SelectedComboValue(cmbStatus))); // Передача статуса

                RefreshTasks(); // Обновление таблицы задач
                ClearFields(); // Очистка полей ввода
                CheckDeadlines(); // Повторная проверка сроков

                MessageBox.Show("Задача успешно добавлена."); // Подтверждение добавления
            }
            catch (Exception ex) // Перехват ошибок
            {
                MessageBox.Show("Ошибка добавления:\n" + ex.Message); // Вывод сообщения об ошибке
            }
        }

        // Обработчик нажатия кнопки изменения
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try // Обработка ошибок редактирования
            {
                if (selectedTaskId == 0) // Проверка выбора задачи
                {
                    MessageBox.Show("Выберите задачу в таблице."); // Предупреждение
                    return; // Остановка метода
                }

                if (!ValidateInput()) // Проверка введённых данных
                    return; // Отмена изменения при ошибке

                // Обновление информации о выбранной задаче
                ExecuteSql(@"
                    UPDATE Tasks
                    SET Title = @title,
                        Description = @description,
                        Priority = @priority,
                        Deadline = @deadline,
                        Progress = @progress,
                        Status = @status
                    WHERE Id = @id;",
                    Param("@title", txtTitle.Text.Trim()), // Новое название
                    Param("@description", txtDescription.Text.Trim()), // Новое описание
                    Param("@priority", SelectedComboValue(cmbPriority)), // Новый приоритет
                    Param("@deadline",
                        dpDeadline.SelectedDate.Value.ToString("yyyy-MM-dd")), // Новый срок выполнения
                    Param("@progress", int.Parse(txtProgress.Text)), // Новый прогресс
                    Param("@status", SelectedComboValue(cmbStatus)), // Новый статус
                    Param("@id", selectedTaskId)); // ID изменяемой задачи

                RefreshTasks(); // Обновление таблицы
                ClearFields(); // Очистка полей
                CheckDeadlines(); // Проверка сроков после изменения

                MessageBox.Show("Задача успешно изменена."); // Подтверждение изменения
            }
            catch (Exception ex) // Перехват ошибок
            {
                MessageBox.Show("Ошибка изменения:\n" + ex.Message); // Вывод ошибки
            }
        }

        // Обработчик нажатия кнопки удаления
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try // Обработка ошибок удаления
            {
                if (selectedTaskId == 0) // Проверка выбора задачи
                {
                    MessageBox.Show("Выберите задачу в таблице."); // Предупреждение
                    return; // Остановка метода
                }

                // Запрос подтверждения удаления
                if (MessageBox.Show(
                    "Удалить выбранную задачу?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return; // Отмена удаления, если пользователь выбрал «Нет»

                // Удаление выбранной задачи по ID
                ExecuteSql(
                    "DELETE FROM Tasks WHERE Id = @id;",
                    Param("@id", selectedTaskId));

                RefreshTasks(); // Обновление таблицы
                ClearFields(); // Очистка полей
                CheckDeadlines(); // Повторная проверка сроков

                MessageBox.Show("Задача удалена."); // Подтверждение удаления
            }
            catch (Exception ex) // Перехват ошибок
            {
                MessageBox.Show("Ошибка удаления:\n" + ex.Message); // Вывод ошибки
            }
        }

        // ВЫБОР ЗАДАЧИ В ТАБЛИЦЕ

        // Обработчик выбора строки таблицы
        private void dgTasks_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            // Проверка, что выбрана строка с данными
            if (dgTasks.SelectedItem is not DataRowView row)
                return; // Завершение, если строка не выбрана

            selectedTaskId = Convert.ToInt32(row["ID"]); // Сохранение ID задачи

            txtTitle.Text = row["Название"].ToString(); // Заполнение поля названия
            txtDescription.Text = row["Описание"].ToString(); // Заполнение поля описания
            txtProgress.Text = row["Прогресс (%)"].ToString(); // Заполнение поля прогресса

            // Установка даты выполнения выбранной задачи
            dpDeadline.SelectedDate = DateTime.Parse(
                row["Срок выполнения"].ToString());

            SetComboValue(cmbPriority, row["Приоритет"].ToString()); // Установка приоритета
            SetComboValue(cmbStatus, row["Статус"].ToString()); // Установка статуса
        }

        // Метод выбора элемента ComboBox по текстовому значению
        private void SetComboValue(ComboBox combo, string value)
        {
            foreach (ComboBoxItem item in combo.Items) // Перебор всех элементов списка
            {
                if (item.Content?.ToString() == value) // Поиск элемента с нужным текстом
                {
                    combo.SelectedItem = item; // Выбор найденного элемента
                    return; // Завершение метода
                }
            }
        }

        // Обработчик кнопки очистки
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields(); // Вызов метода очистки
        }

        // Метод очистки формы
        private void ClearFields()
        {
            selectedTaskId = 0; // Сброс ID выбранной задачи

            txtTitle.Clear(); // Очистка названия
            txtDescription.Clear(); // Очистка описания
            txtProgress.Text = "0"; // Установка прогресса в 0%

            cmbPriority.SelectedIndex = 1; // Выбор второго приоритета
            cmbStatus.SelectedIndex = 0; // Выбор первого статуса

            dpDeadline.SelectedDate = DateTime.Today.AddDays(1); // Установка срока на завтра

            dgTasks.SelectedItem = null; // Снятие выделения в таблице
        }


        // Метод проверки просроченных и приближающихся сроков
        private void CheckDeadlines()
        {
            try // Обработка возможных ошибок проверки
            {
                // Получение незавершённых задач из базы данных
                DataTable tasks = GetTable(@"
                    SELECT Title, Deadline
                    FROM Tasks
                    WHERE Status <> 'Завершена'
                    ORDER BY Deadline;");

                int overdue = 0; // Счётчик просроченных задач
                int approaching = 0; // Счётчик задач с приближающимся сроком

                // Перебор всех полученных задач
                foreach (DataRow row in tasks.Rows)
                {
                    // Преобразование срока выполнения в дату
                    DateTime deadline = DateTime.Parse(
                        row["Deadline"].ToString());

                    if (deadline.Date < DateTime.Today) // Проверка просрочки
                        overdue++; // Увеличение количества просроченных задач
                    else if (deadline.Date <= DateTime.Today.AddDays(2)) // Срок сегодня или в ближайшие два дня
                        approaching++; // Увеличение количества приближающихся задач
                }

                // Проверка наличия задач, требующих внимания
                if (overdue > 0 || approaching > 0)
                {
                    // Формирование предупреждения
                    lblNotifications.Text =
                        $"ВНИМАНИЕ! Просрочено задач: {overdue}. " +
                        $"Скоро срок выполнения: {approaching}.";

                    lblNotifications.Foreground = Brushes.DarkRed; // Красный цвет предупреждения
                }
                else // Если проблем со сроками нет
                {
                    lblNotifications.Text =
                        "Уведомления: приближающихся сроков нет."; // Сообщение об отсутствии предупреждений

                    lblNotifications.Foreground = Brushes.DarkGreen; // Зелёный цвет сообщения
                }
            }
            catch (Exception ex) // Перехват ошибок проверки
            {
                lblNotifications.Text =
                    "Ошибка проверки сроков: " + ex.Message; // Вывод ошибки в интерфейсе
            }
        }

        // Метод, вызываемый при закрытии окна
        protected override void OnClosed(EventArgs e)
        {
            notificationTimer.Stop(); // Остановка таймера уведомлений
            base.OnClosed(e); // Вызов стандартной обработки закрытия окна
        }
    }
}
