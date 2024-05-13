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
    /// Логика взаимодействия для CatalogAddPage.xaml
    /// </summary>
    public partial class CatalogAddPage : Page
    {
        private CarDealershipEntities db;

        public CatalogAddPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            cmbCar.ItemsSource = db.cars.ToList();
            cmbCar.DisplayMemberPath = "make";

            cmbLocation.ItemsSource = db.locations.ToList();
            cmbLocation.DisplayMemberPath = "location_name";

            cmbCar.SelectionChanged += cmbCar_SelectionChanged;
            cmbLocation.SelectionChanged += cmbLocation_SelectionChanged;
        }

        private void cmbCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedCarData();
        }

        private void cmbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedCarData();
        }

        private void LoadSelectedCarData()
        {
            car selectedCar = cmbCar.SelectedItem as car;
            location selectedLocation = cmbLocation.SelectedItem as location;

            if (selectedCar != null)
            {
                // Устанавливаем данные в текстовые блоки
                txtMake.Text = selectedCar.make;
                txtModel.Text = selectedCar.model;

                var carType = db.car_types.FirstOrDefault(ct => ct.type_id == selectedCar.type_id);
                if (carType != null)
                {
                    txtBodyType.Text = carType.type_name;
                }

                txtYear.Text = selectedCar.year.ToString();
                txtColor.Text = selectedCar.color;
                txtTrimLevel.Text = selectedCar.trim_level;
                txtModification.Text = selectedCar.modification;
                txtPrice.Text = selectedCar.price.ToString();

                // Проверяем, что selectedLocation не равен null
                if (selectedLocation != null)
                {
                    // Поиск информации о инвентаре для выбранного автомобиля и расположения
                    var invInfo = db.inventories.FirstOrDefault(inv => inv.car_id == selectedCar.car_id && inv.location_id == selectedLocation.location_id);
                    if (invInfo != null)
                    {
                        txtCount.Text = invInfo.count.ToString();

                        var statusInfo = db.status.FirstOrDefault(st => st.status_id == invInfo.status_id);

                        if (statusInfo != null)
                        {
                            txtStatus.Text = statusInfo.status_name;
                        }
                        else
                        {
                            txtStatus.Text = "Статус не определен";
                        }
                    }
                    else
                    {
                        txtCount.Text = "Нет данных";
                        txtStatus.Text = "Статус не определен";
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            car selectedCar = cmbCar.SelectedItem as car;

            if (selectedCar != null)
            {
                // Проверяем наличие для выбранного автомобиля
                var invInfo = db.inventories.FirstOrDefault(inv => inv.car_id == selectedCar.car_id);

                if (invInfo != null && invInfo.count > 0)
                {
                    try
                    {
                        // Создаем новую запись в каталоге
                        catalog newCatalogEntry = new catalog
                        {
                            car_id = selectedCar.car_id,
                            inventory_id = invInfo.inventory_id
                            // Добавьте другие поля, если необходимо
                        };

                        // Добавляем запись в таблицу каталогов
                        db.catalogs.Add(newCatalogEntry);
                        db.SaveChanges();

                        MessageBox.Show("Запись успешно добавлена в каталог.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Недостаточное количество автомобиля для добавления в каталог.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль для добавления в каталог.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}