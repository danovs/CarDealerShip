using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class CatalogPanel : Page
    {
        // Экземпляр контекста базы данных.
        private CarDealershipEntities db;

        // Инициализация контекста БД, загрузка данных каталога автомобилей из БД.
        public CatalogPanel()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadCatalogData();
        }

        // Метод для загрузки данных каталога автомобилей из базы данных.
        private void LoadCatalogData()
        {
            // Запрос к базе данных для получения данных каталога. (Присваиваем в поля, значения с запроса. Данные поля будут использованы в привязке колонок в XAML)
            var query = from catalog in db.catalogs
                        join car in db.cars on catalog.car_id equals car.car_id
                        join car_types in db.car_types on car.type_id equals car_types.type_id
                        join inventory in db.inventories on catalog.inventory_id equals inventory.inventory_id
                        join status in db.status on inventory.status_id equals status.status_id
                        join location in db.locations on inventory.location_id equals location.location_id
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
                            StatusName = status.status_name,
                            LocationName = location.location_name
                        };

            // Установка источника данных для DataGrid на основе результата запроса.
            DGridCatalog.ItemsSource = query.ToList();
        }

        // Кнопрка "Добавить".
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CatalogAddPage());
        }

        // Кнопка "Удалить".
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения действия пользователя.
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверка наличия выбранной записи в DataGrid
                    if (DGridCatalog.SelectedItem != null)
                    {
                        var selectedCatalogItem = (dynamic)DGridCatalog.SelectedItem;
                        int CatalogId = selectedCatalogItem.CatalogId;

                        // Поиск выбранной записи в базе данных
                        var selectedCatalogItemToDelete = db.catalogs.Find(CatalogId);

                        // Удаление записи, если она найдена
                        if (selectedCatalogItemToDelete != null)
                        {
                            db.catalogs.Remove(selectedCatalogItemToDelete);
                            db.SaveChanges();
                            MessageBox.Show("Запись удалена");

                            LoadCatalogData(); // Повторная загрузка данных каталога после удаления записи
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