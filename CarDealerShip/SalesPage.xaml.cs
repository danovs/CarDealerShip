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
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private CarDealershipEntities db;
        public SalesPage()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadSalesData();
        }

        private void LoadSalesData()
        {
            var salesData = from sale in db.sales
                            join car in db.cars on sale.car_id equals car.car_id
                            join employee in db.employees on sale.employee_id equals employee.employee_id
                             join client in db.clients on sale.client_id equals client.client_id
                             join saleStatus in db.sale_statuses on sale.sale_status_id equals saleStatus.sale_status_id
                             select new
                             {
                                 SaleId = sale.sale_id,
                                 EmployeeName = String.Concat(employee.name, " ", employee.surname),
                                 ClientName = client.full_name,
                                 CarName = String.Concat(car.make, " ", car.model),
                                 CarFeatures = String.Concat(car.trim_level, " ", car.modification),
                                 Color = car.color,
                                 Price = sale.sale_price,
                                 Status = saleStatus.sale_status_name,
                                 SaleDate = sale.sale_date
                             };

            DGridSales.ItemsSource = salesData.ToList();

        }
    }
}
