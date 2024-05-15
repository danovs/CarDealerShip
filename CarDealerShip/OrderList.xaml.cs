using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для OrderList.xaml
    /// </summary>
    public partial class OrderList : Page
    {
        private CarDealershipEntities db;
        
        public OrderList()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadOrderItems();
        }

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
            DGridOrders.ItemsSource = orderData.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить запись с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (DGridOrders.SelectedItem != null)
                    {
                        var selectedOrder = (dynamic)DGridOrders.SelectedItem;

                        int orderID = selectedOrder.ID;

                        var orderToDelete = db.appointments.Find(orderID);

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
                    MessageBox.Show($"Произошла ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Запись не была удалена");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DGridOrders.SelectedItem != null)
            {
                dynamic selectedOrder = DGridOrders.SelectedItem;

                int orderID = selectedOrder.ID;

                var orderToEdit = db.appointments.Find(orderID);

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
    }
}
