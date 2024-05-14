using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для EditCurrentEmployeePage.xaml
    /// </summary>
    public partial class EditCurrentEmployeePage : Page
    {
        private CarDealershipEntities db;

        private employee currentEmployee;

        public EditCurrentEmployeePage(employee employee)
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            currentEmployee = db.employees.Find(employee.employee_id);

            cmbRole.ItemsSource = db.roles.Select(r => r.role_name).ToList();
            cmbUsername.ItemsSource = db.users.Select(u => u.username).ToList();

            txtSurname.Text = currentEmployee.surname;
            txtName.Text = currentEmployee.name;
            txtLastname.Text = currentEmployee.lastname;
            txtPhone.Text = currentEmployee.phone;
            txtEmail.Text = currentEmployee.email;
            cmbRole.SelectedItem = currentEmployee.user.role.role_name;
            cmbUsername.SelectedItem = currentEmployee.user.username;
            txtSalary.Text = currentEmployee.salary.ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить обновленные данные сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Check for empty required fields
                    if (string.IsNullOrWhiteSpace(txtSurname.Text) ||
                        string.IsNullOrWhiteSpace(txtName.Text) ||
                        string.IsNullOrWhiteSpace(txtLastname.Text) ||
                        string.IsNullOrWhiteSpace(txtPhone.Text) ||
                        string.IsNullOrWhiteSpace(txtEmail.Text) ||
                        cmbRole.SelectedItem == null ||
                        cmbUsername.SelectedItem == null ||
                        string.IsNullOrWhiteSpace(txtSalary.Text))
                    {
                        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Update employee details
                    currentEmployee.surname = txtSurname.Text;
                    currentEmployee.name = txtName.Text;
                    currentEmployee.lastname = txtLastname.Text;
                    currentEmployee.phone = txtPhone.Text;
                    currentEmployee.email = txtEmail.Text;

                    if (decimal.TryParse(txtSalary.Text, out decimal salaryValue))
                    {
                        currentEmployee.salary = salaryValue;
                    }
                    else
                    {
                        MessageBox.Show("Некорректный формат заработной платы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Get selected role and update employee's role
                    string selectedRoleName = cmbRole.SelectedItem as string;

                    var newRole = db.roles.FirstOrDefault(r => r.role_name == selectedRoleName);

                    if (newRole != null)
                    {
                        currentEmployee.user.role = newRole;

                        db.SaveChanges();
                        MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Выбранная роль не найдена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Изменения не сохранены", "Отмена сохранения", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}