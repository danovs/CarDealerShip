using CarDealerShip.AuthReg;
using System.Windows;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class MainWindow : Window
    {
        
        // После успешной авторизации/регистрации производим переход на домашнюю страницу.
        // ВАЖНО: FrameManager - является классом для перехода на страницы. Если хотите внести изменения - откройте FrameManager.cs в обозревателе решений.
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
            FrameManger.MainFrame = MainFrame;
        }

        // Кнопка "Закрытие приложения"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из приложения?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Login loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Мы рады, что вы продолжаете пользоваться нашим приложением! :)", "Спасибо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Событие "MouseDown" для перемещения окна за его заголовок.
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) // Проверка нажатия ЛКМ. В случае, если ЛКМ зажат, можно перемещать окно.
            {
                this.DragMove();
            }
        }

        // Переход на страницу "Каталог автомобилей".
        private void Car_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new CatalogPage());
        }

        // Переход на страницу "Домашняя страница".
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new HomePage());
        }

        // Переход на страницу "Оформление заказа".
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new OrderPage());
        }

        // Переход на страницу "Настройка профиля".
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new SettingProfile());
        }

        // Переход на страницу "Отзыв".
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FrameManger.MainFrame.Navigate(new FeedbackPage());
        }
    }
}
