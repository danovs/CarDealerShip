using System;
using System.Collections.Generic;
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
using Xceed.Wpf.Toolkit;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для FeedbackPageEA.xaml
    /// </summary>
    public partial class FeedbackPageEA : Page
    {
        private CarDealershipEntities db;
        
        public FeedbackPageEA()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadFeedbackData();
        }

        private void LoadFeedbackData()
        {
            DGridFeedback.ItemsSource = db.feedbacks.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DGridFeedback.SelectedItem != null)
            {
                feedback selectedFeedback = DGridFeedback.SelectedItem as feedback;

                MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить отзыв?", "Удаление отзыва", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.feedbacks.Attach(selectedFeedback);
                        db.feedbacks.Remove(selectedFeedback);

                        db.SaveChanges();
                        System.Windows.MessageBox.Show("Отзыв был удалён.");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка при удалении отзыва: " + ex.Message);
                    }

                    LoadFeedbackData();
                }
                else
                {
                    System.Windows.MessageBox.Show("Отзыв не был удалён");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
