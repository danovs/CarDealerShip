using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CarDealerShip
{
    public partial class CarsAddPage : Page
    {
        // Экземляр контекста БД. Текущий редактируемый автомобиль. Путь к выбанному изображению.
        private CarDealershipEntities db;

        private string imagePath;

        public CarsAddPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities(); // Инициализация контекста базы данных.

            // Заполнение выпадающего списка типов кузовов данными из базы данных.
            cmbBodyType.ItemsSource = db.car_types.Select(c => c.type_name).ToList();

            // Подключение обработчиков событий для предотвращения ввода цифр в txtBrand и txtColor
            txtBrand.PreviewTextInput += TxtBrand_PreviewTextInput;
            txtColor.PreviewTextInput += TxtColor_PreviewTextInput;
        }

        // Обработчик события для предотвращения ввода цифр в txtBrand
        private void TxtBrand_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(char.IsDigit); // Проверка, является ли введенный символ цифрой
        }

        // Обработчик события для предотвращения ввода цифр в txtColor
        private void TxtColor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(char.IsDigit); // Проверка, является ли введенный символ цифрой
        }

        // Кнопка выбора картинки для автомобиля.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.png; *.jpg; *.jpeg)| *.png; *.jpg; *.jpeg | All files(*.*) | *.*";

            // Открытие диалогового окна для выбора изображения. Получение пути к выбранному файлу, его отображение, и отображение выбранного изображения.
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                txtImagePath.Text = imagePath;
                imgPreview.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        // Кнопка "Добавить автомобиль"
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите добавить автомобиль в инвентарь?", "Добавление автомобиля", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Проверка подтверждения действия пользователя.
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверка наличия заполнения всех обязательных полей формы.
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

                    // Проверка наличия автомобиля с такой же маркой, моделью и цветом в базе данных.
                    bool carExists = db.cars.Any(c => c.color == color && c.make == txtBrand.Text && c.model == txtModel.Text);

                    if (carExists)
                    {
                        MessageBox.Show("Автомобиль с такой маркой и моделью уже существует в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string imagePath = txtImagePath.Text;

                    // Проверка наличия выбранного файла изображения.
                    if (!File.Exists(imagePath))
                    {
                        MessageBox.Show("Выбранное изображение не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] imageData = File.ReadAllBytes(imagePath); // Чтение байтов изображения из файла.

                    // Создание нового объекта car и заполнение его данными.
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

                    // Добавление нового автомобиля в базу данных и сохранение изменений.
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