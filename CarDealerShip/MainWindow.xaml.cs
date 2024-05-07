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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
            FrameManger.MainFrame = MainFrame;
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

        private void Car_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new CatalogPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new HomePage());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new SettingProfile());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new OrderPage());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new FeedbackPage());
        }
    }
}
