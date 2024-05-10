using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
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
    /// Логика взаимодействия для EditEmployeePage.xaml
    /// </summary>
    public partial class EditEmployeePage : Page
    {
        private CarDealershipEntities db;
        
        public EditEmployeePage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            cmbRole.ItemsSource = db.roles.Select(r => r.role_name).ToList();

            cmbUsername.ItemsSource = db.users.Select(u => u.username).ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите добавить нового сотрудника в систему?", "Добавление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (MessageBoxResult.Yes == result)
            {
                try
                {
                    string selectedRole = cmbRole.SelectedItem as string;
                    string selectedUsername = cmbUsername.SelectedItem as string;
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
                            return; // Останавливаем выполнение метода, если поля не заполнены
                        }

                        employee newEmployee = new employee
                        {
                            user_id = selectedUser.user_id,
                            role_id = (int)(db.roles.FirstOrDefault(r => r.role_name == selectedRole)?.role_id),
                            surname = txtSurname.Text,
                            name = txtName.Text,
                            lastname = txtLastname.Text,
                            phone = txtPhone.Text,
                            email = txtEmail.Text,
                            hiredate = DateTime.Now
                        };

                        db.employees.Add(newEmployee);
                        int changesSaved = db.SaveChanges(); // Сохраняем изменения и получаем количество измененных записей

                        if (changesSaved > 0)
                        {
                            // После успешного сохранения сотрудника обновляем role_id у пользователя
                            selectedUser.role_id = newEmployee.role_id;
                            db.SaveChanges(); // Сохраняем изменения в таблице users

                            MessageBox.Show("Сотрудник успешно добавлен!");
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
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Сотрудник не добавлен в систему", "Отмена добавления", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
