using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            LoadClientOrderData();
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
                    MessageBox.Show("Не удалось найти данные пользователя в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Пожалуйста, укажите марку и модель автомобиля в формате 'Марка Модель'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string carMake = makeModelParts[0]; // Первая часть после разделителя - марка
                string carModel = string.Join(" ", makeModelParts.Skip(1)); // Остальное - модель

                if (string.IsNullOrWhiteSpace(carMake) || string.IsNullOrWhiteSpace(carModel) || string.IsNullOrWhiteSpace(trimLevelAndModification) || string.IsNullOrWhiteSpace(color))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля автомобиля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int currentUserId = ((App)Application.Current).CurrentUserId;

                var client = db.clients.FirstOrDefault(c => c.user_id == currentUserId);

                if (client == null || string.IsNullOrWhiteSpace(client.full_name) || string.IsNullOrWhiteSpace(client.phone))
                {
                    MessageBox.Show("Не удалось найти данные клиента или они неполные. Пожалуйста, заполните свои контактные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show("Для внесения данных нажмите на значок в верхнем левом углу приложения.", "Подсказка", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var car = db.cars.FirstOrDefault(c => c.make == carMake && c.model == carModel && c.color == color);

                if (car == null)
                {
                    MessageBox.Show("Не удалось найти данные об автомобиле в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Ваш заказ был оформлен. Ожидайте звонок от сотрудника на указанный Вами номер телефона.", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении записи о заказе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadClientOrderData()
        {
            try
            {
                var client = db.clients.FirstOrDefault(c => c.user_id == currentUserId);

                if (client != null)
                {
                    txtName.Text = client.full_name;
                    textPhoneNumber.Text = client.phone;

                    // Поиск последнего оформленного заказа пользователя
                    var lastAppointment = db.appointments
                        .Where(a => a.client_id == client.client_id)
                        .OrderByDescending(a => a.appointment_date)
                        .FirstOrDefault();

                    if (lastAppointment != null)
                    {
                        var car = db.cars.FirstOrDefault(c => c.car_id == lastAppointment.car_id);

                        if (car != null)
                        {
                            // Заполнение текстбоксов данными о последнем заказе
                            SetCarDetails($"{car.make} {car.model}", car.trim_level, car.color);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось найти данные пользователя в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
