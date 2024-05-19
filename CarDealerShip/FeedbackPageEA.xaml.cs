using System;
using System.Data.Entity; // Добавим пространство имен для Include
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class FeedbackPageEA : Page
    {
        private CarDealershipEntities db;
        private bool isSearchPlaceholder = true;

        public FeedbackPageEA()
        {
            InitializeComponent();

            db = new CarDealershipEntities();
            if (db != null)
            {
                
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
                LoadFeedbackData();
            }
            else
            {
                MessageBox.Show("Ошибка: База данных не инициализирована");
            }
        }

        private void LoadFeedbackData()
        {
            var feedbackData = db.feedbacks.Select(feedback => new
            {
                feedback.feedback_id,
                ClientName = feedback.client != null ? feedback.client.full_name : "", // Проверяем наличие клиента
                feedback.feedback_text,
                feedback.feedback_date
            }).ToList();

            DGridFeedback.ItemsSource = feedbackData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DGridFeedback.SelectedItem != null)
            {
                feedback selectedFeedback = DGridFeedback.SelectedItem as feedback;
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить отзыв?", "Удаление отзыва", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.feedbacks.Attach(selectedFeedback);
                        db.feedbacks.Remove(selectedFeedback);
                        db.SaveChanges();
                        MessageBox.Show("Отзыв был удалён.");
                        LoadFeedbackData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении отзыва: " + ex.Message);
                    }
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Поиск" && db != null)
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();
                var searchTerms = searchText.Split(' ');

                var searchResult = db.feedbacks
                    .Include(f => f.client)
                    .Where(feedback =>
                        searchTerms.All(term =>
                            feedback.client.full_name.ToLower().Contains(term.ToLower()) ||
                            feedback.feedback_text.ToLower().Contains(term.ToLower()) ||
                            feedback.feedback_date.ToString().Contains(term)
                        )
                    )
                    .Select(feedback => new
                    {
                        feedback.feedback_id,
                        ClientName = feedback.client != null ? feedback.client.full_name : "",
                        feedback.feedback_text,
                        feedback.feedback_date
                    })
                    .ToList();

                DGridFeedback.ItemsSource = searchResult;
            }
            else if (db != null) // Добавляем условие для загрузки данных при пустой строке поиска
            {
                LoadFeedbackData();
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isSearchPlaceholder)
            {
                SearchTextBox.Text = "";
                isSearchPlaceholder = false;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск";
                isSearchPlaceholder = true;
            }
        }

        // Добавим обработчик события загрузки страницы для вызова метода LoadFeedbackData
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFeedbackData();
        }
    }
}