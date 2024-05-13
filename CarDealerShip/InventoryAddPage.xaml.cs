using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
                car selectedCar = cmbCar.SelectedItem as car;

                if (selectedCar != null)
                {
                    txtMake.Text = selectedCar.make;
                    txtModel.Text = selectedCar.model;
                    txtYear.Text = selectedCar.year.ToString() ?? "Не указан";
                    txtColor.Text = selectedCar.color;
                    txtModification.Text = selectedCar.modification;
                    txtTrimLevel.Text = selectedCar.trim_level;
                }

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

                    var selectedCar = db.cars.FirstOrDefault(c => c.car_id == carId);

                    if (selectedCar != null)
                    {
                        bool carExistsInventory = db.inventories.Any(inv => inv.car_id == carId && inv.location_id == locationId);

                        if (carExistsInventory)
                        {
                            MessageBox.Show("Такой автомобиль уже существует в инвентаре по указанному расположению.");
                        }

                        else
                        {
                            inventory newInventory = new inventory
                            {
                                car_id = carId,
                                location_id = locationId,
                                count = count,
                                status_id = statusId
                            };
                            db.inventories.Add(newInventory);
                            db.SaveChanges();
                            MessageBox.Show("Данные успешно сохранены в инвентаре!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выбранный автомобиль не найден.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Автомобиль не добавлен в инвентарь.");
            }
        }
    }
}