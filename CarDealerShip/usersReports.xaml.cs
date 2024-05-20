using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для usersReports.xaml
    /// </summary>
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
                try
                {
                    var selectedUser = DGridUsers.SelectedItem;
                    int userId = (int)selectedUser.GetType().GetProperty("UserID").GetValue(selectedUser, null);

                    // Найти связанные записи в таблице clients
                    var relatedClients = db.clients.Where(c => c.user_id == userId).ToList();
                    foreach (var client in relatedClients)
                    {
                        db.clients.Remove(client);
                    }

                    // Найти связанные записи в таблице employees
                    var relatedEmployees = db.employees.Where(re => re.user_id == userId).ToList();
                    foreach (var employee in relatedEmployees)
                    {
                        db.employees.Remove(employee);
                    }

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
    }
}
