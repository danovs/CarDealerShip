using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class InventoryEditPage : Page
    {
        private CarDealershipEntities db; // Поле для хранения экземпляра контекста базы данных.
        private int inventoryId; // Поле для хранения ID инвентаря, который редактируется.

        // Инициализация экземпляра контекста базы данных.
        // Присваивание переданного ID инвентаря полю inventoryId.
        // Установка источников данных для комбо-боксов.
        // Заполнение полей данными для редактируемого инвентаря.
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
            cmbLocation.DisplayMemberPath = "location_name";
            cmbLocation.SelectedValuePath = "location_id";

            // Подключение обработчика события для предотвращения ввода букв в txtCount
            txtCount.PreviewTextInput += TxtCount_PreviewTextInput;

            FillFields();
        }

        // Метод для заполнения полей данными редактируемого инвентаря.
        private void FillFields()
        {
            var selectedInventory = db.inventories.FirstOrDefault(i => i.inventory_id == inventoryId);

            if (selectedInventory != null)
            {
                // Заполнение полей данными из выбранного инвентаря.
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

        // Обработчик события для предотвращения ввода букв в txtCount
        private void TxtCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, e.Text.Length - 1); // Проверка, является ли введенный символ цифрой
        }

        // Кнопка "Сохранить".
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения для изменения данных
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Получение выбранного инвентаря для редактирования.
                    var selectedInventory = db.inventories.FirstOrDefault(i => i.inventory_id == inventoryId);

                    if (selectedInventory != null)
                    {
                        // Проверка, были ли изменения в форме редактирования.
                        bool dataChanged = IsDataChanged(selectedInventory);

                        if (dataChanged)
                        {
                            // Производим проверку наличия другого инвентаря с таким же автомобилем на выбранной локации.
                            int carId = (int)cmbCar.SelectedValue;
                            int locationId = (int)cmbLocation.SelectedValue;
                            bool isDuplicateLocation = db.inventories.Any(inv => inv.car_id == carId && inv.location_id == locationId && inv.inventory_id != selectedInventory.inventory_id);

                            if (isDuplicateLocation)
                            {
                                MessageBox.Show("Указанное расположение для выбранного автомобиля уже занято.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return; // Прерывание выполнения метода.
                            }

                            // Обновляем свойства редактируемого инвентаря.
                            selectedInventory.car_id = (int)cmbCar.SelectedValue;
                            selectedInventory.location_id = (int)cmbLocation.SelectedValue;
                            selectedInventory.count = int.Parse(txtCount.Text);
                            selectedInventory.status_id = (int)cmbStatus.SelectedValue;

                            db.SaveChanges();

                            MessageBox.Show("Данные успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Нет изменений для сохранения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Инвентарь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Если пользователь не подтвердил сохранение
            {
                MessageBox.Show("Изменения не сохранены.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Метод для проверки, были ли изменения в форме редактирования.
        private bool IsDataChanged(inventory selectedInventory)
        {
            int carId = (int)cmbCar.SelectedValue;
            int locationId = (int)cmbLocation.SelectedValue;
            int count = int.Parse(txtCount.Text);
            int statusId = (int)cmbStatus.SelectedValue;

            return selectedInventory.car_id != carId ||
                   selectedInventory.location_id != locationId ||
                   selectedInventory.count != count ||
                   selectedInventory.status_id != statusId;
        }

        // Обработчик события изменения выбранного автомобиля в раскрывающиемся списке.
        private void cmbCar_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если автомобиль выбран, производим получение выбранного автомобиля, и если данный автомобиль найден, то заполняем текстовые поля данными о его характеристиках.
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

                    // Поиск и вывод типа кузова автомобиля.
                    if (selectedCar.type_id != null)
                    {
                        var carType = db.car_types.FirstOrDefault(ct => ct.type_id == selectedCar.type_id); // Получение типа кузова

                        if (carType != null)
                        {
                            txtBodyType.Text = carType.type_name; // Вывод типа кузова
                        }
                        else
                        {
                            txtBodyType.Text = "Неизвестно";
                        }
                    }
                    else // Если тип кузова не указан
                    {
                        txtBodyType.Text = "Не указан";
                    }
                }
            }
        }
    }
}