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
    /// Логика взаимодействия для Inventory.xaml
    /// </summary>
    public partial class Inventory : Page
    {
        private CarDealershipEntities db;

        public Inventory()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadInventoryData();
        }


        private void LoadInventoryData()
        {
            var query = from inventory in db.inventories
                        join
                        car in db.cars on inventory.car_id equals car.car_id
                        join location in db.locations on inventory.location_id equals location.location_id
                        join status in db.status on inventory.status_id equals status.status_id
                        select new
                        {
                            inventoryId = inventory.inventory_id,
                            carMake = car.make,
                            Model = car.model,
                            Year = car.year,
                            Color = car.color,
                            TrimLevel = car.trim_level,
                            Modification = car.modification,
                            Count = inventory.count,
                            LocationName = location.location_name,
                            StatusName = status.status_name
                        };
            DGridInventory.ItemsSource = query.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new InventoryAddPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (DGridInventory.SelectedItem != null)
                    {
                        // Get the selected inventory item
                        var selectedInventory = (dynamic)DGridInventory.SelectedItem;

                        // Find the inventoryId from the selected item (using lowercase property name)
                        int inventoryId = selectedInventory.inventoryId;

                        // Find the inventory record to delete from the database
                        var inventoryToDelete = db.inventories.Find(inventoryId);

                        if (inventoryToDelete != null)
                        {
                            // Remove the inventory record from the database
                            db.inventories.Remove(inventoryToDelete);
                            db.SaveChanges();

                            // Refresh the data grid
                            MessageBox.Show("Запись удалена");
                            LoadInventoryData();
                        }
                        else
                        {
                            MessageBox.Show("Выбранная запись не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, выберите запись для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Запись не была удалена");
            }
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DGridInventory.SelectedItem != null)
            {
                var selectedItem = DGridInventory.SelectedItem;
                var propertyInfo = selectedItem.GetType().GetProperty("inventoryId");
                if (propertyInfo != null)
                {
                    int inventoryId = (int)propertyInfo.GetValue(selectedItem);

                    var selectedInventory = db.inventories.FirstOrDefault(i => i.inventory_id == inventoryId);

                    if (selectedInventory != null)
                    {
                        // Передаем inventory_id при создании InventoryEditPage
                        FrameManger.AdminFrame.Navigate(new InventoryEditPage(selectedInventory.inventory_id));
                    }
                    else
                    {
                        MessageBox.Show("Инвентарь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить ID инвентаря.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
