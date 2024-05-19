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
        private bool isSearchPlaceholder = true;

        // Инициализация контекста БД, загрузка данных каталога автомобилей из БД.
        public CatalogPanel()
        {
            InitializeComponent();

            db = new CarDealershipEntities();
            if (db != null)
            {
                LoadCatalogData();
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
                SearchTextBox.GotFocus += SearchTextBox_GotFocus;
                SearchTextBox.LostFocus += SearchTextBox_LostFocus;
            }
            else
            {
                MessageBox.Show("БД не инициализирована");
            }
        }

        // Метод для загрузки данных каталога автомобилей из базы данных.
        private void LoadCatalogData()
        {
            // Запрос к базе данных для получения данных каталога.
            var query = from catalog in db.catalogs
                        join car in db.cars on catalog.car_id equals car.car_id
                        join carType in db.car_types on car.type_id equals carType.type_id
                        join inventory in db.inventories on catalog.inventory_id equals inventory.inventory_id
                        join status in db.status on inventory.status_id equals status.status_id
                        join location in db.locations on inventory.location_id equals location.location_id
                        select new
                        {
                            CatalogId = catalog.catalog_id,
                            CarMake = car.make,
                            Model = car.model,
                            BodyType = carType.type_name,
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

        // Кнопка "Добавить".
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Поиск" && db != null)
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();
                var searchTerms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var searchResult = from catalog in db.catalogs
                                   join car in db.cars on catalog.car_id equals car.car_id
                                   join carType in db.car_types on car.type_id equals carType.type_id
                                   join inventory in db.inventories on catalog.inventory_id equals inventory.inventory_id
                                   join status in db.status on inventory.status_id equals status.status_id
                                   join location in db.locations on inventory.location_id equals location.location_id
                                   where searchTerms.All(term =>
                                       car.make.ToLower().Contains(term) ||
                                       car.model.ToLower().Contains(term) ||
                                       car.year.ToString().Contains(term) ||
                                       car.color.ToLower().Contains(term) ||
                                       car.price.ToString().Contains(term) ||
                                       car.trim_level.ToLower().Contains(term) ||
                                       car.modification.ToLower().Contains(term) ||
                                       carType.type_name.ToLower().Contains(term) ||
                                       status.status_name.ToLower().Contains(term) ||
                                       location.location_name.ToLower().Contains(term)
                                   )
                                   select new
                                   {
                                       CatalogId = catalog.catalog_id,
                                       CarMake = car.make,
                                       Model = car.model,
                                       BodyType = carType.type_name,
                                       Year = car.year,
                                       Color = car.color,
                                       Price = car.price,
                                       TrimLevel = car.trim_level,
                                       Modification = car.modification,
                                       StatusName = status.status_name,
                                       LocationName = location.location_name
                                   };

                DGridCatalog.ItemsSource = searchResult.ToList();
            }
            else if (db != null)
            {
                LoadCatalogData();
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