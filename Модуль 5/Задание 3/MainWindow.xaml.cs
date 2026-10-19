using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Задание_3
{
    public partial class MainWindow : Window
    {
        // Создаем динамическую коллекцию задач, которая автоматически уведомляет интерфейс об изменениях (добавление/удаление)
        private readonly ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

        public MainWindow()
        {
            InitializeComponent(); // Инициализация графических компонентов из XAML-разметки

            TaskList.ItemsSource = tasks; // Привязываем коллекцию задач к элементу отображения списка (например, ListBox)
            UpdateStatus(); // Вызываем первоначальное обновление счетчиков задач внизу окна
        }

        // Обработчик клика по кнопке добавления задачи
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            AddTask(); // Вызываем метод создания и валидации новой задачи
        }

        // Обработчик нажатия клавиш в поле ввода для перехвата клавиши Enter
        private void TaskInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Если пользователь нажал именно Enter
            {
                AddTask(); // Вызываем метод создания и валидации новой задачи
            }
        }

        // Внутренний метод для валидации текста и добавления новой задачи в общий список
        private void AddTask()
        {
            string title = TaskInput.Text.Trim(); // Обрезаем лишние пробелы в начале и конце введенной строки

            if (string.IsNullOrWhiteSpace(title)) // Проверяем, не осталось ли поле ввода пустым
            {
                // Показываем предупреждение, если пользователь ничего не ввел
                MessageBox.Show(
                    "Введите название задачи!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                TaskInput.Focus(); // Возвращаем курсор обратно в текстовое поле для ввода
                return; // Прерываем добавление пустой задачи
            }

            TaskItem task = new TaskItem(title); // Создаем новый объект задачи с введенным названием

            task.PropertyChanged += Task_PropertyChanged; // Подписываемся на изменение свойств этой конкретной задачи (выполнение)
            tasks.Add(task); // Добавляем созданную задачу в общую коллекцию на экране

            TaskInput.Clear(); // Очищаем текстовое поле ввода от старого текста
            TaskInput.Focus(); // Снова переносим фокус курсора на поле ввода для удобства

            UpdateStatus(); // Пересчитываем статистику задач
        }

        // Обработчик клика по кнопке удаления задачи (обычно кнопка-крестик внутри строки списка)
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            // Извлекаем кнопку и привязанный к текущей строке объект задачи
            if (sender is Button button && button.DataContext is TaskItem task)
            {
                task.PropertyChanged -= Task_PropertyChanged; // Отписываемся от отслеживания изменений удаляемой задачи
                tasks.Remove(task); // Удаляем саму задачу из коллекции на холсте

                UpdateStatus(); // Пересчитываем статистику после удаления элемента
            }
        }

        // Метод, срабатывающий при изменении внутренних свойств задачи (например, когда ставится галочка)
        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskItem.IsCompleted)) // Проверяем, изменилось ли именно свойство завершения задачи
            {
                UpdateStatus(); // Обновляем общую статистику выполненных задач
            }
        }

        // Вспомогательный обработчик для ручного изменения состояния чекбокса в интерфейсе
        private void TaskCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateStatus(); // Обновляем строку состояния при переключении чекбокса
        }

        // Метод расчета и обновления текстовой строки со статистикой по всем задачам
        private void UpdateStatus()
        {
            int completed = 0; // Инициализируем локальный счетчик выполненных задач

            foreach (TaskItem task in tasks) // Проходим циклом по каждой задаче из списка
            {
                if (task.IsCompleted) // Если у задачи стоит флаг "Выполнено"
                {
                    completed++; // Увеличиваем счетчик завершенных на единицу
                }
            }

            // Формируем красивую строку текста с результатами и выводим её в статус-бар на экран
            StatusText.Text =
                $"Всего задач: {tasks.Count} | " +
                $"Выполнено: {completed} | " +
                $"Осталось: {tasks.Count - completed}";
        }
    }

    // Класс для отдельной задачи, реализующий интерфейс автоматического уведомления интерфейса (Binding)
    public class TaskItem : INotifyPropertyChanged
    {
        private bool isCompleted; // Закрытое поле для хранения статуса выполнения задачи

        public string Title { get; set; } // Автосвойство для хранения названия задачи

        // Свойство статуса выполнения задачи с логикой отправки уведомлений графическому интерфейсу WPF
        public bool IsCompleted
        {
            get
            {
                return isCompleted; // Возвращаем текущее состояние флага
            }
            set
            {
                if (isCompleted != value) // Проверяем, изменилось ли значение на новое (чтобы избежать лишней работы)
                {
                    isCompleted = value; // Присваиваем новое значение свойству
                    OnPropertyChanged(); // Оповещаем  через событие, что галочку переключили
                }
            }
        }

        // Конструктор для создания задачи с её названием
        public TaskItem(string title)
        {
            Title = title; // Записываем название
            IsCompleted = false; // При создании любая задача по умолчанию считается невыполненной
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, необходимое интерфейсу INotifyPropertyChanged для связывания

        // Вспомогательный метод для безопасного вызова события изменения параметров
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Передаем интерфейсу WPF имя свойства, которое только что поменялось, для перерисовки элемента на экране
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
