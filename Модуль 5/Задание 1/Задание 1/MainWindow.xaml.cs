using System; 
using System.Windows; 
using System.Windows.Controls; 
using System.Windows.Input;
using System.Windows.Media; 
using System.Windows.Shapes; 

namespace Задание_1 // Пространство имен проекта, объединяющее связанные классы
{
    public partial class MainWindow : Window // Объявление частичного класса главного окна, наследующего Window
    {
        private Point startPoint; // Точка, где пользователь нажал мышь (начало рисования)
        private Shape currentShape; // Ссылка на фигуру, которая создается в данный момент
        private bool isDrawing; // Флаг-индикатор: идет ли прямо сейчас процесс рисования

        // Конструктор класса главного окна
        public MainWindow()
        {
            InitializeComponent(); // Инициализация элементов интерфейса, заданных в XAML-разметке
        }

        // Метод для получения выбранного в интерфейсе цвета (возвращает кисть Brush)
        private Brush GetSelectedBrush()
        {
            // Получаем текст выбранного элемента в выпадающем списке цветов
            string color = (ColorSelector.SelectedItem as ComboBoxItem)
                ?.Content?.ToString();

            // Проверяем текст и возвращаем соответствующую системную кисть
            switch (color)
            {
                case "Красный": // Если выбран пункт "Красный"
                    return Brushes.Red; // Возвращаем красную кисть
                case "Синий": // Если выбран пункт "Синий"
                    return Brushes.Blue; // Возвращаем синюю кисть
                case "Зелёный": // Если выбран пункт "Зелёный"
                    return Brushes.Green; // Возвращаем зеленую кисть
                default: // Если совпадений нет или выбор пуст
                    return Brushes.Black; // По умолчанию возвращаем черную кисть
            }
        }

        // Обработчик нажатия левой кнопки мыши на холсте (старт рисования)
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DrawingCanvas); // Фиксируем координаты клика относительно холста
            isDrawing = true; // Включаем режим рисования
            DrawingCanvas.CaptureMouse(); // Захватываем мышь холстом, чтобы движения считывались даже за его пределами

            // Получаем название выбранной фигуры из выпадающего списка
            string selectedShape = (ShapeSelector.SelectedItem as ComboBoxItem)
                ?.Content?.ToString();

            Brush brush = GetSelectedBrush(); // Получаем выбранный пользователем цвет кисти

            // Создаем экземпляр нужной фигуры в зависимости от выбора
            switch (selectedShape)
            {
                case "Линия": // Если выбрана линия
                    currentShape = new Line // Создаем объект линии
                    {
                        X1 = startPoint.X, // Начальная координата X
                        Y1 = startPoint.Y, // Начальная координата Y
                        X2 = startPoint.X, // Конечная X (пока равна начальной)
                        Y2 = startPoint.Y, // Конечная Y (пока равна начальной)
                        Stroke = brush, // Задаем цвет контура
                        StrokeThickness = 2 // Задаем толщину контура в 2 пикселя
                    };
                    break; // Выход из switch

                case "Круг": // Если выбран круг
                    currentShape = new Ellipse // Создаем эллипс (круг настроим при движении)
                    {
                        Stroke = brush, // Задаем цвет контура
                        StrokeThickness = 2 // Задаем толщину контура в 2 пикселя
                    };
                    break; // Выход из switch

                case "Квадрат": // Если выбран квадрат
                    currentShape = new Rectangle // Создаем прямоугольник (квадрат настроим при движении)
                    {
                        Stroke = brush, // Задаем цвет контура
                        StrokeThickness = 2 // Задаем толщину контура в 2 пикселя
                    };
                    break; // Выход из switch
            }

            if (currentShape != null) // Если фигура была успешно создана
            {
                DrawingCanvas.Children.Add(currentShape); // Добавляем созданную фигуру на холст для отображения
            }
        }

        // Обработчик движения мыши по холсту (изменение размеров фигуры на лету)
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing || currentShape == null) // Если режим рисования выключен или фигура не создана
                return; // Прерываем выполнение метода

            Point currentPoint = e.GetPosition(DrawingCanvas); // Получаем текущие координаты курсора мыши

            if (currentShape is Line line) // Если рисуемая фигура — это линия
            {
                line.X2 = currentPoint.X; // Обновляем конечную координату X под курсор
                line.Y2 = currentPoint.Y; // Обновляем конечную координату Y под курсор
            }
            else // Если рисуется круг (Ellipse) или квадрат (Rectangle)
            {
                // Вычисляем левую верхнюю точку описанного прямоугольника (координаты отрисовки)
                double x = Math.Min(startPoint.X, currentPoint.X); // Выбираем наименьший X (для правильного отображения влево)
                double y = Math.Min(startPoint.Y, currentPoint.Y); // Выбираем наименьший Y (для правильного отображения вверх)

                // Вычисляем текущую ширину и высоту по модулю разности координат
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);

                // Логика для сохранения пропорций 1:1 (чтобы получался строгий круг или квадрат)
                if (currentShape is Ellipse || currentShape is Rectangle)
                {
                    double size = Math.Max(width, height); // Берем наибольшую сторону за основу размера

                    // Корректируем верхнюю левую точку, если ведем мышь влево или вверх от старта
                    if (currentPoint.X < startPoint.X) x = startPoint.X - size; // Сдвигаем X влево на размер фигуры
                    if (currentPoint.Y < startPoint.Y) y = startPoint.Y - size; // Сдвигаем Y вверх на размер фигуры

                    width = size; // Устанавливаем ширину равной итоговому размеру
                    height = size; // Устанавливаем высоту равной итоговому размеру
                }

                currentShape.Width = width; // Присваиваем вычисленную ширину фигуре
                currentShape.Height = height; // Присваиваем вычисленную высоту фигуре

                Canvas.SetLeft(currentShape, x); // Привязываем фигуру к координате X на холсте
                Canvas.SetTop(currentShape, y); // Привязываем фигуру к координате Y на холсте
            }
        }

        // Обработчик отпускания левой кнопки мыши (завершение рисования)
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing) // Если мы не находились в режиме рисования
                return; // Прерываем выполнение метода

            isDrawing = false; // Отключаем режим рисования
            DrawingCanvas.ReleaseMouseCapture(); // Освобождаем захват мыши холстом
            currentShape = null; // Сбрасываем ссылку на фигуру (текущая фигура завершена)
        }

        // Обработчик клика по кнопке очистки холста
        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear(); // Удаляем все нарисованные объекты (дочерние элементы) с холста
        }
    } 
} 
