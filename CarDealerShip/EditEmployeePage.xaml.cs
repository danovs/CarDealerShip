using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class EditEmployeePage : Page
    {
        private CarDealershipEntities db;

        public EditEmployeePage()
        {
            InitializeComponent();
            db = new CarDealershipEntities();

            cmbRole.ItemsSource = db.roles.ToList();
            cmbRole.DisplayMemberPath = "role_name";
            cmbRole.SelectedValuePath = "role_id";

            cmbUsername.ItemsSource = db.users.Select(u => u.username).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите добавить нового сотрудника в систему?", "Добавление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    role selectedRole = cmbRole.SelectedItem as role;
                    string selectedUsername = cmbUsername.SelectedItem as string;

                    if (selectedRole != null)
                    {
                        var selectedUser = db.users.FirstOrDefault(u => u.username == selectedUsername);

                        if (selectedUser != null)
                        {
                            if (string.IsNullOrWhiteSpace(txtSurname.Text) ||
                                string.IsNullOrWhiteSpace(txtName.Text) ||
                                string.IsNullOrWhiteSpace(txtLastname.Text) ||
                                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                                string.IsNullOrWhiteSpace(txtEmail.Text))
                            {
                                MessageBox.Show("Пожалуйста, заполните все поля.");
                                return;
                            }

                            employee newEmployee = new employee
                            {
                                user_id = selectedUser.user_id,
                                surname = txtSurname.Text,
                                name = txtName.Text,
                                lastname = txtLastname.Text,
                                phone = txtPhone.Text,
                                email = txtEmail.Text,
                                hiredate = DateTime.Now
                            };
                            db.employees.Add(newEmployee);
                            int changesSaved = db.SaveChanges();

                            if (changesSaved > 0)
                            {
                                selectedUser.role_id = selectedRole.role_id;
                                db.SaveChanges();
                                MessageBox.Show("Сотрудник был добавлен в систему!");
                            }
                            else
                            {
                                MessageBox.Show("Изменения не были внесены. Не удалось сохранить данные.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден в базе данных.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите роль для нового сотрудника.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Сотрудник не добавлен в систему", "Отмена добавления", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^a-zA-Zа-яА-Я]+"); // Разрешены только буквы
            return !regex.IsMatch(text);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Запретить пробел
            }
        }

        private void TextBox_PreviewTextInputForSalary(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedForSalary(e.Text);
        }

        private static bool IsTextAllowedForSalary(string text)
        {
            Regex regex = new Regex("[^0-9]+"); // Разрешены только цифры
            return !regex.IsMatch(text);
        }

        private void TextBox_PreviewKeyDownForSalary(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Запретить пробел
            }
        }
    }
}