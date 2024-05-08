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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для SettingProfile.xaml
    /// </summary>
    public partial class SettingProfile : Page
    {

        private readonly CarDealershipEntities db;
        private int currentUserId;
        
        public SettingProfile()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            currentUserId = ((App)Application.Current).CurrentUserId;
            LoadUserData();
        }

        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Focus();
        }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUserId != 0)
            {
                string newFullName = txtName.Text;
                string phoneNumber = textPhoneNumber.Text;

                try
                {
                    var client = db.clients.FirstOrDefault(c => c.user_id == currentUserId);

                    if (client != null)
                    {
                        // Проверяем, были ли внесены изменения
                        if (client.full_name != newFullName || client.phone != phoneNumber)
                        {
                            // Обновляем данные клиента только при наличии изменений
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

        private void LoadUserData()
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
    }
}
