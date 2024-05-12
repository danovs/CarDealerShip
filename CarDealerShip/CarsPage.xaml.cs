using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
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
    /// Логика взаимодействия для CarsPage.xaml
    /// </summary>
    public partial class CarsPage : Page
    {
        private CarDealershipEntities db;
        public CarsPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadCarsData();
        }

        private void LoadCarsData()
        {
            DGridCars.ItemsSource = db.cars.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CarsAddPage());

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (DGridCars.SelectedItem != null)
            {
                car selectedCar = DGridCars.SelectedItem as car;

                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить автомобиль с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.cars.Attach(selectedCar);
                        db.cars.Remove(selectedCar);

                        db.SaveChanges();

                        MessageBox.Show("Данный автомобиль удален из базы данных");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Автомобиль не был удален с базы данных");
                }

            }
            else
            {
                MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                car selectedCar = (car)DGridCars.SelectedItem;

                if (selectedCar != null)
                {
                    FrameManger.AdminFrame.Navigate(new CarEditPage(selectedCar));
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите автомобиль для редактирования.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
