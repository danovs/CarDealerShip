﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class CatalogAddPage : Page
    {
        // Экземпляр контекста базы данных
        private CarDealershipEntities db;

        // Инициализация контекста базы данных и установка источников данных для выпадающих списков (Автомобиль, расположение).
        public CatalogAddPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities(); 

            cmbCar.ItemsSource = db.cars.ToList();
            cmbCar.DisplayMemberPath = "make";

            cmbLocation.ItemsSource = db.locations.ToList();
            cmbLocation.DisplayMemberPath = "location_name";

            // Добавление обработчиков событий изменения выбора в комбо-боксах
            cmbCar.SelectionChanged += cmbCar_SelectionChanged;
            cmbLocation.SelectionChanged += cmbLocation_SelectionChanged;
        }

        // Обработчик события изменения выбранного автомобиля. Загружает данные о автомобиле.
        private void cmbCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedCarData();
        }

        // Обработчик события изменения выбранного местоположения. Загружает данные о выбранном автомобиле, исходя из расположения. (Отображение количества и статуса).
        private void cmbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedCarData();
        }

        // Метод для загрузки данных о выбранном автомобиле.
        private void LoadSelectedCarData()
        {
            car selectedCar = cmbCar.SelectedItem as car; // Получение выбранного автомобиля.
            location selectedLocation = cmbLocation.SelectedItem as location; // Получение выбранного местоположения.

            if (selectedCar != null)
            {
                // Заполнение текстовых блоков данными о выбранном автомобиле.
                txtMake.Text = selectedCar.make;
                txtModel.Text = selectedCar.model;
                txtYear.Text = selectedCar.year.ToString();
                txtColor.Text = selectedCar.color;
                txtTrimLevel.Text = selectedCar.trim_level;
                txtModification.Text = selectedCar.modification;
                txtPrice.Text = selectedCar.price.ToString();

                // Поиск и отображение информации о типе кузова автомобиля.
                var carType = db.car_types.FirstOrDefault(ct => ct.type_id == selectedCar.type_id);
                if (carType != null)
                {
                    txtBodyType.Text = carType.type_name;
                }

                if (selectedLocation != null)
                {
                    // Поиск и отображение информации о наличии автомобиля на выбранном местоположении.
                    var invInfo = db.inventories.FirstOrDefault(inv => inv.car_id == selectedCar.car_id && inv.location_id == selectedLocation.location_id);
                    if (invInfo != null)
                    {
                        txtCount.Text = invInfo.count.ToString(); // Отображение количества доступных автомобилей.
                        var statusInfo = db.status.FirstOrDefault(st => st.status_id == invInfo.status_id); // Получение информации о статусе автомобиля.
                        if (statusInfo != null)
                        {
                            txtStatus.Text = statusInfo.status_name; // Отображение статуса автомобиля.
                        }
                        else
                        {
                            txtStatus.Text = "Статус не определен";
                        }
                    }
                    else
                    {
                        txtCount.Text = "Нет данных";
                        txtStatus.Text = "Статус не определен";
                    }
                }
            }
        }

        // Кнопка "Добавить".
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            car selectedCar = cmbCar.SelectedItem as car;
            location selectedLocation = cmbLocation.SelectedItem as location;

            if (selectedCar != null && selectedLocation != null)
            {
                // Проверка подтверждения действия пользователя.
                MessageBoxResult result = MessageBox.Show("Вы точно хотите добавить автомобиль в инвентарь?", "Добавление автомобиля", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Проверка наличия записи о выбранном автомобиле и местоположении в каталоге
                    var existingEntry = db.catalogs.FirstOrDefault(c => c.car_id == selectedCar.car_id && c.inventory.location_id == selectedLocation.location_id);

                    if (existingEntry != null)
                    {
                        MessageBox.Show("Данный автомобиль уже добавлен в каталог для выбранного расположения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        var invInfo = db.inventories.FirstOrDefault(inv => inv.car_id == selectedCar.car_id && inv.location_id == selectedLocation.location_id);

                        if (invInfo != null && invInfo.count > 0)
                        {
                            try
                            {
                                // Создание новой записи в каталоге
                                catalog newCatalogEntry = new catalog
                                {
                                    car_id = selectedCar.car_id,
                                    inventory_id = invInfo.inventory_id
                                };

                                // Добавление записи в таблицу каталогов
                                db.catalogs.Add(newCatalogEntry);
                                db.SaveChanges();

                                MessageBox.Show("Запись успешно добавлена в каталог.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Недостаточное количество автомобиля на складе для добавления в каталог.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль и местоположение для добавления в каталог.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}