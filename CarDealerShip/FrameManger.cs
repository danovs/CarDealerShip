using System.Windows.Controls;

namespace CarDealerShip
{
    class FrameManger
    {
        // Статическое свойство для доступа к главному фрейму.
        public static Frame MainFrame { get; set; }

        // Статическое свойство для доступа к административному фрейму.
        public static Frame AdminFrame { get; set; }
    }
}