using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Задание_4
{
    public partial class MainWindow : Window
    {
        private double zoom = 1.0; // Текущий коэффициент масштабирования изображения (1.0 = 100%)
        private BitmapSource originalImage; // Хранилище для загруженного оригинального изображения

        public MainWindow()
        {
            InitializeComponent(); // Инициализация элементов пользовательского интерфейса из XAML
        }

        // Обработчик клика по кнопке "Открыть изображение"
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и настраиваем стандартное диалоговое окно для выбора графических файлов
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Выберите изображение", // Заголовок окна диалога
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff" // Маска поддерживаемых форматов
            };

            // Если пользователь выбрал файл изображения и нажал кнопку подтверждения
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(); // Создаем новый объект для работы с картинкой

                    bitmap.BeginInit(); // Начинаем процесс инициализации свойств объекта BitmapImage
                    bitmap.UriSource = new Uri(dialog.FileName); // Указываем путь к выбранному на диске файлу
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Кэшируем изображение в память сразу, чтобы закрыть доступ к файлу на диске
                    bitmap.EndInit(); // Завершаем инициализацию графического объекта
                    bitmap.Freeze(); // «Замораживаем» объект для повышения производительности и безопасного доступа из разных потоков

                    originalImage = bitmap; // Сохраняем загруженную картинку в переменную класса
                    PreviewImage.Source = originalImage; // Отображаем загруженное изображение в элементе Image на форме

                    zoom = 1.0; // Сбрасываем текущий масштаб до исходных 100%
                    UpdateZoom(); // Вызываем метод для применения дефолтного масштаба в интерфейсе

                    // Выводим имя открытого файла в строку состояния
                    StatusText.Text = $"Файл: {System.IO.Path.GetFileName(dialog.FileName)}";
                }
                catch (Exception ex) // Перехватываем ошибки, если файл поврежден или имеет неверный формат
                {
                    // Показываем пользователю окно с описанием возникшей графической ошибки
                    MessageBox.Show(
                        "Не удалось открыть изображение:\n" + ex.Message,
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Обработчик клика по кнопке "Увеличить"
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null) // Если изображение еще не загружено
                return; // Прерываем выполнение метода

            zoom = Math.Min(zoom * 1.25, 5.0); // Увеличиваем масштаб на 25%, но ограничиваем его максимальным порогом в 500% (5.0)
            UpdateZoom(); // Применяем новые размеры к картинке
        }

        // Обработчик клика по кнопке "Уменьшить"
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null) // Если изображение еще не загружено
                return; // Прерываем выполнение метода

            zoom = Math.Max(zoom / 1.25, 0.1); // Уменьшаем масштаб на 25%, но ограничиваем его минимальным порогом в 10% (0.1)
            UpdateZoom(); // Применяем новые размеры к картинке
        }

        // Обработчик клика по кнопке сброса масштаба до исходных 100%
        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null) // Если изображение еще не загружено
                return; // Прерываем выполнение метода

            zoom = 1.0; // Устанавливаем масштаб ровно в 100%
            UpdateZoom(); // Применяем изменения на экране
        }

        // Обработчик клика по кнопке автоматической подгонки картинки под размеры окна просмотра
        private void FitImage_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null) // Если изображение еще не загружено
                return; // Прерываем выполнение метода

            double availableWidth = ImageScrollViewer.ViewportWidth; // Получаем текущую доступную ширину области просмотра компонента ScrollViewer
            double availableHeight = ImageScrollViewer.ViewportHeight; // Получаем текущую доступную высоту области просмотра компонента ScrollViewer

            if (availableWidth <= 0 || availableHeight <= 0) // Если размеры области просмотра некорректны (например, окно свернуто)
                return; // Прерываем выполнение метода

            double scaleX = availableWidth / originalImage.PixelWidth; // Вычисляем коэффициент масштабирования по горизонтали
            double scaleY = availableHeight / originalImage.PixelHeight; // Вычисляем коэффициент масштабирования по вертикали

            zoom = Math.Min(scaleX, scaleY); // Берем наименьший коэффициент, чтобы изображение полностью поместилось в окно без обрезки
            zoom = Math.Max(0.1, Math.Min(zoom, 5.0)); // Дополнительно удерживаем полученный зум в рамках разрешенных лимитов от 10% до 500%

            UpdateZoom(); // Применяем рассчитанный масштаб к изображению
        }

        // Вспомогательный метод для обновления масштаба и изменения текста на экране
        private void UpdateZoom()
        {
            ImageScale.ScaleX = zoom; // Задаем коэффициент трансформации элемента Image по оси X
            ImageScale.ScaleY = zoom; // Задаем коэффициент трансформации элемента Image по оси Y

            ZoomText.Text = $"Масштаб: {zoom * 100:F0}%"; // Форматируем и выводим текущий процент зума без знаков после запятой
        }
    }
}
