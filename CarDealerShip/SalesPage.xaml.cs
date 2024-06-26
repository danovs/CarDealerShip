﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private CarDealershipEntities db;
        private bool isSearchPlaceholder = true;
        public SalesPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadSalesData();

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void LoadSalesData()
        {
            var salesData = from sale in db.sales
                            join car in db.cars on sale.car_id equals car.car_id
                            join employee in db.employees on sale.employee_id equals employee.employee_id
                             join client in db.clients on sale.client_id equals client.client_id
                             join saleStatus in db.sale_statuses on sale.sale_status_id equals saleStatus.sale_status_id
                             select new
                             {
                                 SaleId = sale.sale_id,
                                 EmployeeName = String.Concat(employee.name, " ", employee.surname),
                                 ClientName = client.full_name,
                                 CarName = String.Concat(car.make, " ", car.model),
                                 CarFeatures = String.Concat(car.trim_level, " ", car.modification),
                                 Color = car.color,
                                 Price = sale.sale_price,
                                 Status = saleStatus.sale_status_name,
                                 SaleDate = sale.sale_date
                             };

            DGridSales.ItemsSource = salesData.ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DGridSales.SelectedItem != null)
            {
                dynamic selectedSale = DGridSales.SelectedItem;

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту запись продажи?", "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int saleId = selectedSale.SaleId;
                        var saleToRemove = db.sales.FirstOrDefault(s => s.sale_id == saleId);

                        if (saleToRemove != null)
                        {
                            db.sales.Remove(saleToRemove);
                            db.SaveChanges();
                            MessageBox.Show("Запись продажи успешно удалена.");
                            LoadSalesData();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти запись продажи для удаления.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении записи.\n" +
                            "Скорее всего, запись где-то используется");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Поиск" && db != null)
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();
                var searchTerms = searchText.Split(' ');

                var salesData = from sale in db.sales
                                join car in db.cars on sale.car_id equals car.car_id
                                join employee in db.employees on sale.employee_id equals employee.employee_id
                                join client in db.clients on sale.client_id equals client.client_id
                                join saleStatus in db.sale_statuses on sale.sale_status_id equals saleStatus.sale_status_id
                                where searchTerms.Any(term =>
                                        employee.name.ToLower().Contains(term) ||
                                        employee.surname.ToLower().Contains(term) ||
                                        client.full_name.ToLower().Contains(term) ||
                                        car.make.ToLower().Contains(term) ||
                                        car.model.ToLower().Contains(term) ||
                                        car.trim_level.ToLower().Contains(term) ||
                                        car.modification.ToLower().Contains(term) ||
                                        car.color.ToLower().Contains(term) ||
                                        sale.sale_price.ToString().Contains(term) ||
                                        saleStatus.sale_status_name.ToLower().Contains(term) ||
                                        sale.sale_date.ToString().ToLower().Contains(term)
                                    )
                                select new
                                {
                                    SaleId = sale.sale_id,
                                    EmployeeName = String.Concat(employee.name, " ", employee.surname),
                                    ClientName = client.full_name,
                                    CarName = String.Concat(car.make, " ", car.model),
                                    CarFeatures = String.Concat(car.trim_level, " ", car.modification),
                                    Color = car.color,
                                    Price = sale.sale_price,
                                    Status = saleStatus.sale_status_name,
                                    SaleDate = sale.sale_date
                                };

                DGridSales.ItemsSource = salesData.ToList();
            }
            else if (db != null)
            {
                LoadSalesData();
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
