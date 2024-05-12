using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class FeedbackPage : Page
    {
        private CarDealershipEntities db;

        public FeedbackPage()
        {
            InitializeComponent();

            // Инициализируем контекст базы данных
            db = new CarDealershipEntities();

            LoadUserFeedBack();
        }

        private void LoadUserFeedBack()
        {
            int userId = ((App)Application.Current).CurrentUserId;
            var existingFeedback = GetExistingFeedback(userId);

            if (existingFeedback != null)
            {
                txtFeedback.Text = existingFeedback.feedback_text;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = ((App)Application.Current).CurrentUserId;

            try
            {
                // Проверяем, заполнен ли профиль клиента
                if (!IsClientProfileComplete(userId))
                {
                    MessageBox.Show("Пожалуйста, заполните свой профиль перед добавлением отзыва.");
                    return;
                }

                string feedbackText = txtFeedback.Text;

                // Проверяем, что текст отзыва не пустой
                if (string.IsNullOrEmpty(feedbackText))
                {
                    MessageBox.Show("Введите текст отзыва.");
                    return;
                }

                // Проверяем, оставлял ли пользователь уже отзыв
                if (HasUserLeftFeedback(userId))
                {
                    MessageBox.Show("Вы уже оставили отзыв ранее. Новый отзыв добавить нельзя.");
                    return;
                }

                // Добавляем отзыв в базу данных
                if (AddFeedbackToDatabase(userId, feedbackText))
                {
                    MessageBox.Show("Отзыв успешно добавлен!");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении отзыва. Пожалуйста, попробуйте снова.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}\nСтек вызова: {ex.StackTrace}");
            }
        }

        private bool IsClientProfileComplete(int userId)
        {
            // Проверяем наличие клиента с указанным user_id и заполненным профилем
            return db.clients.Any(c => c.user_id == userId && !string.IsNullOrEmpty(c.full_name) && !string.IsNullOrEmpty(c.phone));
        }

        private bool HasUserLeftFeedback(int userId)
        {
            // Получаем client_id для текущего пользователя
            int clientId = GetCurrentClientId(userId);

            // Проверяем наличие отзывов для данного client_id
            return db.feedbacks.Any(f => f.client_id == clientId);
        }



        private bool AddFeedbackToDatabase(int userId, string feedbackText)
        {
            try
            {
                // Получаем client_id для текущего пользователя
                int clientId = GetCurrentClientId(userId);

                // Если клиент не найден, выводим сообщение об ошибке
                if (clientId == 0)
                {
                    MessageBox.Show("Ошибка: клиент не найден.");
                    return false;
                }

                // Создаем новую сущность Feedback
                var newFeedback = new feedback
                {
                    client_id = clientId,
                    feedback_text = feedbackText,
                    feedback_date = DateTime.Now // Устанавливаем текущую дату и время
                };

                // Добавляем отзыв в коллекцию и сохраняем изменения в базе данных
                db.feedbacks.Add(newFeedback);
                db.SaveChanges();

                return true; // Возвращаем успешный результат добавления
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}");
                return false; // Возвращаем неуспешный результат добавления
            }
        }

        private void textFeedback_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtFeedback.Focus();
        }

        private void txtFeedback_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Скрываем подсказку при вводе текста в текстовое поле
            textFeedback.Visibility = string.IsNullOrEmpty(txtFeedback.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Метод для получения client_id по user_id
        private int GetCurrentClientId(int userId)
        {
            var client = db.clients.FirstOrDefault(c => c.user_id == userId);
            return client?.client_id ?? 0; // Возвращаем client_id или 0, если клиент не найден
        }

        private feedback GetExistingFeedback(int userId)
        {
            int clientId = GetCurrentClientId(userId);

            // Находим существующий отзыв для данного клиента, если он есть
            return db.feedbacks.FirstOrDefault(f => f.client_id == clientId);
        }

    }
}