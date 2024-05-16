using System;
using System.Collections.Generic;
using System.Linq;
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
            catch(Exception ex)
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
        }
    }
}
