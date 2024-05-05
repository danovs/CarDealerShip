using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;


namespace CarDealerShip.AuthReg
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly CarDealerShipEntities db;
        public Login()
        {
            InitializeComponent();
            db = new CarDealerShipEntities();
        }

        private bool AuthUserCheck(string login, string password)
        {
            User user = db.Users.FirstOrDefault(u => u.Username == login && u.Password == password);
            return user!= null;
        }

        private void textLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtLogin.Focus();
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogin.Text) && txtLogin.Text.Length > 0)
            {
                textLogin.Visibility = Visibility.Collapsed;
            }
            else
            {
                textLogin.Visibility= Visibility.Visible;
            }

        }


        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        { 
            txtPassword.Focus();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text.Length < 8)
            {
                MessageBox.Show("Логин должен содержать минимум 8 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (txtPassword.Password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (txtLogin.Text.Length > 50)
            {
                MessageBox.Show("Логин не может содержать больше 50 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }

            string login = txtLogin.Text;
            string password = txtPassword.Password;

            if (AuthUserCheck(login, password))
            {
                MainWindow mainWindow = new MainWindow();
                this.Close();
                mainWindow.Show();

            }
            else
            {
                MessageBox.Show("Неверный логин или пароль. Попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
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
            RegWindow regWindow = new RegWindow();
            this.Close();
            regWindow.Show();
        }
    }
}
