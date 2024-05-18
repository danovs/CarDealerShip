using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class CarsPage : Page
    {
        // Экземпляр контекста базы данных.
        private CarDealershipEntities db;

        // Инициализация контекста базы данных и загрузка данных об автомобилях при открытии страницы.
        public CarsPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadCarsData();
        }

        // Метод для загрузки данных об автомобилях из базы данных и отображения их в таблице.
        private void LoadCarsData()
        {
            DGridCars.ItemsSource = db.cars.ToList(); // Установка источника данных для датагрид из списка всех автомобилей в базе данных.
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (db != null)
            {
                string searchText = SearchTextBox.Text.Trim(); // Получаем текст из текстового поля поиска и удаляем лишние пробелы

                // Если текст поиска не пустой, выполняем поиск
                if (!string.IsNullOrEmpty(searchText))
                {
                    // Преобразуем текст поиска в нижний регистр для регистронезависимого поиска
                    string lowercaseSearchText = searchText.ToLower();

                    // Выполняем поиск записей, удовлетворяющих критериям поиска
                    var searchResult = db.cars.Where(car => car.make.ToLower().Contains(lowercaseSearchText) ||
                    car.model.ToLower().Contains(lowercaseSearchText) ||
                    car.year.ToString().Contains(searchText) ||
                    car.color.ToLower().Contains(lowercaseSearchText) ||
                    car.price.ToString().Contains(searchText));

                    // Обновляем источник данных для DataGrid, отображая найденные результаты
                    DGridCars.ItemsSource = searchResult.ToList();
                }
                else
                {
                    // Если текст поиска пустой, отображаем все записи
                    LoadCarsData();
                }
            }
            else
            {
                MessageBox.Show("Ошибка: База данных не инициализирована.");
            }
        }

        // Кнопка "Добавить". Производит переход на страницу добавления автомобиля.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new CarsAddPage());
        }

        // Кнопка "Удалить".
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (DGridCars.SelectedItem != null) // Проверка наличия выбранного автомобиля.
            {
                car selectedCar = DGridCars.SelectedItem as car; // Получение выбранного автомобиля из DataGrid

                // Проверка подтверждения действия пользователя.
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить автомобиль с базы данных?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Присоединение выбранного автомобиля к контексту базы данных. Удаление выбранного автомобиля из БД. Сохранение изменений в БД.
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.cars.Attach(selectedCar); 
                        db.cars.Remove(selectedCar);

                        db.SaveChanges();

                        MessageBox.Show("Данный автомобиль удален из базы данных");
                    }
                    catch (Exception ex)
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

        // Кнопка "Изменить".
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение выбранного автомобиля из датагрид.
                // Переход на страницу редактирования выбранного автомобиля.
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