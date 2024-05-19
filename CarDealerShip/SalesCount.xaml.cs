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
    
    public partial class SalesCount : Page
    {
        private CarDealershipEntities db;
        public SalesCount()
        {
            InitializeComponent();

            db = new CarDealershipEntities();

            LoadSaleCountData();
        }

        private void LoadSaleCountData()
        {
            var salesData = from saleCount in db.sales_counts
                            join employee in db.employees on saleCount.employee_id equals employee.employee_id
                            select new
                            {
                                SalesCountId = saleCount.sales_count_id,
                                EmployeeName = employee.name + " " + employee.surname,
                                SalesCount = saleCount.Sales_count,
                                TotalSales = saleCount.total_sales
                            };
            DGridSaleCount.ItemsSource = salesData.ToList();
        }

        private void UpdateSalesCounts_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Выберите дата начала и дата конца.");
                return;
            }

            UpdateSalesCounts((DateTime)startDate, (DateTime)endDate);
            LoadSaleCountData();
        }

        private void UpdateSalesCounts(DateTime startDate, DateTime endDate)
        {
            var salesInPeriod = db.sales.Where(s => s.sale_date >= startDate && s.sale_date <= endDate).ToList();

            var salesGroupedByEmployee = salesInPeriod.GroupBy(s => s.employee_id).Select(g => new
            {
                EmployeeId = g.Key,
                SalesCount = g.Count(),
                TotalSales = g.Sum(s => s.sale_price)
            });

            foreach (var group in salesGroupedByEmployee)
            {
                var existingSalesCount = db.sales_counts
            .FirstOrDefault(sc => sc.employee_id == group.EmployeeId);

                if (existingSalesCount != null)
                {
                    existingSalesCount.Sales_count = group.SalesCount;
                    existingSalesCount.total_sales = group.TotalSales;
                }
                else
                {
                    var newSalesCount = new sales_counts
                    {
                        employee_id = group.EmployeeId,
                        Sales_count = group.SalesCount,
                        total_sales = group.TotalSales
                    };
                    db.sales_counts.Add(newSalesCount);
                }
            }
            db.SaveChanges();
        }
    }
}
