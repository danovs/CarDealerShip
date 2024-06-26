﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class EditCurrentEmployeePage : Page
    {
        // Экземпляр контекста базы данных. Текущий сотрудник для редактирования.
        private CarDealershipEntities db;
        private employee currentEmployee;

        // Инициализация контекста базы данных. Поиск сотрудника по идентификатору. Установка источников данных для выпадающих списков cmbRole и cmbUsername.
        public EditCurrentEmployeePage(employee employee)
        {
            InitializeComponent();

            db = new CarDealershipEntities();
            if (employee is employee)
            {
                currentEmployee = db.employees.Find(employee.employee_id);

                cmbRole.ItemsSource = db.roles.Select(r => r.role_name).ToList();
                cmbUsername.ItemsSource = db.users.Select(u => u.username).ToList();

                // Заполнение полей формы данными текущего сотрудника.
                txtSurname.Text = currentEmployee.surname;
                txtName.Text = currentEmployee.name;
                txtLastname.Text = currentEmployee.lastname;
                txtPhone.Text = currentEmployee.phone;
                txtEmail.Text = currentEmployee.email;
                cmbRole.SelectedItem = currentEmployee.user.role.role_name; // Выбранная роль сотрудника
                cmbUsername.SelectedItem = currentEmployee.user.username; // Выбранное имя пользователя
                txtSalary.Text = currentEmployee.salary.ToString(); // Заработная плата
            }
            else
            {
                MessageBox.Show("Ошибка: Некорректный тип объекта передан в конструктор EditCurrentEmployeePage", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "Сохранить".
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения действия пользователя.
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить обновленные данные сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверка наличия и заполненности всех обязательных полей.
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

                    // Проверка наличия изменений.
                    if (!CheckForChanges())
                    {
                        MessageBox.Show("Нет изменений для сохранения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Обновление данных сотрудника.
                    currentEmployee.surname = txtSurname.Text;
                    currentEmployee.name = txtName.Text;
                    currentEmployee.lastname = txtLastname.Text;
                    currentEmployee.phone = txtPhone.Text;
                    currentEmployee.email = txtEmail.Text;

                    // Проверка и преобразование заработной платы.
                    if (decimal.TryParse(txtSalary.Text, out decimal salaryValue))
                    {
                        currentEmployee.salary = salaryValue;
                    }
                    else
                    {
                        MessageBox.Show("Некорректный формат заработной платы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получение выбранной роли и обновление роли сотрудника.
                    string selectedRoleName = cmbRole.SelectedItem as string;
                    var newRole = db.roles.FirstOrDefault(r => r.role_name == selectedRoleName);

                    if (newRole != null)
                    {
                        currentEmployee.user.role = newRole; // Установка новой роли сотруднику.
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

        // Метод для проверки наличия изменений.
        private bool CheckForChanges()
        {
            return currentEmployee.surname != txtSurname.Text ||
                   currentEmployee.name != txtName.Text ||
                   currentEmployee.lastname != txtLastname.Text ||
                   currentEmployee.phone != txtPhone.Text ||
                   currentEmployee.email != txtEmail.Text ||
                   currentEmployee.user.role.role_name != (string)cmbRole.SelectedItem ||
                   currentEmployee.salary != Convert.ToDecimal(txtSalary.Text);
        }

        // Обработчик для проверки ввода только букв
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Zа-яА-Я]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void SalaryTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Разрешаем только цифры
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
