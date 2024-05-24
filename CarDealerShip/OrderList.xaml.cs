using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class OrderList : Page
    {
        // Экземпляр контекста БД.

        private CarDealershipEntities db;
        private bool isSearchPlaceholder = true;

        // Инициализация экземпляра контекста БД и загрузка данных о заказе.

        public OrderList()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;

            LoadOrderItems();
        }

        // Функция загрузки данных о заказе. Здесь мы делаем запрос к нашей БД, где мы обращаемся к таблицам, которые имеют связь с таблицей appointments (Заказы)
        private void LoadOrderItems()
        {
            var orderData = from appointment in db.appointments
                            join client in db.clients on appointment.client_id equals client.client_id
                            join car in db.cars on appointment.car_id equals car.car_id
                            join appointmentStatus in db.appointments_status on appointment.appointmentStatus_id equals appointmentStatus.appointmentStatus_id
                            select new
                            {
                                ID = appointment.appointment_id,
                                Client = client.full_name,
                                Phone = client.phone,
                                Make = car.make,
                                Model = car.model,
                                Year = car.year,
                                Color = car.color,
                                TrimLevel = car.trim_level,
                                Modification = car.modification,
                                Date = appointment.appointment_date,
                                Status = appointmentStatus.appoinmentStatus_name
                            };
            DGridOrders.ItemsSource = orderData.ToList(); // Устанавливаем данные в датагрид.
        }

        // Кнопка для удаления записи из базы данных.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения пользователя.
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (DGridOrders.SelectedItem != null) // Проверка выбора записи из датагрида.
                    {
                        // Получение выбранной записи, идентификатора выбранного заказа, и поиск по ID в базе данных.
                        var selectedOrder = (dynamic)DGridOrders.SelectedItem;

                        int orderID = selectedOrder.ID;

                        var orderToDelete = db.appointments.Find(orderID);

                        // Если заказ найден, производим удаление из базы данных. После удаления, вызываем метод для обновление данных в датагриде.
                        if (orderToDelete != null)
                        {
                            db.appointments.Remove(orderToDelete);
                            db.SaveChanges();

                            MessageBox.Show("Запись удалена");

                            LoadOrderItems();
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

        // Кнопка изменить.
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Проверка выбора записи из датагрид.
            // Получение ID выбранного заказа, и поиск его в базе данных.
            if (DGridOrders.SelectedItem != null)
            {
                dynamic selectedOrder = DGridOrders.SelectedItem;

                int orderID = selectedOrder.ID;

                var orderToEdit = db.appointments.Find(orderID);

                // Если заказ надйен, производим переход на страницу редактирования заказа, куда мы передаем ID клиента, и заказа.
                if (orderToEdit != null)
                {
                    FrameManger.AdminFrame.Navigate(new OrderEditPage(orderToEdit.client_id, orderToEdit.appointment_id));
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите заказ для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Поиск" && db != null)
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();
                var searchTerms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var searchResult = from appointment in db.appointments
                                   join client in db.clients on appointment.client_id equals client.client_id
                                   join car in db.cars on appointment.car_id equals car.car_id
                                   join status in db.appointments_status on appointment.appointmentStatus_id equals status.appointmentStatus_id
                                   where searchTerms.All(term =>
                                       client.full_name.ToLower().Contains(term) ||
                                       client.phone.ToLower().Contains(term) ||
                                       car.make.ToLower().Contains(term) ||
                                       car.model.ToLower().Contains(term) ||
                                       car.year.ToString().Contains(term) ||
                                       car.color.ToLower().Contains(term) ||
                                       car.trim_level.ToLower().Contains(term) ||
                                       car.modification.ToLower().Contains(term) ||
                                       appointment.appointment_date.ToString().Contains(term) ||
                                       status.appoinmentStatus_name.ToLower().Contains(term)
                                   )
                                   select new
                                   {
                                       ID = appointment.appointment_id,
                                       Client = client.full_name,
                                       Phone = client.phone,
                                       Make = car.make,
                                       Model = car.model,
                                       Year = car.year,
                                       Color = car.color,
                                       TrimLevel = car.trim_level,
                                       Modification = car.modification,
                                       Date = appointment.appointment_date,
                                       Status = status.appoinmentStatus_name
                                   };

                DGridOrders.ItemsSource = searchResult.ToList();
            }
            else if (db != null)
            {
                LoadOrderItems();
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
