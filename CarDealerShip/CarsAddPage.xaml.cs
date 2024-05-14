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
using Microsoft.Win32;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для CarsAddPage.xaml
    /// </summary>
    public partial class CarsAddPage : Page
    {
        private CarDealershipEntities db;

        private string imagePath;

        public CarsAddPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            cmbBodyType.ItemsSource = db.car_types.Select(c => c.type_name).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.png; *.jpg; *.jpeg)| *.png; *.jpg; *.jpeg | All files(*.*) | *.*";

            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                txtImagePath.Text = imagePath;
                imgPreview.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите добавить автомобиль в инвентарь?", "Добавление автомобиля", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверка на пустые поля
                    if (string.IsNullOrWhiteSpace(txtBrand.Text) ||
                        string.IsNullOrWhiteSpace(txtModel.Text) ||
                        string.IsNullOrWhiteSpace(txtYear.Text) ||
                        string.IsNullOrWhiteSpace(txtColor.Text) ||
                        string.IsNullOrWhiteSpace(txtPrice.Text) ||
                        cmbBodyType.SelectedItem == null ||
                        string.IsNullOrWhiteSpace(txtImagePath.Text))
                    {
                        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string color = txtColor.Text;

                    bool carExists = db.cars.Any(c => c.color == color && c.make == txtBrand.Text && c.model == txtModel.Text);

                    if (carExists)
                    {
                        MessageBox.Show("Автомобиль с такой маркой и моделью уже существует в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string imagePath = txtImagePath.Text;

                    // Проверка наличия файла изображения
                    if (!File.Exists(imagePath))
                    {
                        MessageBox.Show("Выбранное изображение не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] imageData = File.ReadAllBytes(imagePath);

                    car newCar = new car
                    {
                        make = txtBrand.Text,
                        model = txtModel.Text,
                        year = Convert.ToInt32(txtYear.Text),
                        color = color,
                        price = Convert.ToDecimal(txtPrice.Text),
                        type_id = db.car_types.FirstOrDefault(c => c.type_name == cmbBodyType.SelectedItem.ToString()).type_id,
                        photo = imageData,
                        modification = txtModification.Text,
                        trim_level = txtTrimLevel.Text,
                    };

                    db.cars.Add(newCar);
                    db.SaveChanges();

                    MessageBox.Show("Автомобиль успешно добавлен в базу данных!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении автомобиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Автомобиль не был добавлен в систему");
            }
        }
    }
}