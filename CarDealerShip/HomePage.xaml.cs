using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        // Кнопка "Начнём". Производит переход на страницу "Каталог автомобилей".
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new CatalogPage());
        }
    }
}
