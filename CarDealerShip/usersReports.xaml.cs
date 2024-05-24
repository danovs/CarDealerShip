using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    public partial class usersReports : Page
    {
        private CarDealershipEntities db;
        private bool isSearchPlaceholder = true;

        public usersReports()
        {
            InitializeComponent();
            db = new CarDealershipEntities();
            LoadUsersData();
        }

        private void LoadUsersData()
        {
            try
            {
                var users = from user in db.users
                            select new
                            {
                                UserID = user.user_id,
                                UserName = user.username
                            };
                DGridUsers.ItemsSource = users.ToList();

                TotalUsersTextBlock.Text = "Всего пользователей: " + users.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isSearchPlaceholder)
            {
                string searchText = SearchTextBox.Text.ToLower();
                var filteredUsers = from user in db.users
                                    where user.username.ToLower().Contains(searchText)
                                    select new
                                    {
                                        UserID = user.user_id,
                                        UserName = user.username
                                    };
                DGridUsers.ItemsSource = filteredUsers.ToList();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DGridUsers.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить данного пользователя?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var selectedUser = DGridUsers.SelectedItem;

                        // Получение идентификатора пользователя
                        int userId = (int)selectedUser.GetType().GetProperty("UserID").GetValue(selectedUser, null);

                        // Найти и удалить связанные записи в таблицах clients и employees
                        var relatedClients = db.clients.Where(c => c.user_id == userId).ToList();
                        db.clients.RemoveRange(relatedClients);

                        var relatedEmployees = db.employees.Where(re => re.user_id == userId).ToList();
                        db.employees.RemoveRange(relatedEmployees);

                        // Найти и удалить пользователя
                        var userToDelete = db.users.FirstOrDefault(u => u.user_id == userId);
                        if (userToDelete != null)
                        {
                            db.users.Remove(userToDelete);
                            db.SaveChanges();

                            // Обновление данных после удаления
                            LoadUsersData();
                            MessageBox.Show("Пользователь успешно удален.");
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден в базе данных.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления пользователя: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите пользователя для удаления.");
                }
            }
            else
            {
                MessageBox.Show("Пользователь не был удалён");
            }
        }
    }
}
