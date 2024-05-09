﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class EmployeesPage : Page
    {
        private CarDealershipEntities db;

        public EmployeesPage()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            DGridEmployees.ItemsSource = db.employees.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу редактирования с пустым сотрудником для добавления
            FrameManger.AdminFrame.Navigate(new EditEmployeePage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (DGridEmployees.SelectedItem != null)
            {
                employee selectedEmployee = DGridEmployees.SelectedItem as employee;

                try
                {
                    // Подключаем выбранного сотрудника к текущему контексту данных
                    db.employees.Attach(selectedEmployee);
                    db.employees.Remove(selectedEmployee);
                    db.SaveChanges();

                    // Обновляем роль пользователя
                    var selectedUser = db.users.FirstOrDefault(u => u.user_id == selectedEmployee.user_id);
                    if (selectedUser != null)
                    {
                        selectedUser.role_id = 3;
                        db.SaveChanges();
                    }

                    MessageBox.Show("Сотрудник успешно удален и его роль изменена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении сотрудника: " + ex.Message);
                }

                // Обновляем список сотрудников после удаления
                LoadEmployeeData();
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.");
            }
        }
    }
}