using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CarDealerShip
{
    public partial class CatalogPage : Page
    {
        // Экземпляр контекста базы данных
        private CarDealershipEntities db;

        // Инициализация контекста базы данных, загрузка карточек автомобилей из базы данных.
        // Добавление обработчиков событий изменения текста в текстовых полях для фильтрации по цене
        public CatalogPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();
            LoadCarCards();

            minPriceTextBox.TextChanged += FilterCarsByPrice;
            maxPriceTextBox.TextChanged += FilterCarsByPrice;
        }

        // Метод для загрузки карточек автомобилей из базы данных.
        private void LoadCarCards()
        {
            carCardsContainer.Children.Clear(); // Очистка контейнера для карточек автомобилей.

            var catalogEntries = db.catalogs.ToList(); // Получение списка записей из каталога.

            // Создание карточек для каждой записи в каталоге.
            foreach (var catalogEntry in catalogEntries)
            {
                var carCard = CreateCarCard(catalogEntry); // Создание карточки для данной записи.
                carCardsContainer.Children.Add(carCard); // Добавление карточки в контейнер.
            }
        }

        // Метод для создания карточки автомобиля на основе записи из каталога.
        private Border CreateCarCard(catalog catalogEntry)
        {
            // Создание контейнера карточки.
            Border cardContainer = new Border();
            cardContainer.Background = Brushes.White;
            cardContainer.CornerRadius = new CornerRadius(5);
            cardContainer.Margin = new Thickness(5, 10, 5, 5);
            cardContainer.Padding = new Thickness(10);
            cardContainer.Width = 300;

            cardContainer.Effect = new DropShadowEffect
            {
                Color = Colors.LightGray,
                Direction = 270,
                ShadowDepth = 5,
                Opacity = 0.125
            };

            // Создание панели для размещения содержимого карточки.
            StackPanel stackPanel = new StackPanel();

            // Создание текстовых блоков с информацией об автомобиле.
            TextBlock carInfo = new TextBlock();
            carInfo.Text = $"{catalogEntry.car.make} {catalogEntry.car.model}";
            carInfo.Style = (Style)FindResource("textMarkModel");

            TextBlock trimLevelInfo = new TextBlock();
            trimLevelInfo.Text = $"{catalogEntry.car.trim_level}";
            trimLevelInfo.Style = (Style)FindResource("textSpecs");

            TextBlock modificationInfo = new TextBlock();
            modificationInfo.Text = $"{catalogEntry.car.modification}";
            modificationInfo.Style = (Style)FindResource("textSpecs");

            TextBlock colorInfo = new TextBlock();
            colorInfo.Text = $"{catalogEntry.car.color}";
            colorInfo.Style = (Style)FindResource("textSpecs");

            // Создание изображения автомобиля.
            Image carImage = new Image();
            carImage.Source = LoadImageFromBytes(catalogEntry.car.photo);
            carImage.Stretch = Stretch.Uniform;
            carImage.MaxWidth = 253;
            carImage.MaxHeight = 190;
            carImage.Margin = new Thickness(0, 10, 0, 10);

            // Создание горизонтальных линий для разделения секций карточки.
            Border topBorder = new Border();
            topBorder.BorderBrush = Brushes.Gray;
            topBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            topBorder.Margin = new Thickness(0, 10, 0, 10);
            topBorder.Opacity = 0.5;

            Border bottomBorder = new Border();
            bottomBorder.BorderBrush = Brushes.Gray;
            bottomBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            bottomBorder.Margin = new Thickness(0, 10, 0, 10);
            bottomBorder.Opacity = 0.5;

            // Создание текстовых блоков с информацией о цене и статусе автомобиля.
            TextBlock priceInfo = new TextBlock();
            priceInfo.Text = $"{catalogEntry.car.price:C0}";
            priceInfo.Style = (Style)FindResource("textMarkModel");

            TextBlock statusInfo = new TextBlock();
            statusInfo.Text = $"{catalogEntry.inventory.status.status_name}";
            statusInfo.Style = (Style)FindResource("textMarkModel");

            // Создание кнопки "Купить".
            Button buyButton = new Button();
            buyButton.Content = "Купить";
            buyButton.Style = (Style)FindResource("BuyButton");

            // Блокируем кнопку, если статус автомобиля - "Not Available"
            if (catalogEntry.inventory.status.status_name == "Нет в наличии")
            {
                buyButton.IsEnabled = false;
            }

            buyButton.Click += (sender, e) =>
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите выбрать данный автомобиль?", "Выбор автомобиля", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Кнопка "Купить".
                    OrderPage orderPage = new OrderPage();

                    // Передача информации об автомобиле на страницу оформления заказа/
                    orderPage.txtCarMakeAndModel.Text = $"{catalogEntry.car.make} {catalogEntry.car.model}";
                    orderPage.txtTrimLevelAndModification.Text = $"{catalogEntry.car.trim_level} {catalogEntry.car.modification}";
                    orderPage.txtColor.Text = $"{catalogEntry.car.color}";

                    NavigationService.Navigate(orderPage);
                }
                else
                {
                    MessageBox.Show("Вы можете выбрать другой автомобиль :)", "Выбор автомобиля", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            };

            // Добавление элементов в панель.
            stackPanel.Children.Add(carInfo);
            stackPanel.Children.Add(modificationInfo);
            stackPanel.Children.Add(trimLevelInfo);
            stackPanel.Children.Add(colorInfo);
            stackPanel.Children.Add(topBorder);
            stackPanel.Children.Add(carImage);
            stackPanel.Children.Add(bottomBorder);
            stackPanel.Children.Add(priceInfo);
            stackPanel.Children.Add(statusInfo);
            stackPanel.Children.Add(buyButton);

            // Установка панели в качестве содержимого контейнера карточки.
            cardContainer.Child = stackPanel;

            return cardContainer;
        }


        // Метод для загрузки изображения из массива байтов.
        private BitmapImage LoadImageFromBytes(byte[] imageData)
        {
            BitmapImage image = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageData))
            {
                stream.Position = 0;
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            }
            return image; // Возвращение загруженного изображения
        }

        // Метод для фильтрации автомобилей по цене.
        private void FilterCarsByPrice(object sender, TextChangedEventArgs e)
        {
            // Проверка на ввод числовых значений в поля цены.
            if (int.TryParse(minPriceTextBox.Text, out int minPrice) && int.TryParse(maxPriceTextBox.Text, out int maxPrice))
            {
                // Фильтрация записей из каталога по цене.
                var FilterEntries = db.catalogs.Where(catalogEntry => catalogEntry.car.price >= minPrice && catalogEntry.car.price <= maxPrice).ToList();

                // Очистка контейнера карточек.
                carCardsContainer.Children.Clear();

                // Создание карточек для отфильтрованных записей.
                foreach (var catalogEntry in FilterEntries)
                {
                    var carCard = CreateCarCard(catalogEntry);
                    carCardsContainer.Children.Add(carCard);
                }
            }
            else
            {
                LoadCarCards(); // Если введены некорректные данные, загружаем все карточки.
            }
        }

        // Обработчик события получения фокуса текстового поля. (Сделано для дизайна)
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Очистка текстового поля, если оно содержит текст по умолчанию.
            TextBox textBox = sender as TextBox;

            if (textBox != null && (textBox.Text == "цена от" || textBox.Text == "цена до"))
            {
                textBox.Text = string.Empty;
            }
        }

        // Обработчик события потери фокуса текстового поля. (Сдеално для дизайна)
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Восстановление текста по умолчанию, если текстовое поле осталось пустым.
            TextBox textBox = sender as TextBox;

            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "minPriceTextBox")
                {
                    textBox.Text = "цена от";
                }
                else if (textBox.Name == "maxPriceTextBox")
                {
                    textBox.Text = "цена до";
                }
            }
        }
    }
}
