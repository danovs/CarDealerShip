using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class SettingProfile : Page
    {
        // Экземпляр контекста БД и поле для хранения ID текущего пользователя.
        private readonly CarDealershipEntities db;
        private int currentUserId;

        // Инициализируем экземпляр базы данных. Получаем ID текущего пользователя из класса app.cs. И загружаем данные о пользователе.
        public SettingProfile()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            currentUserId = ((App)Application.Current).CurrentUserId;
            LoadUserData();
        }

        // Обработчик события нажатия кнопки мыши на текстовом поле для имени
        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Focus();
        }

        // Обработчик события изменения текста в текстовом поле для имени
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text.Length > 0)
            {
                textName.Visibility = Visibility.Collapsed;
            }
            else
            {
                textName.Visibility = Visibility.Visible;
            }
        }

        // Обработчик события предварительного ввода текста для текстового поля txtName
        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешены только буквы и пробелы
            Regex regex = new Regex("^[a-zA-Zа-яА-Я ]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Кнопка "Сохранить".
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно сохранить новые изменения?", "Изменение данных", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Проверка наличия корректного идентификатора пользователя. Если проверка пройдена - получаем ноое имя и номер телефона из текстовых полей.
                if (currentUserId != 0)
                {
                    string newFullName = txtName.Text;
                    string phoneNumber = textPhoneNumber.Text;

                    // Проверка длины имени
                    if (newFullName.Length < 10)
                    {
                        MessageBox.Show("ФИО должно содержать минимум 10 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        // Поиск клиента в базе данных по идентификатору пользователя. Если клиент есть, проеряем наличие изменений в данных клиента, после обновляем их.
                        var client = db.clients.FirstOrDefault(c => c.user_id == currentUserId);

                        if (client != null)
                        {
                            if (client.full_name != newFullName || client.phone != phoneNumber)
                            {
                                client.full_name = newFullName;
                                client.phone = phoneNumber;
                                db.SaveChanges();

                                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Нет изменений для сохранения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Клиент не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Некорректный идентификатор пользователя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Данные не были сохранены");
            }
        }

        // Метод для загрузки данных пользователя.
        private void LoadUserData()
        {
            try
            {
                // Поиск клиента в базе данных по идентификатору пользователя. Если запись найдена, передаем данные в текстовые поля.
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
    }
}