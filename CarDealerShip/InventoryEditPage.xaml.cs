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
    /// Логика взаимодействия для InventoryEditPage.xaml
    /// </summary>
    public partial class InventoryEditPage : Page
    {
        private CarDealershipEntities db;
        private int inventoryId;
        
        public InventoryEditPage(int inventoryId)
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            this.inventoryId = inventoryId;


            cmbCar.ItemsSource = db.cars.ToList();
            cmbCar.DisplayMemberPath = "make";
            cmbCar.SelectedValuePath = "car_id";

            cmbStatus.ItemsSource = db.status.ToList();

            cmbStatus.DisplayMemberPath = "status_name";
            cmbStatus.SelectedValuePath = "status_id";

            cmbLocation.ItemsSource = db.locations.ToList();
            cmbLocation.DisplayMemberPath = "location_name"; // Или другое подходящее свойство
            cmbLocation.SelectedValuePath = "location_id";

            FillFields();
        }

        private void FillFields()
        {
            var selectedInventory = db.inventories.FirstOrDefault(i => i.inventory_id == inventoryId);

            if (selectedInventory != null)
            {
                cmbCar.SelectedValue = selectedInventory.car_id;
                txtCount.Text = selectedInventory.count.ToString();
                cmbLocation.SelectedValue = selectedInventory.location_id;
                cmbStatus.SelectedValue = selectedInventory.status_id;

            }
            else
            {
                MessageBox.Show("Данные не загружены");
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selectedInventory = db.inventories.FirstOrDefault(i => i.inventory_id == inventoryId);

                    if (selectedInventory != null)
                    {
                        int carId = (int)cmbCar.SelectedValue;
                        int locationId = (int)cmbLocation.SelectedValue;

                        bool isDuplicateLocation = db.inventories.Any(inv => inv.car_id == carId && inv.location_id == locationId && inv.inventory_id != selectedInventory.inventory_id);

                        if (isDuplicateLocation)
                        {
                            MessageBox.Show("Указанное расположение для выбранного автомобиля уже занято.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }


                        // Обновляем свойства объекта selectedInventory
                        selectedInventory.car_id = (int)cmbCar.SelectedValue;
                        selectedInventory.location_id = (int)cmbLocation.SelectedValue;
                        selectedInventory.count = int.Parse(txtCount.Text);
                        selectedInventory.status_id = (int)cmbStatus.SelectedValue;

                        // Сохраняем изменения в базе данных
                        db.SaveChanges();

                        MessageBox.Show("Данные успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Изменения не сохранены.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
    }
}
