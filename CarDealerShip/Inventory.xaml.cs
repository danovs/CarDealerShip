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

namespace CarDealerShip
{
    /// <summary>
    /// Логика взаимодействия для Inventory.xaml
    /// </summary>
    public partial class Inventory : Page
    {
        private CarDealershipEntities db;
        
        public Inventory()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadInventoryData();
        }


        private void LoadInventoryData()
        {
            DGridInventory.ItemsSource = db.inventories.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new InventoryAddPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (DGridInventory.SelectedItem != null)
            {
                inventory selectedInventory = DGridInventory.SelectedItem as inventory;

                try
                {
                    db.inventories.Attach(selectedInventory);
                    db.inventories.Remove(selectedInventory);

                    db.SaveChanges();

                    MessageBox.Show("Данные удалены");

                    LoadInventoryData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            FrameManger.AdminFrame.Navigate(new InventoryEditPage());
        }
    }
}
