using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для CarEditPage.xaml
    /// </summary>
    public partial class CarEditPage : Page
    {
        private CarDealershipEntities db;

        private car currentCar;

        private string imagePath;

        public CarEditPage(car car)
        {

            InitializeComponent();

            db = new CarDealershipEntities();
            currentCar = db.cars.Find(car.car_id);
            cmbBodyType.ItemsSource = db.car_types.Select(c => c.type_name).ToList();

            txtBrand.Text = currentCar.make;
            txtModel.Text = currentCar.model;
            txtYear.Text = currentCar.year.ToString();
            txtColor.Text = currentCar.color;
            txtPrice.Text = currentCar.price.ToString();
            cmbBodyType.SelectedItem = currentCar.car_types.type_name;
            txtModification.Text = currentCar.modification;
            txtConfiguration.Text = currentCar.trim_level;
            DisplayImage();

        }

        private void DisplayImage()
        {
            if (currentCar.photo != null && currentCar.photo.Length > 0)
            {
                try
                {
                    BitmapImage bitmapImage = LoadImage(currentCar.photo);

                    imgPreview.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отображении изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public static BitmapImage LoadImage(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.png; *.jpg; *.jpeg)|*.png; *.jpg; *.jpeg|All files(*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    imagePath = openFileDialog.FileName; // Сохраняем путь к выбранному файлу в переменную класса
                    txtImagePath.Text = imagePath;

                    BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                    imgPreview.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите внести изменения в данный автомобиль?", "Изменение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Check if any required fields are empty
                    if (string.IsNullOrEmpty(txtBrand.Text) ||
                        string.IsNullOrEmpty(txtModel.Text) ||
                        string.IsNullOrEmpty(txtYear.Text) ||
                        string.IsNullOrEmpty(txtColor.Text) ||
                        string.IsNullOrEmpty(txtPrice.Text) ||
                        cmbBodyType.SelectedItem == null)
                    {
                        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; // Exit the method without saving if any field is empty
                    }

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        currentCar.photo = imageData; // Обновляем поле photo у объекта currentCar
                    }

                    // Update other properties of currentCar
                    currentCar.make = txtBrand.Text;
                    currentCar.model = txtModel.Text;
                    currentCar.year = Convert.ToInt32(txtYear.Text);
                    currentCar.color = txtColor.Text;
                    currentCar.price = Convert.ToDecimal(txtPrice.Text);
                    currentCar.modification = txtModification.Text;
                    currentCar.trim_level = txtConfiguration.Text;

                    // Update car type (body type)
                    string selectedBodyType = cmbBodyType.SelectedItem.ToString();
                    car_types bodyType = db.car_types.FirstOrDefault(ct => ct.type_name == selectedBodyType);
                    if (bodyType != null)
                    {
                        currentCar.type_id = bodyType.type_id;
                    }

                    db.SaveChanges(); // Save changes to the database
                    MessageBox.Show("Данные успешно обновлены в базе данных!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Изменения не были сохранены");
            }
        }
    }
}
