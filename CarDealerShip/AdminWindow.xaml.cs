using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            FrameManger.AdminFrame = AdminFrame;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new EmployeesPage());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CarsPage());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new Inventory());
        }

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

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CatalogPanel());
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new OrderList());
        }
    }
}
