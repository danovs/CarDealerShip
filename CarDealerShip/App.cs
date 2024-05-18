using System.Windows;

namespace CarDealerShip
{
    public partial class App : Application
    {
        // Свойство для хранения ID текущего пользователя.
        public int CurrentUserId { get; set; }

        // Метод для установки ID текущего пользователя.
        public void SetCurrentUserId(int userId)
        {
            CurrentUserId = userId;
        }
    }
}
