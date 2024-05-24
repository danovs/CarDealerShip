using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class OrderEditPage : Page
    {
        // Поля для хранения ID клиента и заказа.
        private int clientId;
        private int appointmentId;

        // Экземпляр контекста БД.
        private CarDealershipEntities db;

        // Инициализация ID клиента и заказа. Также, инициализируем БД.
        // Производим загрузку данных клиента и статусов в раскрывающийся список.

        public OrderEditPage(int clientId, int appointmentId)
        {
            InitializeComponent();
            this.clientId = clientId;
            this.appointmentId = appointmentId;
            db = new CarDealershipEntities();
            LoadClientData();
            LoadAppointmentStatus();
        }

        // Загрузка данных о клиенте. Производим поиск по ID клиента, и устанавливаем ФИО и номер телефона в текстовые поля.   
        private void LoadClientData()
        {
            var client = db.clients.FirstOrDefault(c => c.client_id == clientId);
            if (client != null)
            {
                txtClient.Text = client.full_name;
                txtNumber.Text = client.phone;
            }
        }

        // Установка списка статусов в раскрывающийся список.
        private void LoadAppointmentStatus()
        {
            var statusList = db.appointments_status.ToList();
            CmbStatus.ItemsSource = statusList;
            CmbStatus.DisplayMemberPath = "appoinmentStatus_name";
            CmbStatus.SelectedValuePath = "appointmentStatus_id";
        }

        // Кнопка для сохранения изменений.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите изменить статус данной записи?", "Изменение статутса", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selectedStatus = (appointments_status)CmbStatus.SelectedItem;

                    // Находим соответствующую запись по appointmentId.
                    var appointment = db.appointments.FirstOrDefault(a => a.appointment_id == appointmentId);

                    if (appointment != null)
                    {
                        // Проверяем, был ли выбран новый статус
                        if (appointment.appointmentStatus_id != selectedStatus.appointmentStatus_id)
                        {
                            // Обновляем статус записи и сохраняем новые данные в БД.
                            appointment.appointmentStatus_id = selectedStatus.appointmentStatus_id;

                            try
                            {
                                db.SaveChanges();
                                MessageBox.Show("Запись успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Нет изменений для сохранения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Запись не была обновлена");
            }
        }
    }
}
