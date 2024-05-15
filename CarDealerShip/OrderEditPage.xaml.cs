using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
    /// Логика взаимодействия для OrderEditPage.xaml
    /// </summary>
    public partial class OrderEditPage : Page
    {
        private int clientId;
        private int appointmentId;
        private CarDealershipEntities db;

        public OrderEditPage(int clientId, int appointmentId)
        {
            InitializeComponent();
            this.clientId = clientId;
            this.appointmentId = appointmentId;
            db = new CarDealershipEntities();
            LoadClientData();
            LoadAppointmentStatus();
        }

        private void LoadClientData()
        {
            var client = db.clients.FirstOrDefault(c => c.client_id == clientId);
            if (client != null)
            {
                txtClient.Text = client.full_name;
                txtNumber.Text = client.phone;
            }
        }

        private void LoadAppointmentStatus()
        {
            var statusList = db.appointments_status.ToList();
            CmbStatus.ItemsSource = statusList;
            CmbStatus.DisplayMemberPath = "appoinmentStatus_name";
            CmbStatus.SelectedValuePath = "appointmentStatus_id";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = (appointments_status)CmbStatus.SelectedItem;

            // Найти соответствующую запись по appointmentId
            var appointment = db.appointments.FirstOrDefault(a => a.appointment_id == appointmentId);

            if (appointment != null)
            {
                // Обновить статус записи
                appointment.appointmentStatus_id = selectedStatus.appointmentStatus_id;

                try
                {
                    // Сохранить изменения в базе данных
                    db.SaveChanges();
                    MessageBox.Show("Запись успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
