using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security;
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
        
        private inventory SelectedInentoryItem;
        
        public InventoryEditPage(inventory selectedInventoryItem)
        {
            InitializeComponent();

            SelectedInentoryItem = selectedInventoryItem;
            db = new CarDealershipEntities();

            LoadData();
        }

        private void LoadData()
        {
            if (SelectedInentoryItem != null)
            {
                // Заполняем комбобокс с автомобилями
                cmbCar.ItemsSource = db.cars.ToList(); // Предполагается, что cars - это DbSet в вашем контексте данных
                cmbCar.DisplayMemberPath = "make"; // Устанавливаем свойство, отображаемое в комбобоксе
                cmbCar.SelectedValuePath = "car_id"; // Устанавливаем свойство, используемое для значения выбора

                // Заполняем комбобокс с местоположениями
                cmbLocation.ItemsSource = db.locations.ToList();
                cmbLocation.DisplayMemberPath = "location_name";
                cmbLocation.SelectedValuePath = "location_id";

                // Заполняем комбобокс со статусами
                cmbStatus.ItemsSource = db.status.ToList();
                cmbStatus.DisplayMemberPath = "status_name";
                cmbStatus.SelectedValuePath = "status_id";

                // Заполняем поля данными выбранной записи инвентаря
                cmbCar.SelectedValue = SelectedInentoryItem.car_id;
                cmbLocation.SelectedValue = SelectedInentoryItem.location_id;
                cmbStatus.SelectedValue = SelectedInentoryItem.status_id;
                txtCount.Text = SelectedInentoryItem.count.ToString();
            }
        }

        private void cmbCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCar.SelectedItem != null)
            {
                var selectedCar = cmbCar.SelectedItem as car;

                txtMake.Text = selectedCar.make;
                txtModel.Text = selectedCar.model;
                txtBodyType.Text = selectedCar.car_types.type_name;
                txtYear.Text = selectedCar.year.ToString();
                txtColor.Text = selectedCar.color;
                txtTrimLevel.Text = selectedCar.trim_level;
                txtModification.Text = selectedCar.modification;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы хотите внести изменения в запись?", "Внесение изменений", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (SelectedInentoryItem != null)
                    {
                        var inventoryItem = db.inventories.FirstOrDefault(i => i.inventory_id == SelectedInentoryItem.inventory_id);

                        if (inventoryItem != null)
                        {
                            if(string.IsNullOrWhiteSpace(txtCount.Text))
                            {
                                MessageBox.Show("Необходимо заполнить все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return; // Прерываем операцию сохранения
                            }

                            inventoryItem.car_id = (int)cmbCar.SelectedValue;
                            inventoryItem.location_id = (int)cmbLocation.SelectedValue;
                            inventoryItem.status_id = (int)cmbStatus.SelectedValue;
                            inventoryItem.count = int.Parse(txtCount.Text);
                            inventoryItem.make = txtMake.Text;
                            inventoryItem.model = txtModel.Text;
                            inventoryItem.color = txtColor.Text;
                            inventoryItem.year = int.Parse(txtYear.Text);
                            inventoryItem.modification = txtModification.Text;
                            inventoryItem.trim_level = txtTrimLevel.Text;

                            db.SaveChanges();

                            MessageBox.Show("Данные сохранены");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Внесения не были изменены");
            }
        }
    }
}
