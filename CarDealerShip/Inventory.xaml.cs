using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class Inventory : Page
    {
        // Экземпляр контекста БД
        private CarDealershipEntities db;
        private bool isSearchPlaceholder = true;

        // Инициализация экземпляра контекста БД и загрузка данных в датагрид.
        public Inventory()
        {
            InitializeComponent();

            db = new CarDealershipEntities();
            if (db != null )
            {
                LoadInventoryData();
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            }
            else
            {
                MessageBox.Show("БД не инициализирована");
            }
        }

        // Загрузка данных в датагрид. Создаем запрос через LINQ, после присваиваем поля для хранения данных с БД.
        // Данные поля используются для свойства Binding в xaml, чтобы мы могли вывести данные в колонки датагрида.
        private void LoadInventoryData()
        {
            var query = from inventory in db.inventories
                        join
                        car in db.cars on inventory.car_id equals car.car_id
                        join location in db.locations on inventory.location_id equals location.location_id
                        join status in db.status on inventory.status_id equals status.status_id
                        select new
                        {
                            InventoryId = inventory.inventory_id,
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
            DGridInventory.ItemsSource = query.ToList(); // Устанавливаем данные в датагрид.
        }

        // Кнопка "Добавить" производится переход на страницу добавления записи.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new InventoryAddPage());
        }

        // Кнопка "Удалить" производится удаление записи с БД.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения удаления записи.
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Если выбранная запись не пустая, получаем выбранную запись, и производим поиск по ID выбранной записи.
                    // Найдя ID выбранной записи, заносим в переменную inventoryToDelete, и делаем проверку, если не пустая переменная, то производим удаление записи и сохраняем изменение в БД.
                    // После удаления - обновляем датагрид с помощью вызова функции "LoadInventoryData"
                    if (DGridInventory.SelectedItem != null)
                    {
                        var selectedInventory = (dynamic)DGridInventory.SelectedItem;
                        int inventoryId = selectedInventory.inventoryId;
                        var inventoryToDelete = db.inventories.Find(inventoryId);

                        if (inventoryToDelete != null)
                        {
                            db.inventories.Remove(inventoryToDelete);
                            db.SaveChanges();

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

        // Кнопка "Изменить" на каждой записи в датагриде.
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Проверям на пустоту выбранную запись. Если не пустая, то получаем выбранный элемент и получаем свойство inventoryId (ID инвентаря)
            if (DGridInventory.SelectedItem != null)
            {
                var selectedItem = DGridInventory.SelectedItem;
                var propertyInfo = selectedItem.GetType().GetProperty("inventoryId");
                
                // Если свойство найдено, производим поиск записи инвентаря в БД.
                if (propertyInfo != null)
                {
                    int inventoryId = (int)propertyInfo.GetValue(selectedItem);

                    var selectedInventory = db.inventories.FirstOrDefault(i => i.inventory_id == inventoryId);

                    if (selectedInventory != null)
                    {
                        // Переход на страницу редактирования инвентаря с передачей идентификатора.
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Поиск" && db != null)
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();
                var searchTerms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var searchResult = from inventory in db.inventories
                                   join car in db.cars on inventory.car_id equals car.car_id
                                   join location in db.locations on inventory.location_id equals location.location_id
                                   join status in db.status on inventory.status_id equals status.status_id
                                   where searchTerms.All(term =>
                                       car.make.ToLower().Contains(term) ||
                                       car.model.ToLower().Contains(term) ||
                                       car.year.ToString().Contains(term) ||
                                       car.color.ToLower().Contains(term) ||
                                       car.trim_level.ToLower().Contains(term) ||
                                       car.modification.ToLower().Contains(term) ||
                                       location.location_name.ToLower().Contains(term) ||
                                       status.status_name.ToLower().Contains(term)
                                   )
                                   select new
                                   {
                                       InventoryId = inventory.inventory_id,
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

                DGridInventory.ItemsSource = searchResult.ToList();
            }
            else if (db != null)
            {
                LoadInventoryData();
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
