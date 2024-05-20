using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class AdminWindow : Window
    {
        // Устанавливаем главный фрейм администратора для навигации.

        private readonly int userRole;
        public AdminWindow(int role)
        {
            InitializeComponent();
            userRole = role;
            SetMenuUIVisibility();
            FrameManger.AdminFrame = AdminFrame;
            
        }

        private void SetMenuUIVisibility()
        {
            if (userRole == 1)
            {
                Employees.Visibility = Visibility.Visible;
                Catalog.Visibility = Visibility.Visible;
                Inventory.Visibility = Visibility.Visible;
                AddCatalog.Visibility = Visibility.Visible;
                Feedback.Visibility = Visibility.Visible;
                Sales.Visibility = Visibility.Visible;
                SalesAdd.Visibility = Visibility.Collapsed;
                SalesCount.Visibility = Visibility.Visible;
                UsersReport.Visibility = Visibility.Visible;
            }
            else if (userRole == 2)
            {
                Employees.Visibility = Visibility.Collapsed;
                Catalog.Visibility = Visibility.Visible;
                Inventory.Visibility = Visibility.Visible;
                AddCatalog.Visibility = Visibility.Visible;
                Feedback.Visibility = Visibility.Visible;
                Sales.Visibility = Visibility.Collapsed;
                SalesAdd.Visibility = Visibility.Visible;
                SalesCount.Visibility = Visibility.Collapsed;
                UsersReport.Visibility = Visibility.Collapsed;
            }
        }

        // Кнопка "Закрытие приложения"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из приложения?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Мы рады, что вы продолжаете пользоваться нашим приложением! :)", "Спасибо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new SalesPage());
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new SaleAddPage());
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new SalesCount());
        }

        private void UsersReport_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new usersReports());
        }
    }
}