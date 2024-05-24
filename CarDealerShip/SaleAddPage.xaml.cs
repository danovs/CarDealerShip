using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class SaleAddPage : Page
    {
        private CarDealershipEntities db;

        public SaleAddPage()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            LoadData();
        }

        private void LoadData()
        {
            var currentEmployee = db.employees.FirstOrDefault(e => e.user_id == ((App)Application.Current).CurrentUserId);

            if (currentEmployee != null)
            {
                txtEmployee.Text = $"{currentEmployee.surname} {currentEmployee.name} {currentEmployee.lastname}";
            }

            var appointments = db.appointments.ToList();
            cmbOrder.ItemsSource = appointments;
            cmbOrder.DisplayMemberPath = "appointment_id";
            cmbOrder.SelectedValuePath = "appointment_id";

            var saleStatuses = db.sale_statuses.ToList();
            cmbStatus.ItemsSource = saleStatuses;
            cmbStatus.DisplayMemberPath = "sale_status_name";
            cmbStatus.SelectedValuePath = "sale_status_id";
        }

        private void cmbOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbOrder.SelectedItem != null)
            {
                var selectedAppointment = (appointment)cmbOrder.SelectedItem;
                var selectedCar = db.cars.FirstOrDefault(c => c.car_id == selectedAppointment.car_id);
                var selectedClient = db.clients.FirstOrDefault(c => c.client_id == selectedAppointment.client_id);

                if (selectedAppointment != null)
                {
                    var client = db.clients.FirstOrDefault(c => c.client_id == selectedAppointment.client_id);
                    if (client != null)
                    {
                        txtClient.Text = client.full_name;
                    }

                    var car = db.cars.FirstOrDefault(c => c.car_id == selectedAppointment.car_id);
                    if (car != null)
                    {
                        txtCar.Text = car.make;
                        txtModel.Text = car.model;
                        txtYear.Text = car.year.ToString();
                        txtColor.Text = car.color;
                        txtTrimLevel.Text = car.trim_level;
                        txtModification.Text = car.modification;
                        txtPrice.Text = car.price.ToString();

                        var carType = db.car_types.FirstOrDefault(ct => ct.type_id == car.type_id);
                        if (carType != null)
                        {
                            txtBodyType.Text = carType.type_name;
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите добавить запись о продаже автомобиля?", "Добавление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (cmbOrder.SelectedItem == null || cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите заказ и статус продажи.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var selectedAppointment = (appointment)cmbOrder.SelectedItem;
                    var currentEmployee = db.employees.FirstOrDefault(em => em.user_id == ((App)Application.Current).CurrentUserId);

                    decimal salePrice;
                    if (!decimal.TryParse(txtPrice.Text, out salePrice))
                    {
                        MessageBox.Show("Введите корректную цену на продажу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var inventoryItem = db.inventories.FirstOrDefault(i => i.car_id == selectedAppointment.car_id);
                    if (inventoryItem != null)
                    {
                        // Проверка, если количество 0, нельзя добавить запись о продаже
                        if (inventoryItem.count == 0)
                        {
                            MessageBox.Show("Количество автомобилей равно нулю. Нельзя добавить запись о продаже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Проверка, если автомобиль не находится в автосалоне
                        if (inventoryItem.location_id != 1)
                        {
                            MessageBox.Show("Нельзя продать автомобиль, который находится не в автосалоне.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        inventoryItem.count -= 1;
                        if (inventoryItem.count < 0)
                        {
                            inventoryItem.count = 0; // Предотвращаем отрицательное значение
                        }

                        // Обновляем статус автомобиля на "Нет в наличии" если количество 0
                        if (inventoryItem.count == 0)
                        {
                            var outOfStockStatus = db.status.FirstOrDefault(s => s.status_name == "Нет в наличии");
                            if (outOfStockStatus != null)
                            {
                                inventoryItem.status_id = outOfStockStatus.status_id;
                            }
                        }
                    }

                    var newSales = new sale
                    {
                        client_id = selectedAppointment.client_id,
                        employee_id = currentEmployee.employee_id,
                        car_id = selectedAppointment.car_id,
                        sale_date = DateTime.Now,
                        sale_price = salePrice,
                        sale_status_id = (int)cmbStatus.SelectedValue
                    };
                    db.sales.Add(newSales);

                    var salesCount = db.sales_counts.FirstOrDefault(sc => sc.employee_id == currentEmployee.employee_id);
                    if (salesCount != null)
                    {
                        salesCount.Sales_count += 1;
                        salesCount.total_sales += salePrice;
                    }
                    else
                    {
                        db.sales_counts.Add(new sales_counts
                        {
                            employee_id = currentEmployee.employee_id,
                            Sales_count = 1,
                            total_sales = salePrice
                        });
                    }

                    db.SaveChanges();
                    MessageBox.Show("Продажа успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Запись не была добавлена в систему");
            }
        }
    }
}