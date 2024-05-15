﻿using System;
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

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private CarDealershipEntities db;
        
        public CatalogPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadCarCards();

            minPriceTextBox.TextChanged += FilterCarsByPrice;
            maxPriceTextBox.TextChanged += FilterCarsByPrice;
        }

        private void LoadCarCards()
        {
            carCardsContainer.Children.Clear();

            var catalogEntries = db.catalogs.ToList();

            foreach (var catalogEntry in catalogEntries)
            {
                var carCard = CreateCarCard(catalogEntry);
                carCardsContainer.Children.Add(carCard);
            }
        }

        private Border CreateCarCard(catalog catalogEntry)
        {
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

            StackPanel stackPanel = new StackPanel();

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

            Image carImage = new Image();
            carImage.Source = LoadImageFromBytes(catalogEntry.car.photo);
            carImage.Stretch = Stretch.Uniform;
            carImage.MaxWidth = 253;
            carImage.MaxHeight = 190;
            carImage.Margin = new Thickness(0, 10, 0, 10);

            Border topBorder = new Border();
            topBorder.BorderBrush = Brushes.Gray;
            topBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            topBorder.Margin = new Thickness(0, 10, 0, 10); // Отступы сверху
            topBorder.Opacity = 0.5;

            Border bottomBorder = new Border();
            bottomBorder.BorderBrush = Brushes.Gray;
            bottomBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            bottomBorder.Margin = new Thickness(0, 10, 0, 10); // Отступы сверху
            bottomBorder.Opacity = 0.5;

            TextBlock priceInfo = new TextBlock();
            priceInfo.Text = $"{catalogEntry.car.price:C0}";
            priceInfo.Style = (Style)FindResource("textMarkModel");

            TextBlock statusInfo = new TextBlock();
            statusInfo.Text = $"{catalogEntry.inventory.status.status_name}";
            statusInfo.Style = (Style)FindResource("textMarkModel");

            Button buyButton = new Button();
            buyButton.Content = "Купить";
            buyButton.Style = (Style)FindResource("BuyButton");
            buyButton.Click += (sender, e) =>
            {
                // Создаем экземпляр PurchasePage
                OrderPage orderPage = new OrderPage();

                // Получаем объект NavigationService текущей страницы
                NavigationService navigationService = NavigationService.GetNavigationService(this);

                // Переходим на страницу PurchasePage
                if (navigationService != null)
                {
                    navigationService.Navigate(orderPage);
                }
            };

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

            cardContainer.Child = stackPanel;

            return cardContainer;

        }

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
            return image;
        }

        private void FilterCarsByPrice(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(minPriceTextBox.Text, out int minPrice) && int.TryParse(maxPriceTextBox.Text, out int maxPrice))
            {
                var FilterEntries = db.catalogs.Where(catalogEntry => catalogEntry.car.price >= minPrice && catalogEntry.car.price <= maxPrice).ToList();

                carCardsContainer.Children.Clear();

                foreach (var catalogEntry in FilterEntries)
                {
                    var carCard = CreateCarCard(catalogEntry);
                    carCardsContainer.Children.Add(carCard);
                }
            }
            else
            {
                LoadCarCards();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && (textBox.Text == "цена от" || textBox.Text == "цена до"))
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
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
