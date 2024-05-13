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
    /// Логика взаимодействия для CatalogPanel.xaml
    /// </summary>
    public partial class CatalogPanel : Page
    {
        private CarDealershipEntities db;

        public CatalogPanel()
        {
            InitializeComponent();
            
            db = new CarDealershipEntities();
            
            LoadCatalogData();
        }

        private void LoadCatalogData()
        {
            var query = from catalog in db.catalogs
                        join car in db.cars on catalog.car_id equals car.car_id
                        join car_types in db.car_types on car.type_id equals car_types.type_id
                        join inventory in db.inventories on catalog.inventory_id equals inventory.inventory_id
                        join status in db.status on inventory.status_id equals status.status_id
                        select new
                        {
                            CatalogId = catalog.catalog_id,
                            CarMake = car.make,
                            Model = car.model,
                            BodyType = car_types.type_name,
                            Year = car.year,
                            Color = car.color,
                            Price = car.price,
                            TrimLevel = car.trim_level,
                            Modification = car.modification,
                            StatusName = status.status_name
                        };
            DGridCatalog.ItemsSource = query.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CatalogAddPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (DGridCatalog.SelectedItem != null)
                    {
                        var selectedCatalogItem = (dynamic)DGridCatalog.SelectedItem;

                        int CatalogId = selectedCatalogItem.CatalogId;

                        var selectedCatalogItemToDelete = db.catalogs.Find(CatalogId);

                        if (selectedCatalogItemToDelete != null)
                        {
                            db.catalogs.Remove(selectedCatalogItemToDelete);
                            db.SaveChanges();
                            MessageBox.Show("Запись удалена");

                            LoadCatalogData();
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
    }
}
