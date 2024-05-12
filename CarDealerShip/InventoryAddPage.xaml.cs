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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для InventoryAddPage.xaml
    /// </summary>
    public partial class InventoryAddPage : Page
    {
        private CarDealershipEntities db;

        public InventoryAddPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            cmbCar.ItemsSource = db.cars.ToList();
            cmbCar.DisplayMemberPath = "make";
            cmbCar.SelectedValuePath = "car_id";

            cmbStatus.ItemsSource = db.status.ToList();

            cmbStatus.DisplayMemberPath = "status_name";
            cmbStatus.SelectedValuePath = "status_id";

            cmbLocation.ItemsSource = db.locations.ToList();
            cmbLocation.DisplayMemberPath = "location_name"; // Или другое подходящее свойство
            cmbLocation.SelectedValuePath = "location_id";


        }

        private void cmbCar_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCar.SelectedItem != null)
            {
                car selectedCar = (car)cmbCar.SelectedItem;

                txtMake.Text = selectedCar.make;
                txtModel.Text = selectedCar.model;
                txtYear.Text = selectedCar.year.ToString();
                txtColor.Text = selectedCar.color;
                txtModification.Text = selectedCar.modification;
                txtTrimLevel.Text = selectedCar.trim_level;

                if (selectedCar.type_id != null)
                {
                    var carType = db.car_types.FirstOrDefault(ct => ct.type_id == selectedCar.type_id);

                    if (carType != null)
                    {
                        txtBodyType.Text = carType.type_name;
                    }
                    else
                    {
                        txtBodyType.Text = "Незивестно";
                    }
                }
                else
                {
                    txtBodyType.Text = "Не указан";
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите добавить автомобиль в инвентарь?", "Добавление автомобиля", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int carId = (int)cmbCar.SelectedValue;
                    int locationId = (int)cmbLocation.SelectedValue;
                    int count = int.Parse(txtCount.Text);
                    int statusId = (int)cmbStatus.SelectedValue;

                    string modification = txtModification.Text;
                    string trimLevel = txtTrimLevel.Text;

                    // Проверяем, существует ли автомобиль с указанной модификацией и комплектацией
                    car existingCar = db.cars.FirstOrDefault(c =>
                        c.modification == modification &&
                        c.trim_level == trimLevel);

                    if (existingCar != null)
                    {
                        // Проверяем, существует ли этот автомобиль в инвентаре по указанному расположению
                        bool carExistsInInventory = db.inventories.Any(inv =>
                            inv.car_id == existingCar.car_id &&
                            inv.location_id == locationId);

                        if (carExistsInInventory)
                        {
                            MessageBox.Show("Такой автомобиль уже существует в инвентаре по указанному расположению.");
                        }
                        else
                        {
                            // Если автомобиль существует, но отсутствует в инвентаре по указанному расположению, добавляем его
                            inventory newInventory = new inventory
                            {
                                car_id = existingCar.car_id,
                                location_id = locationId,
                                count = count,
                                status_id = statusId,
                                make = existingCar.make,
                                model = existingCar.model,
                                year = existingCar.year,
                                color = existingCar.color,
                                modification = existingCar.modification,
                                trim_level = existingCar.trim_level,
                            };

                            db.inventories.Add(newInventory);
                            db.SaveChanges();

                            MessageBox.Show("Данные сохранены успешно!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Указанный автомобиль не найден.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Автомобиль не добавлен в инвентарь");
            }

        }
    }
}