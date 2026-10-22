
using Microsoft.Data.Sqlite; 
using Microsoft.Win32; 
using System; 
using System.Data; 
using System.IO; 
using System.Windows; 
using System.Windows.Controls; 

namespace Задание_2 
{
    public partial class MainWindow : Window // Объявляем главное окно приложения.
    {
        // Указываем путь к файлу базы данных SQLite.
        private readonly string dbPath =
            @"D:\Колледж\Практика1\praktika\Модуль 9\Задание 2\project_manager.db";

        // Создаём строку подключения к базе данных.
        // Foreign Keys=True включает поддержку внешних ключей SQLite.
        private string ConnectionString =>
            $"Data Source={dbPath};Foreign Keys=True";

        // Храним ID выбранного проекта.
        private int selectedProjectId = 0;

        // Храним ID выбранного сотрудника.
        private int selectedEmployeeId = 0;

        // Храним ID выбранной задачи.
        private int selectedTaskId = 0;

        public MainWindow() // Конструктор главного окна.
        {
            InitializeComponent(); // Загружаем элементы интерфейса из XAML.

            try // Начинаем обработку возможных ошибок.
            {
                // Получаем путь к папке, в которой хранится база данных.
                string folder = Path.GetDirectoryName(dbPath);

                // Проверяем, что путь к папке получен.
                if (!string.IsNullOrEmpty(folder))
                    Directory.CreateDirectory(folder); // Создаём папку, если её нет.

                InitializeDatabase(); // Создаём таблицы базы данных.
                AddSampleData(); // Добавляем демонстрационные записи, если таблицы пусты.
                RefreshAll(); // Загружаем данные в таблицы интерфейса.
            }
            catch (Exception ex) // Перехватываем ошибку при запуске.
            {
                MessageBox.Show( // Показываем сообщение об ошибке.
                    "Ошибка при открытии базы данных:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Открываем соединение с SQLite.
        private SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(ConnectionString); // Создаём соединение.
            connection.Open(); // Открываем соединение с базой.
            return connection; // Возвращаем открытое соединение.
        }

        // Создаём таблицы базы данных.
        private void InitializeDatabase()
        {
            using var connection = OpenConnection(); // Открываем соединение.
            using var command = connection.CreateCommand(); // Создаём SQL-команду.

            command.CommandText = @" 
                CREATE TABLE IF NOT EXISTS Projects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT
                );

                CREATE TABLE IF NOT EXISTS Employees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT NOT NULL,
                    Position TEXT
                );

                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    ProjectId INTEGER NOT NULL,
                    EmployeeId INTEGER,
                    Status TEXT NOT NULL DEFAULT 'Новая',
                    Progress INTEGER NOT NULL DEFAULT 0
                        CHECK (Progress BETWEEN 0 AND 100),
                    FOREIGN KEY (ProjectId) REFERENCES Projects(Id)
                        ON DELETE CASCADE,
                    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
                        ON DELETE SET NULL
                );
            "; // Задаём SQL для создания трёх связанных таблиц.

            command.ExecuteNonQuery(); // Выполняем создание таблиц.
        }

        // Добавляем демонстрационные данные, если таблицы пустые.
        private void AddSampleData()
        {
            using var connection = OpenConnection(); // Открываем соединение.
            using var transaction = connection.BeginTransaction(); // Начинаем транзакцию.

            // Добавляем пример проекта, только если проектов ещё нет.
            ExecuteNonQuery(connection, transaction, @"
                INSERT INTO Projects (Name, Description)
                SELECT 'Учебный проект', 'Пример проекта для проверки'
                WHERE NOT EXISTS (SELECT 1 FROM Projects);");

            // Добавляем пример сотрудника, только если сотрудников ещё нет.
            ExecuteNonQuery(connection, transaction, @"
                INSERT INTO Employees (FullName, Position)
                SELECT 'Иванов Иван', 'Разработчик'
                WHERE NOT EXISTS (SELECT 1 FROM Employees);");

            // Добавляем пример задачи, только если задач ещё нет.
            ExecuteNonQuery(connection, transaction, @"
                INSERT INTO Tasks (Title, ProjectId, EmployeeId, Status, Progress)
                SELECT
                    'Создать интерфейс',
                    (SELECT Id FROM Projects ORDER BY Id LIMIT 1),
                    (SELECT Id FROM Employees ORDER BY Id LIMIT 1),
                    'В работе',
                    50
                WHERE NOT EXISTS (SELECT 1 FROM Tasks);");

            transaction.Commit(); // Подтверждаем изменения в базе.
        }

        // Выполняем SQL-команду внутри переданной транзакции.
        private void ExecuteNonQuery(
            SqliteConnection connection, // Передаём соединение с базой.
            SqliteTransaction transaction, // Передаём текущую транзакцию.
            string sql) // Получаем SQL-команду.
        {
            using var command = connection.CreateCommand(); // Создаём команду.
            command.Transaction = transaction; // Привязываем команду к транзакции.
            command.CommandText = sql; // Устанавливаем SQL-запрос.
            command.ExecuteNonQuery(); // Выполняем запрос без возврата таблицы.
        }

        // Получаем результат SQL-запроса в виде DataTable.
        private DataTable GetTable(string sql)
        {
            using var connection = OpenConnection(); // Открываем соединение.
            using var command = connection.CreateCommand(); // Создаём SQL-команду.

            command.CommandText = sql; // Устанавливаем запрос.

            using var reader = command.ExecuteReader(); // Выполняем запрос и читаем результат.

            DataTable table = new DataTable(); // Создаём пустую таблицу в памяти.
            table.Load(reader); // Загружаем в неё результат запроса.

            return table; // Возвращаем заполненную таблицу.
        }

        // Выполняем SQL-команду с параметрами.
        private int ExecuteCommand(string sql, params SqliteParameter[] parameters)
        {
            using var connection = OpenConnection(); // Открываем соединение.
            using var command = connection.CreateCommand(); // Создаём команду.

            command.CommandText = sql; // Устанавливаем SQL-запрос.

            foreach (var parameter in parameters) // Перебираем все параметры запроса.
                command.Parameters.Add(parameter); // Добавляем параметр в команду.

            return command.ExecuteNonQuery(); // Выполняем команду и возвращаем число изменённых строк.
        }

        // Обновляем все таблицы и списки на экране.
        private void RefreshAll()
        {
            // Загружаем список проектов в таблицу.
            dgProjects.ItemsSource = GetTable(
                "SELECT Id, Name, Description FROM Projects ORDER BY Id").DefaultView;

            // Загружаем список сотрудников в таблицу.
            dgEmployees.ItemsSource = GetTable(
                "SELECT Id, FullName, Position FROM Employees ORDER BY Id").DefaultView;

            // Получаем список проектов для выбора при создании задачи.
            DataTable projects = GetTable(
                "SELECT Id, Name FROM Projects ORDER BY Name");

            cmbTaskProject.DisplayMemberPath = "Name"; // Показываем название проекта.
            cmbTaskProject.SelectedValuePath = "Id"; // Используем ID проекта как значение.
            cmbTaskProject.ItemsSource = projects.DefaultView; // Заполняем список проектов.

            // Получаем список сотрудников для назначения задачи.
            DataTable employees = GetTable(
                "SELECT Id, FullName FROM Employees ORDER BY FullName");

            cmbTaskEmployee.DisplayMemberPath = "FullName"; // Показываем ФИО сотрудника.
            cmbTaskEmployee.SelectedValuePath = "Id"; // Используем ID сотрудника как значение.
            cmbTaskEmployee.ItemsSource = employees.DefaultView; // Заполняем список сотрудников.

            // Загружаем задачи вместе с названиями проектов и ФИО сотрудников.
            dgTasks.ItemsSource = GetTable(@"
                SELECT
                    T.Id,
                    T.Title,
                    P.Name AS Project,
                    COALESCE(E.FullName, 'Не назначен') AS Employee,
                    T.Status,
                    T.Progress
                FROM Tasks T
                INNER JOIN Projects P ON T.ProjectId = P.Id
                LEFT JOIN Employees E ON T.EmployeeId = E.Id
                ORDER BY T.Id").DefaultView;
        }

        // Добавляем новый проект.
        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, введено ли название проекта.
            if (string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                MessageBox.Show("Введите название проекта."); // Показываем предупреждение.
                return; // Прерываем выполнение метода.
            }

            // Добавляем проект в базу с помощью параметров.
            ExecuteCommand(
                "INSERT INTO Projects (Name, Description) VALUES (@name, @description)",
                new SqliteParameter("@name", txtProjectName.Text.Trim()),
                new SqliteParameter("@description", txtProjectDescription.Text.Trim()));

            RefreshAll(); // Обновляем таблицы.
            ClearProjectFields(); // Очищаем поля проекта.

            MessageBox.Show("Проект добавлен."); // Сообщаем об успехе.
        }

        // Изменяем выбранный проект.
        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли проект.
            if (selectedProjectId == 0)
            {
                MessageBox.Show("Выберите проект для редактирования.");
                return;
            }

            // Проверяем, введено ли название.
            if (string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                MessageBox.Show("Введите название проекта.");
                return;
            }

            // Обновляем название и описание выбранного проекта.
            ExecuteCommand(
                @"UPDATE Projects
                  SET Name = @name, Description = @description
                  WHERE Id = @id",
                new SqliteParameter("@name", txtProjectName.Text.Trim()),
                new SqliteParameter("@description", txtProjectDescription.Text.Trim()),
                new SqliteParameter("@id", selectedProjectId));

            RefreshAll(); // Обновляем данные на экране.
            ClearProjectFields(); // Очищаем поля.

            MessageBox.Show("Проект изменён."); // Сообщаем об успехе.
        }

        // Удаляем выбранный проект.
        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли проект.
            if (selectedProjectId == 0)
            {
                MessageBox.Show("Выберите проект для удаления.");
                return;
            }

            // Запрашиваем подтверждение удаления.
            if (MessageBox.Show(
                "Удалить проект вместе с его задачами?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return; // Отменяем удаление, если пользователь выбрал «Нет».

            // Удаляем проект; его задачи удалятся благодаря ON DELETE CASCADE.
            ExecuteCommand(
                "DELETE FROM Projects WHERE Id = @id",
                new SqliteParameter("@id", selectedProjectId));

            RefreshAll(); // Обновляем таблицы.
            ClearProjectFields(); // Очищаем поля проекта.
        }

        // Обрабатываем выбор проекта в таблице.
        private void dgProjects_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка с данными.
            if (dgProjects.SelectedItem is DataRowView row)
            {
                selectedProjectId = Convert.ToInt32(row["Id"]); // Сохраняем ID.
                txtProjectName.Text = row["Name"]?.ToString() ?? ""; // Заполняем название.
                txtProjectDescription.Text = row["Description"]?.ToString() ?? ""; // Заполняем описание.
            }
        }

        // Очищаем поля проекта.
        private void ClearProjectFields()
        {
            selectedProjectId = 0; // Сбрасываем выбранный ID.
            txtProjectName.Clear(); // Очищаем название.
            txtProjectDescription.Clear(); // Очищаем описание.
            dgProjects.SelectedItem = null; // Снимаем выделение строки.
        }

        // Добавляем сотрудника.
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, введено ли ФИО.
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника.");
                return;
            }

            // Добавляем сотрудника в базу.
            ExecuteCommand(
                "INSERT INTO Employees (FullName, Position) VALUES (@name, @position)",
                new SqliteParameter("@name", txtEmployeeName.Text.Trim()),
                new SqliteParameter("@position", txtEmployeePosition.Text.Trim()));

            RefreshAll(); // Обновляем таблицы и списки.
            ClearEmployeeFields(); // Очищаем поля сотрудника.

            MessageBox.Show("Сотрудник добавлен."); // Показываем сообщение.
        }

        // Редактируем сотрудника.
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли сотрудник.
            if (selectedEmployeeId == 0)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.");
                return;
            }

            // Проверяем наличие ФИО.
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника.");
                return;
            }

            // Обновляем ФИО и должность сотрудника.
            ExecuteCommand(
                @"UPDATE Employees
                  SET FullName = @name, Position = @position
                  WHERE Id = @id",
                new SqliteParameter("@name", txtEmployeeName.Text.Trim()),
                new SqliteParameter("@position", txtEmployeePosition.Text.Trim()),
                new SqliteParameter("@id", selectedEmployeeId));

            RefreshAll(); // Обновляем интерфейс.
            ClearEmployeeFields(); // Очищаем поля.

            MessageBox.Show("Данные сотрудника изменены."); // Сообщаем об успехе.
        }

        // Удаляем сотрудника.
        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли сотрудник.
            if (selectedEmployeeId == 0)
            {
                MessageBox.Show("Выберите сотрудника для удаления.");
                return;
            }

            // Просим подтвердить удаление.
            if (MessageBox.Show(
                "Удалить выбранного сотрудника?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            // Удаляем сотрудника; назначенные ему задачи останутся без исполнителя.
            ExecuteCommand(
                "DELETE FROM Employees WHERE Id = @id",
                new SqliteParameter("@id", selectedEmployeeId));

            RefreshAll(); // Обновляем таблицы.
            ClearEmployeeFields(); // Очищаем поля.
        }

        // Обрабатываем выбор сотрудника.
        private void dgEmployees_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка сотрудника.
            if (dgEmployees.SelectedItem is DataRowView row)
            {
                selectedEmployeeId = Convert.ToInt32(row["Id"]); // Сохраняем ID.
                txtEmployeeName.Text = row["FullName"]?.ToString() ?? ""; // Заполняем ФИО.
                txtEmployeePosition.Text = row["Position"]?.ToString() ?? ""; // Заполняем должность.
            }
        }

        // Очищаем поля сотрудника.
        private void ClearEmployeeFields()
        {
            selectedEmployeeId = 0; // Сбрасываем ID.
            txtEmployeeName.Clear(); // Очищаем ФИО.
            txtEmployeePosition.Clear(); // Очищаем должность.
            dgEmployees.SelectedItem = null; // Снимаем выделение.
        }

        // Добавляем задачу.
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateTask()) // Проверяем корректность введённых данных.
                return; // Прерываем добавление, если проверка не пройдена.

            // Сохраняем задачу в базе.
            ExecuteCommand(
                @"INSERT INTO Tasks
                    (Title, ProjectId, EmployeeId, Status, Progress)
                  VALUES
                    (@title, @projectId, @employeeId, @status, @progress)",
                new SqliteParameter("@title", txtTaskTitle.Text.Trim()),
                new SqliteParameter("@projectId", cmbTaskProject.SelectedValue),
                new SqliteParameter("@employeeId",
                    cmbTaskEmployee.SelectedValue ?? (object)DBNull.Value),
                new SqliteParameter("@status", GetSelectedStatus()),
                new SqliteParameter("@progress", int.Parse(txtTaskProgress.Text)));

            RefreshAll(); // Обновляем список задач.
            ClearTaskFields(); // Очищаем поля задачи.

            MessageBox.Show("Задача добавлена."); // Показываем сообщение.
        }

        // Редактируем выбранную задачу.
        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли задача.
            if (selectedTaskId == 0)
            {
                MessageBox.Show("Выберите задачу для редактирования.");
                return;
            }

            if (!ValidateTask()) // Проверяем поля задачи.
                return;

            // Обновляем данные выбранной задачи.
            ExecuteCommand(
                @"UPDATE Tasks SET
                    Title = @title,
                    ProjectId = @projectId,
                    EmployeeId = @employeeId,
                    Status = @status,
                    Progress = @progress
                  WHERE Id = @id",
                new SqliteParameter("@title", txtTaskTitle.Text.Trim()),
                new SqliteParameter("@projectId", cmbTaskProject.SelectedValue),
                new SqliteParameter("@employeeId",
                    cmbTaskEmployee.SelectedValue ?? (object)DBNull.Value),
                new SqliteParameter("@status", GetSelectedStatus()),
                new SqliteParameter("@progress", int.Parse(txtTaskProgress.Text)),
                new SqliteParameter("@id", selectedTaskId));

            RefreshAll(); // Обновляем данные.
            ClearTaskFields(); // Очищаем поля.

            MessageBox.Show("Задача изменена."); // Сообщаем об успехе.
        }

        // Удаляем выбранную задачу.
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли задача.
            if (selectedTaskId == 0)
            {
                MessageBox.Show("Выберите задачу для удаления.");
                return;
            }

            // Запрашиваем подтверждение удаления.
            if (MessageBox.Show(
                "Удалить выбранную задачу?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            // Удаляем задачу по её ID.
            ExecuteCommand(
                "DELETE FROM Tasks WHERE Id = @id",
                new SqliteParameter("@id", selectedTaskId));

            RefreshAll(); // Обновляем список.
            ClearTaskFields(); // Очищаем поля задачи.
        }

        // Обрабатываем выбор задачи в таблице.
        private void dgTasks_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли строка с задачей.
            if (dgTasks.SelectedItem is not DataRowView row)
                return;

            selectedTaskId = Convert.ToInt32(row["Id"]); // Сохраняем ID задачи.
            txtTaskTitle.Text = row["Title"]?.ToString() ?? ""; // Заполняем название задачи.
            txtTaskProgress.Text = row["Progress"]?.ToString() ?? "0"; // Заполняем прогресс.

            // Получаем дополнительные данные выбранной задачи.
            DataTable task = GetTable(
                "SELECT ProjectId, EmployeeId, Status FROM Tasks WHERE Id = " +
                selectedTaskId);

            if (task.Rows.Count == 0) // Проверяем, найдена ли задача.
                return;

            // Выбираем проект, к которому относится задача.
            cmbTaskProject.SelectedValue = Convert.ToInt32(task.Rows[0]["ProjectId"]);

            // Выбираем сотрудника или оставляем задачу без исполнителя.
            cmbTaskEmployee.SelectedValue =
                task.Rows[0]["EmployeeId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(task.Rows[0]["EmployeeId"]);

            // Получаем статус задачи.
            string status = task.Rows[0]["Status"]?.ToString() ?? "Новая";

            // Ищем соответствующий статус в выпадающем списке.
            for (int i = 0; i < cmbTaskStatus.Items.Count; i++)
            {
                if (cmbTaskStatus.Items[i] is ComboBoxItem item &&
                    item.Content?.ToString() == status)
                {
                    cmbTaskStatus.SelectedIndex = i; // Выбираем найденный статус.
                    break; // Прекращаем поиск.
                }
            }
        }

        // Проверяем введённые данные задачи.
        private bool ValidateTask()
        {
            // Проверяем название задачи.
            if (string.IsNullOrWhiteSpace(txtTaskTitle.Text))
            {
                MessageBox.Show("Введите название задачи.");
                return false;
            }

            // Проверяем выбор проекта.
            if (cmbTaskProject.SelectedValue == null)
            {
                MessageBox.Show("Сначала добавьте и выберите проект.");
                return false;
            }

            // Проверяем, что прогресс — целое число от 0 до 100.
            if (!int.TryParse(txtTaskProgress.Text, out int progress) ||
                progress < 0 || progress > 100)
            {
                MessageBox.Show("Прогресс должен быть числом от 0 до 100.");
                return false;
            }

            // Проверяем, выбран ли статус.
            if (cmbTaskStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус задачи.");
                return false;
            }

            return true; // Все проверки пройдены.
        }

        // Получаем текст выбранного статуса.
        private string GetSelectedStatus()
        {
            // Если выбран элемент ComboBoxItem, берём его Content.
            if (cmbTaskStatus.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? "Новая";

            // Иначе берём текст выбранного элемента напрямую.
            return cmbTaskStatus.SelectedItem?.ToString() ?? "Новая";
        }

        // Очищаем поля задачи.
        private void ClearTaskFields()
        {
            selectedTaskId = 0; // Сбрасываем ID задачи.
            txtTaskTitle.Clear(); // Очищаем название.
            txtTaskProgress.Text = "0"; // Устанавливаем начальный прогресс.
            cmbTaskProject.SelectedIndex = -1; // Снимаем выбор проекта.
            cmbTaskEmployee.SelectedIndex = -1; // Снимаем выбор сотрудника.
            cmbTaskStatus.SelectedIndex = 0; // Выбираем первый статус.
            dgTasks.SelectedItem = null; // Снимаем выделение задачи.
        }

        // Обновляем данные по нажатию кнопки.
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshAll(); // Повторно загружаем данные из базы.
            MessageBox.Show("Данные обновлены."); // Сообщаем об обновлении.
        }

        // Формируем отчёт и показываем его в таблице.
        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            dgReport.ItemsSource = GetTable(@"
                SELECT
                    T.Id,
                    T.Title,
                    P.Name AS Project,
                    COALESCE(E.FullName, 'Не назначен') AS Employee,
                    T.Status,
                    T.Progress
                FROM Tasks T
                INNER JOIN Projects P ON T.ProjectId = P.Id
                LEFT JOIN Employees E ON T.EmployeeId = E.Id
                ORDER BY T.Id").DefaultView;
        }

        // Экспортируем отчёт в текстовый файл.
        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные для отчёта.
            DataTable report = GetTable(@"
                SELECT
                    T.Id,
                    T.Title,
                    P.Name AS Project,
                    COALESCE(E.FullName, 'Не назначен') AS Employee,
                    T.Status,
                    T.Progress
                FROM Tasks T
                INNER JOIN Projects P ON T.ProjectId = P.Id
                LEFT JOIN Employees E ON T.EmployeeId = E.Id
                ORDER BY T.Id");

            // Создаём диалог сохранения файла.
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Сохранить отчёт", // Заголовок окна.
                Filter = "Текстовые файлы (*.txt)|*.txt", // Разрешаем сохранять TXT.
                FileName = "Отчёт.txt" // Имя файла по умолчанию.
            };

            if (dialog.ShowDialog() != true) // Проверяем, выбрал ли пользователь файл.
                return; // Выходим, если сохранение отменено.

            using StreamWriter writer = new StreamWriter(dialog.FileName); // Открываем файл для записи.

            writer.WriteLine("ОТЧЁТ ПО ПРОЕКТАМ И ЗАДАЧАМ"); // Записываем заголовок отчёта.
            writer.WriteLine("Дата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm")); // Записываем дату и время.
            writer.WriteLine(new string('-', 60)); // Добавляем разделитель.

            // Перебираем все задачи отчёта.
            foreach (DataRow row in report.Rows)
            {
                writer.WriteLine($"Задача: {row["Title"]}"); // Записываем название задачи.
                writer.WriteLine($"Проект: {row["Project"]}"); // Записываем название проекта.
                writer.WriteLine($"Сотрудник: {row["Employee"]}"); // Записываем исполнителя.
                writer.WriteLine($"Статус: {row["Status"]}"); // Записываем статус.
                writer.WriteLine($"Прогресс: {row["Progress"]}%"); // Записываем прогресс.
                writer.WriteLine(new string('-', 60)); // Разделяем записи.
            }

            MessageBox.Show("Отчёт сохранён."); // Сообщаем об успешном сохранении.
        }
    }
}
