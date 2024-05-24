using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarDealerShip
{
    public partial class FeedbackPage : Page
    {
        // Экземпляр контекста БД
        private CarDealershipEntities db;

        // Инициализируем экземпляр контекста БД и загружаем отзыв пользователя, если он был оставлен.
        public FeedbackPage()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            LoadUserFeedBack();
        }

        // Метод для загрузки отзыва пользователя.
        private void LoadUserFeedBack()
        {
            // Получение идентификатора текущего пользователя. И получаем существующий отзыв пользователя, если он был оставлен.
            int userId = ((App)Application.Current).CurrentUserId;

            var existingFeedback = GetExistingFeedback(userId);

            // Если отзыв существует, отображаем его текст в текстовом поле и отключаем текстовое поле.
            if (existingFeedback != null)
            {
                txtFeedback.Text = existingFeedback.feedback_text;
                txtFeedback.IsEnabled = false;
            }
        }

        // "Кнопка сохранить".
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение идентификатора текущего пользователя.
            int userId = ((App)Application.Current).CurrentUserId;

            try
            {
                // Проверка, заполнен ли профиль клиента перед оставлением отзыва.
                if (!IsClientProfileComplete(userId))
                {
                    MessageBox.Show("Пожалуйста, заполните свой профиль перед добавлением отзыва.");
                    FrameManger.MainFrame.Navigate(new SettingProfile());
                    return;
                }

                // Получение текста отзыва из текстового поля.
                string feedbackText = txtFeedback.Text;

                // Проверка, не является ли текст отзыва пустым.
                if (string.IsNullOrEmpty(feedbackText))
                {
                    MessageBox.Show("Введите текст отзыва.");
                    return;
                }

                // Проверка, оставлял ли пользователь уже отзыв ранее.
                if (HasUserLeftFeedback(userId))
                {
                    MessageBox.Show("Вы уже оставили отзыв ранее. Новый отзыв добавить нельзя.");
                    return;
                }

                // Добавление отзыва в базу данных.
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

        // Метод для проверки, заполнен ли профиль клиента.
        private bool IsClientProfileComplete(int userId)
        {
            // Проверка наличия клиента с указанным user_id и заполненным профилем.
            return db.clients.Any(c => c.user_id == userId && !string.IsNullOrEmpty(c.full_name) && !string.IsNullOrEmpty(c.phone));
        }

        // Метод для проверки, оставлял ли пользователь уже отзыв ранее.
        private bool HasUserLeftFeedback(int userId)
        {
            // Получение client_id для текущего пользователя.
            int clientId = GetCurrentClientId(userId);

            // Проверка наличия отзывов для данного client_id.
            return db.feedbacks.Any(f => f.client_id == clientId);
        }

        // Метод для добавления отзыва в базу данных.
        private bool AddFeedbackToDatabase(int userId, string feedbackText)
        {
            try
            {
                int clientId = GetCurrentClientId(userId);

                // Если клиент не найден, вывод сообщения об ошибке.
                if (clientId == 0)
                {
                    MessageBox.Show("Ошибка: клиент не найден.");
                    return false;
                }

                // Создание нового отзыва.
                var newFeedback = new feedback
                {
                    client_id = clientId,
                    feedback_text = feedbackText,
                    feedback_date = DateTime.Now
                };

                // Добавление отзыва в коллекцию и сохранение изменений в базе данных.
                db.feedbacks.Add(newFeedback);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}");
                return false;
            }
        }

        // Обработчик события нажатия мыши на тексте отзыва.
        private void textFeedback_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtFeedback.Focus();
        }

        // Обработчик события изменения текста в текстовом поле для отзыва.
        private void txtFeedback_TextChanged(object sender, TextChangedEventArgs e)
        {
            textFeedback.Visibility = string.IsNullOrEmpty(txtFeedback.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Метод для получения client_id по user_id.
        private int GetCurrentClientId(int userId)
        {
            // Получение клиента по user_id.
            // Возвращение client_id или 0, если клиент не найден.
            var client = db.clients.FirstOrDefault(c => c.user_id == userId);
            return client?.client_id ?? 0;
        }

        // Метод для получения существующего отзыва для пользователя.
        private feedback GetExistingFeedback(int userId)
        {
            // Поиск существующего отзыва для данного клиента, если он есть.
            int clientId = GetCurrentClientId(userId);

            return db.feedbacks.FirstOrDefault(f => f.client_id == clientId);
        }
    }
}