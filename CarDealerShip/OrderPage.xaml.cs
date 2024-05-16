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
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        private CarDealershipEntities db;
        private int currentUserId;

        public OrderPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();
            currentUserId = ((App)Application.Current).CurrentUserId;

            LoadClientData();
        }

        private void LoadClientData()
        {
            try
            {
                var client = db.clients.FirstOrDefault(c => c.user_id == currentUserId);

                if (client != null)
                {
                    txtName.Text = client.full_name;
                    textPhoneNumber.Text = client.phone;
                }
                else
                {
                    MessageBox.Show("Данные пользователя не найдены в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SetCarDetails(string makeModel, string trimAndModification, string color)
        {
            txtCarMakeAndModel.Text = makeModel;
            txtTrimLevelAndModification.Text = trimAndModification;
            txtColor.Text = color;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string carMakeAndModel = txtCarMakeAndModel.Text;
                string trimLevelAndModification = txtTrimLevelAndModification.Text;
                string color = txtColor.Text;

                // Разделение строки carMakeAndModel на марку и модель по пробелу
                string[] makeModelParts = carMakeAndModel.Split(' ');

                if (makeModelParts.Length < 2)
                {
                    MessageBox.Show("Неверный формат марки и модели автомобиля. Пожалуйста, укажите их в формате 'Марка Модель'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string carMake = makeModelParts[0]; // Первая часть после разделителя - марка
                string carModel = string.Join(" ", makeModelParts.Skip(1)); // Остальное - модель

                if (string.IsNullOrWhiteSpace(carMake) || string.IsNullOrWhiteSpace(carModel) || string.IsNullOrWhiteSpace(trimLevelAndModification) || string.IsNullOrWhiteSpace(color))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля об автомобиле.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int currentUserId = ((App)Application.Current).CurrentUserId;

                var client = db.clients.FirstOrDefault(c => c.user_id == currentUserId);

                if (client == null)
                {
                    MessageBox.Show("Данные пользователя не найдены в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var car = db.cars.FirstOrDefault(c => c.make == carMake && c.model == carModel && c.color == color);

                if (car == null)
                {
                    MessageBox.Show("Данные об автомобиле не найдены в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                appointment newAppointment = new appointment
                {
                    client_id = client.client_id,
                    car_id = car.car_id,
                    appointment_date = DateTime.Now,
                    appointmentStatus_id = 1
                };

                db.appointments.Add(newAppointment);
                db.SaveChanges();
                MessageBox.Show("Запись о заказе добавлена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении записи о заказе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
