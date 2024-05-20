using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class EmployeesPage : Page
    {
        // Экземпляр контекста базы данных
        private CarDealershipEntities db;
        private bool isSearchPlaceholder = true;

        // Инициализация контекста базы данных и загрузка данных сотрудников из БД.
        public EmployeesPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            if (db != null )
            {
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
                LoadEmployeeData();
            }

        }

        // Метод для загрузки данных сотрудников из базы данных.
        private void LoadEmployeeData()
        {
            // Запрос к базе данных для получения данных о сотрудниках и их ролях (Данные поля будут задействованы в привязке в датагриде XAML)
            var query = (from employee in db.employees
                         join user in db.users on employee.user_id equals user.user_id
                         join role in db.roles on user.role_id equals role.role_id
                         select new
                         {
                             EmployeeId = employee.employee_id,
                             UserId = employee.user_id,
                             HireDate = employee.hiredate,
                             RoleName = role.role_name,
                             Surname = employee.surname,
                             Name = employee.name,
                             LastName = employee.lastname,
                             Phone = employee.phone,
                             Email = employee.email,
                             Salary = employee.salary
                         });

            // Установка источника данных для DataGrid на основе результата запроса.
            DGridEmployees.ItemsSource = query.ToList();
        }

        // Кнопка "Добавить".
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            FrameManger.AdminFrame.Navigate(new EditEmployeePage());
        }

        // Кнопка "Удалить".
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Проверка выбранного элемента в DataGrid
            if (DGridEmployees.SelectedItem != null)
            {
                // Преобразование выбранного элемента в тип employee
                employee selectedEmployee = DGridEmployees.SelectedItem as employee;

                // Проверка подтверждения действия пользователя
                MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить данного сотрудника?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (MessageBoxResult.Yes == result)
                {
                    try
                    {
                        // Подключение выбранного сотрудника к текущему контексту данных.
                        db.employees.Attach(selectedEmployee);
                        db.employees.Remove(selectedEmployee);
                        db.SaveChanges();

                        // Обновление роли пользователя
                        var selectedUser = db.users.FirstOrDefault(u => u.user_id == selectedEmployee.user_id);
                        if (selectedUser != null)
                        {
                            selectedUser.role_id = 3; // 3 - ID роли "Клиент"
                            db.SaveChanges();
                        }

                        MessageBox.Show("Сотрудник успешно удален и его роль изменена.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении сотрудника: " + ex.Message);
                    }

                    // Обновление списка сотрудников после удаления
                    LoadEmployeeData();
                }
                else
                {
                    MessageBox.Show("Сотрудник не был удален", "Удаление пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.");
            }
        }

        // Кнопка "Изменить".
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение выбранного сотрудника из DataGrid
                var selectedEmployee = DGridEmployees.SelectedItem;

                // Проверка, что выбран сотрудник
                if (selectedEmployee != null)
                {
                    // Создание анонимного объекта
                    var employeeInfo = selectedEmployee as dynamic;

                    // Получение ID выбранного сотрудника
                    int employeeId = employeeInfo.EmployeeId;

                    // Получение сотрудника из базы данных по ID
                    var employee = db.employees.FirstOrDefault(emp => emp.employee_id == employeeId);

                    if (employee != null)
                    {
                        // Передача выбранного сотрудника на страницу редактирования
                        FrameManger.AdminFrame.Navigate(new EditCurrentEmployeePage(employee));
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти выбранного сотрудника в базе данных.");
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите сотрудника для редактирования.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Поиск" && db != null)
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();
                var searchTerms = searchText.Split(' ');

                var searchResult = db.employees
    .Where(employee =>
        searchTerms.All(term =>
            employee.surname.ToLower().Contains(term) ||
            employee.name.ToLower().Contains(term) ||
            employee.lastname.ToLower().Contains(term) ||
            employee.phone.ToLower().Contains(term) ||
            employee.email.ToLower().Contains(term) ||
            employee.salary.ToString().Contains(term) ||
            db.users.Any(user =>
                user.user_id == employee.user_id &&
                db.roles.Any(role =>
                    role.role_id == user.role_id &&
                    role.role_name.ToLower().Contains(term)
                )
            )
        )
    )
    .Select(employee => new
    {
        EmployeeId = employee.employee_id,
        HireDate = employee.hiredate,
        RoleName = db.roles.FirstOrDefault(role =>
            db.users.Any(user =>
                user.user_id == employee.user_id &&
                user.role_id == role.role_id
            )).role_name,
        Surname = employee.surname,
        Name = employee.name,
        LastName = employee.lastname,
        Phone = employee.phone,
        Email = employee.email,
        Salary = employee.salary
    })
    .ToList();

                DGridEmployees.ItemsSource = searchResult;
            }
            else if (db != null)
            {
                LoadEmployeeData();
            }
        }
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isSearchPlaceholder)
            {
                SearchTextBox.Text = "";
                isSearchPlaceholder = false;
            }
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск";
                isSearchPlaceholder = true;
            }
        }
    }
}