using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CarDealerShip
{
    public partial class CarEditPage : Page
    {
        // Экземляр контекста БД. Текущий редактируемый автомобиль. Путь к выбанному изображению.
        private CarDealershipEntities db;

        private car currentCar;

        private string imagePath;
        
        // Инициализацтя контекста БД, поиск текущего автомобиля в БД по его ID. Заполняем выпадающий список типов кузовов и заполянем формы данными о текущем автомобиле.
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

            DisplayImage(); // Отображение текущего автомобиля (фотография).
        }

        // Метод для отображения изображения текущего автомобиля.
        private void DisplayImage()
        {
            if (currentCar.photo != null && currentCar.photo.Length > 0)
            {
                try
                {
                    BitmapImage bitmapImage = LoadImage(currentCar.photo); // Загружаем изображение из массива байтов.
                    imgPreview.Source = bitmapImage; // Устанавливаем изображение в элемент Image.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отображении изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Метод для загрузки изображения из массива байтов.
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
            image.Freeze(); // Замораживаем изображение для повышения производительности.
            return image;
        }

        // Обработчик события нажатия кнопки для выбора изображения.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.png; *.jpg; *.jpeg)|*.png; *.jpg; *.jpeg|All files(*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    imagePath = openFileDialog.FileName; // Сохраняем путь к выбранному файлу в переменной класса
                    txtImagePath.Text = imagePath; // Отображаем путь к файлу в текстовом поле

                    BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath)); // Создаем объект BitmapImage из выбранного файла
                    imgPreview.Source = bitmapImage; // Устанавливаем выбранное изображение в элемент Image
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обработчик события нажатия кнопки для сохранения изменений.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите внести изменения в данный автомобиль?", "Изменение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Если пользователь подтвердил свое намерение внести изменения
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверяем, заполнены ли обязательные поля
                    if (string.IsNullOrEmpty(txtBrand.Text) ||
                        string.IsNullOrEmpty(txtModel.Text) ||
                        string.IsNullOrEmpty(txtYear.Text) ||
                        string.IsNullOrEmpty(txtColor.Text) ||
                        string.IsNullOrEmpty(txtPrice.Text) ||
                        cmbBodyType.SelectedItem == null)
                    {
                        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Если выбрано новое изображение, загружаем его и сохраняем в базе данных
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath); // Чтение байтов изображения из файла
                        currentCar.photo = imageData; // Обновление поля photo у текущего автомобиля
                    }

                    // Обновляем остальные свойства текущего автомобиля
                    currentCar.make = txtBrand.Text;
                    currentCar.model = txtModel.Text;
                    currentCar.year = Convert.ToInt32(txtYear.Text);
                    currentCar.color = txtColor.Text;
                    currentCar.price = Convert.ToDecimal(txtPrice.Text);
                    currentCar.modification = txtModification.Text;
                    currentCar.trim_level = txtConfiguration.Text;

                    // Обновляем тип кузова автомобиля и сохраняем данные в БД.
                    string selectedBodyType = cmbBodyType.SelectedItem.ToString();
                    car_types bodyType = db.car_types.FirstOrDefault(ct => ct.type_name == selectedBodyType);
                    if (bodyType != null)
                    {
                        currentCar.type_id = bodyType.type_id;
                    }

                    db.SaveChanges();
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