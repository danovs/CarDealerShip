using System;
using System.Windows;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class AdminWindow : Window
    {
        // Устанавливаем главный фрейм администратора для навигации.
        public AdminWindow()
        {
            InitializeComponent();
            FrameManger.AdminFrame = AdminFrame;
        }

        // Кнопка "Закрытие приложения"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Обработчик события нажатия мыши на границе окна для перемещения.
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Если нажата левая кнопка мыши, перемещаем окно.
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // Кнопка "Сотрудники"
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new EmployeesPage());
        }

        // Кнопка "Автомобили"
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CarsPage());
        }

        // Кнопка "Инвентарь"
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new Inventory());
        }

        // Кнопка "Отзывы"
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameManger.AdminFrame.Navigate(new FeedbackPageEA());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "Каталог"
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CatalogPanel());
        }

        // Кнопка "Заказы"
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new OrderList());
        }
    }
}