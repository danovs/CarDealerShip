using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class FeedbackPageEA : Page
    {
        //Экземпляр контекста БД.
        private CarDealershipEntities db;
        
        // Инициализация экземпляра контекста БД и загрузка записей с таблицы "Feedback" (Отзывы).
        public FeedbackPageEA()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadFeedbackData();
        }

        // Метод для загрузки записей с таблицы "Feedback" (Отзывы). Устанавливаем источник данных для датагрид, содержающего отзывы.
        private void LoadFeedbackData()
        {
            DGridFeedback.ItemsSource = db.feedbacks.ToList();
        }

        // Кнопка "Удалить"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка выбранного элемента в датагрид. Если запись найдена, то приводим выбранный элемент к типу feedback.
            if (DGridFeedback.SelectedItem != null)
            {
                feedback selectedFeedback = DGridFeedback.SelectedItem as feedback;

                // Проверка подтверждения пользователем о удалении записи.
                // Если проверка пройдена, то присоединяем выбранный отзыв к контексту БД, после удаляем его. Сохраняем изменения в БД.
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить отзыв?", "Удаление отзыва", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.feedbacks.Attach(selectedFeedback);
                        db.feedbacks.Remove(selectedFeedback);

                        db.SaveChanges();
                        MessageBox.Show("Отзыв был удалён.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении отзыва: " + ex.Message);
                    }

                    // Обновляем датагрид.
                    LoadFeedbackData();
                }
                else
                {
                    MessageBox.Show("Отзыв не был удалён");
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
