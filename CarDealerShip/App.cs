using System.Windows;

namespace CarDealerShip
{
    public partial class App : Application
    {
        public int CurrentUserId { get; set; }

        public void SetCurrentUserId(int userId)
        {
            CurrentUserId = userId;
        }
    }
}
