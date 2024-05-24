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
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (DGridInventory.SelectedItem != null)
                    {
                        dynamic selectedInventory = DGridInventory.SelectedItem;
                        int inventoryId = selectedInventory.InventoryId; // Используйте правильное имя свойства
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
                    MessageBox.Show($"Произошла ошибка при удалении записи.\n" +
                        $"Скорее всего, данная запись где-то используется.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Проверяем, выбрана ли запись в датагриде
            if (DGridInventory.SelectedItem != null)
            {
                // Получаем выбранный инвентарь из датагрида
                dynamic selectedItem = DGridInventory.SelectedItem;

                // Получаем значение inventoryId из выбранной записи
                int inventoryId = selectedItem.InventoryId;

                // Создаем экземпляр страницы редактирования и передаем значение inventoryId в конструктор
                FrameManger.AdminFrame.Navigate(new InventoryEditPage(inventoryId));
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
