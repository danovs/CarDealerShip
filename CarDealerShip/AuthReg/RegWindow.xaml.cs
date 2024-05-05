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
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Runtime.Remoting.Messaging;

namespace CarDealerShip.AuthReg
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        private readonly CarDealerShipEntities db;
        
        public RegWindow()
        {
            InitializeComponent();
            db = new CarDealerShipEntities();
            
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
                textLogin.Visibility = Visibility.Visible;
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
            // Проверка длины логина
            if (txtLogin.Text.Length < 8)
            {
                MessageBox.Show("Логин должен содержать минимум 8 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка длины пароля
            if (txtPassword.Password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка длины логина (максимальная длина)
            if (txtLogin.Text.Length > 50)
            {
                MessageBox.Show("Логин не может содержать больше 50 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка длины номера телефона
            string phoneNumber = txtPhoneNumber.Text;
            string cleanedPhoneNumber = new String(phoneNumber.Where(char.IsDigit).ToArray());
            if (cleanedPhoneNumber.Length != 11)
            {
                MessageBox.Show("Номер телефона должен содержать 11 цифр", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если все проверки пройдены, открываем главное окно (MainWindow)

            try
            {
                int userRoleID = GetUserRoleID("Пользователь");

                User newUser = new User
                {
                    Username = txtLogin.Text,
                    Password = txtPassword.Password,
                    UserNumber = phoneNumber,
                    RoleID = userRoleID
                };

                db.Users.Add(newUser);

                db.SaveChanges();

                MessageBox.Show("Вы успешно зарегистрированы!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (DbEntityValidationException ex)
            {
                // Отображение подробной информации об ошибках проверки сущностей
                foreach (var validationError in ex.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
                {
                    MessageBox.Show($"Ошибка при валидации данных: {validationError.ErrorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (DbUpdateException ex)
            {
                // Отображение общей информации о проблеме при сохранении данных
                MessageBox.Show($"Ошибка при сохранении данных: {ex.InnerException?.Message ?? ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private int GetUserRoleID(string roleName)
        {
            Role role = db.Roles.FirstOrDefault(r => r.RoleName == roleName);
            if (role != null)
            {
                return role.RoleID;
            }
            else
            {
                throw new Exception($"Роль '{roleName}' не найдена в таблице Roles.");
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
            Login login = new Login();
            this.Close();
            login.Show();
        }
    }
}
