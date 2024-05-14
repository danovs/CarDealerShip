using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Panels;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
        }


        private void CreateCarCard(car selectedCar)
        {
            if (selectedCar != null)
            {
                Border carCard = new Border();
                carCard.Background = Brushes.White;
                carCard.CornerRadius = new CornerRadius(5);
                carCard.Margin = new Thickness(5);
                carCard.Padding = new Thickness(10);
                carCard.Width = 300;

                DropShadowEffect dropShadow = new DropShadowEffect();
                dropShadow.Color = Colors.Gray;
                dropShadow.Direction = 270;
                dropShadow.ShadowDepth = 5;
                dropShadow.Opacity = 0.15;
                carCard.Effect = dropShadow;

                StackPanel stackPanel = new StackPanel();

                TextBlock brandModelTextBlock = new TextBlock();
                brandModelTextBlock.Text = $"{selectedCar.make} {selectedCar.model}";
                brandModelTextBlock.Style = FindResource("textMarkModel") as Style;
                stackPanel.Children.Add(brandModelTextBlock);

                TextBlock specsTextBlock1 = new TextBlock();
                specsTextBlock1.Text = selectedCar.trim_level;
                specsTextBlock1.Style = FindResource("textSpecs") as Style;
                stackPanel.Children.Add(specsTextBlock1);

                TextBlock specsTextBlock2 = new TextBlock();
                specsTextBlock2.Text = selectedCar.modification;
                specsTextBlock2.Style = FindResource("textSpecs") as Style;
                stackPanel.Children.Add(specsTextBlock2);

                Image carImage = new Image();
                carImage.Stretch = Stretch.Uniform;
                carImage.Width = 280; // Ширина изображения в карточке
                carImage.Height = 180; // Высота изображения в карточке

                // Получаем информацию о наличии из базы данных (например, по car_id)
                using (var db = new CarDealershipEntities())
                {
                    var inventoryInfo = db.inventories.FirstOrDefault(inv => inv.car_id == selectedCar.car_id);
                    if (inventoryInfo != null)
                    {
                        if (inventoryInfo.count > 0)
                        {
                            // Автомобиль доступен на складе
                            carImage.Source = LoadImage(selectedCar.photo); // Загружаем изображение автомобиля

                            TextBlock availabilityTextBlock = new TextBlock();
                            availabilityTextBlock.Text = "Наличие: В наличии";
                            availabilityTextBlock.FontSize = 16;
                            availabilityTextBlock.Style = FindResource("textMarkModel") as Style;
                            stackPanel.Children.Add(availabilityTextBlock);
                        }
                        else
                        {
                            // Автомобиль отсутствует на складе
                            carImage.Source = new BitmapImage(new Uri("/MenuImages/default_car_image.jpg", UriKind.Relative));

                            TextBlock availabilityTextBlock = new TextBlock();
                            availabilityTextBlock.Text = "Наличие: Нет в наличии";
                            availabilityTextBlock.FontSize = 16;
                            availabilityTextBlock.Style = FindResource("textMarkModel") as Style;
                            stackPanel.Children.Add(availabilityTextBlock);
                        }
                    }
                }

                stackPanel.Children.Add(carImage);

                TextBlock priceTextBlock = new TextBlock();
                priceTextBlock.Text = $"Цена: {selectedCar.price}";
                priceTextBlock.FontSize = 16;
                priceTextBlock.Style = FindResource("textMarkModel") as Style;
                stackPanel.Children.Add(priceTextBlock);

                Button buyButton = new Button();
                buyButton.Content = "Купить";
                buyButton.Style = FindResource("BuyButton") as Style;
                stackPanel.Children.Add(buyButton);

                carCard.Child = stackPanel;

                wrapPanel.Children.Add(carCard); // Добавляем карточку в wrapPanel (или другой контейнер)
            }
        }
        public static BitmapImage LoadImage(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

    }
}
