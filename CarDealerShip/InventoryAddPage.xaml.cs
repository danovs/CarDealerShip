using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class InventoryAddPage : Page
    {
        private CarDealershipEntities db; // Поле для хранения экземпляра контекста базы данных.

        public InventoryAddPage()
        {
            InitializeComponent();

            // Инициализация экземпляра контекста базы данных.
            db = new CarDealershipEntities();

            // Установка источников данных для раскрывающийся списков.
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
        }

        // Обработчик события изменения выбранного автомобиля в комбобоксах.
        private void cmbCar_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCar.SelectedItem != null)
            {
                car selectedCar = cmbCar.SelectedItem as car;

                if (selectedCar != null)
                {
                    // Заполнение текстовых полей данными об автомобиле.
                    txtMake.Text = selectedCar.make;
                    txtModel.Text = selectedCar.model;
                    txtYear.Text = selectedCar.year.ToString() ?? "Не указан";
                    txtColor.Text = selectedCar.color;
                    txtModification.Text = selectedCar.modification;
                    txtTrimLevel.Text = selectedCar.trim_level;

                    // Поиск и вывод типа кузова автомобиля.
                    if (selectedCar.type_id != null)
                    {
                        var carType = db.car_types.FirstOrDefault(ct => ct.type_id == selectedCar.type_id);
                        txtBodyType.Text = carType != null ? carType.type_name : "Неизвестно";
                    }
                    else
                    {
                        txtBodyType.Text = "Не указан";
                    }
                }
            }
        }

        // Обработчик события для предотвращения ввода букв в txtCount
        private void TxtCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, e.Text.Length - 1); // Проверка, является ли введенный символ цифрой
        }

        // Кнопка "Добавить" для добавления автомобиля в инвентарь. (Производится добавление записи в БД)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения операции добавления
            MessageBoxResult result = MessageBox.Show("Вы точно хотите добавить автомобиль в инвентарь?", "Добавление автомобиля", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Получение данных из элементов на форме.
                    int carId = (int)cmbCar.SelectedValue;
                    int locationId = (int)cmbLocation.SelectedValue;
                    int count = int.Parse(txtCount.Text);
                    int statusId = (int)cmbStatus.SelectedValue;

                    // Поиск выбранного автомобиля в базе данных
                    // Если автомобиль найден, проверяем наличие автомобиля в инвентаре на выбранной локации.
                    // Если автомобиль уже есть в инвентаре на выбранной локации, выводим сообщение, что уже существует данный автомобиль по указанному расположению.
                    var selectedCar = db.cars.FirstOrDefault(c => c.car_id == carId);

                    if (selectedCar != null)
                    {
                        bool carExistsInventory = db.inventories.Any(inv => inv.car_id == carId && inv.location_id == locationId);

                        if (carExistsInventory)
                        {
                            MessageBox.Show("Такой автомобиль уже существует в инвентаре по указанному расположению.");
                        }
                        // В противном случае, создаем новую запись об инвентаре и добавляем её в базу данных.
                        else
                        {
                            inventory newInventory = new inventory
                            {
                                car_id = carId,
                                location_id = locationId,
                                count = count,
                                status_id = statusId
                            };
                            db.inventories.Add(newInventory);
                            db.SaveChanges(); // Сохранение изменений
                            MessageBox.Show("Данные успешно сохранены в инвентаре!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выбранный автомобиль не найден.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Вывод сообщения об ошибке при возникновении исключения
                }
            }
            else
            {
                MessageBox.Show("Автомобиль не добавлен в инвентарь.");
            }
        }
    }
}