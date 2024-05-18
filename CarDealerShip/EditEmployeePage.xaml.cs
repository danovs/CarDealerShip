using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class EditEmployeePage : Page
    {
        // Экземпляр контекста базы данных
        private CarDealershipEntities db;

        // Инициализация контекста базы данных. Загрузка списка ролей из БД для выпадаюшего списка cmbRole.
        public EditEmployeePage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            cmbRole.ItemsSource = db.roles.ToList();
            cmbRole.DisplayMemberPath = "role_name"; // Устанавливаем свойство отображения элементов списка
            cmbRole.SelectedValuePath = "role_id"; // Устанавливаем свойство значения элементов списка

            // Подгружаем имена пользователей в выпадающий список cmbUsername.
            cmbUsername.ItemsSource = db.users.Select(u => u.username).ToList();
        }

        // Кнопка "Добавить"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения действия пользователя.
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите добавить нового сотрудника в систему?", "Добавление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Получение выбранной роли и имени пользователя из выпадающих списков.
                    role selectedRole = cmbRole.SelectedItem as role;
                    string selectedUsername = cmbUsername.SelectedItem as string;

                    // Проверка наличия выбранной роли и имени пользователя.
                    if (selectedRole != null)
                    {
                        var selectedUser = db.users.FirstOrDefault(u => u.username == selectedUsername);

                        if (selectedUser != null)
                        {
                            // Проверка наличия и заполненности всех полей для создания сотрудника.
                            if (string.IsNullOrWhiteSpace(txtSurname.Text) ||
                                string.IsNullOrWhiteSpace(txtName.Text) ||
                                string.IsNullOrWhiteSpace(txtLastname.Text) ||
                                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                                string.IsNullOrWhiteSpace(txtEmail.Text))
                            {
                                MessageBox.Show("Пожалуйста, заполните все поля.");
                                return;
                            }

                            // Создание нового сотрудника и его добавление в БД.
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

                            // Проверка успешности сохранения изменений.
                            if (changesSaved > 0)
                            {
                                // Изменение роли пользователя на выбранную.
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
    }
}