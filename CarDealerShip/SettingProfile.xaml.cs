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
    /// Логика взаимодействия для SettingProfile.xaml
    /// </summary>
    public partial class SettingProfile : Page
    {
        public SettingProfile()
        {
            InitializeComponent();
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

        private void textSurname_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSurname.Focus();
        }

        private void txtSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSurname.Text) && txtSurname.Text.Length > 0)
            {
                textSurname.Visibility = Visibility.Collapsed;
            }

            else
            {
                textSurname.Visibility = Visibility.Visible;
            }
        }

        private void textLastname_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtLastName.Focus();
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLastName.Text) && txtLastName.Text.Length > 0)
            {
                textLastname.Visibility = Visibility.Collapsed;
            }

            else
            {
                textLastname.Visibility = Visibility.Visible;
            }
        }
    }
}
