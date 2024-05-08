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
using System.Configuration;
using BCrypt.Net;
using System.Windows.Controls.Primitives;
using System.Data.Entity;

namespace CarDealerShip.AuthReg
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {

        private readonly CarDealershipEntities db;
        
        public RegWindow()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            
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
            try
            {
                if (txtLogin.Text.Length < 8 || txtPassword.Password.Length < 8)
                {
                    MessageBox.Show("Логин и пароль должны содержать минимум 8 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtLogin.Text.Length > 50)
                {
                    MessageBox.Show("Логин не может содержать больше 50 символов!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание нового пользователя
                var newUser = new user
                {
                    username = txtLogin.Text,
                    password_hash = PasswordHashMethod(txtPassword.Password),
                    role_id = 3 // 3 - ID роли "client"
                };

                db.users.Add(newUser);
                db.SaveChanges();

                // Создание новой записи клиента
                var newClient = new client
                {
                    user_id = newUser.user_id,
                    full_name = "",
                    phone = ""
                };

                db.clients.Add(newClient);
                db.SaveChanges();
                ((App)Application.Current).SetCurrentUserId(newUser.user_id);


                MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрываем текущее окно и открываем окно входа
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (DbEntityValidationException ex)
            {
                // Обработка ошибок проверки сущностей в Entity Framework
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Ошибка при проверке сущности: {validationError.ErrorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                // Обработка ошибок обновления базы данных
                MessageBox.Show($"Ошибка при обновлении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Общая обработка других исключений
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private string PasswordHashMethod(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
