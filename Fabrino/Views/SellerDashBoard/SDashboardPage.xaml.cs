using LiveCharts;
using LiveCharts.Wpf;
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

namespace Fabrino.Views.SellerDashBoard
{
    /// <summary>
    /// Interaction logic for SDashboardPage.xaml
    /// </summary>
    public partial class SDashboardPage : Page
    {
        private readonly DashboardService _dashboardService;
        public SDashboardPage()
        {
            InitializeComponent();
            _dashboardService = new DashboardService(new AppDbContext());
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            var data = _dashboardService.GetDashboardData();

            // تنظیم مقادیر در UI
            ProductCountText.Text = data.ProductCount.ToString();
            TotalOrdersText.Text = data.TotalOrders.ToString();
            OutOfStockText.Text = data.OutOfStockItems.ToString();
            CustomerCountText.Text = data.CustomerCount.ToString("N0");

            // داده تستی برای نمودار دایره‌ای
            SalesPieChart.Series = new SeriesCollection
    {
        new PieSeries
        {
            Title = "پارچه ساتن",
            Values = new ChartValues<double> { 40 },
            DataLabels = true,
            Fill = new SolidColorBrush(Color.FromRgb(135, 206, 250))
        },
        new PieSeries
        {
            Title = "پارچه کتان",
            Values = new ChartValues<double> { 30 },
            DataLabels = true,
            Fill = new SolidColorBrush(Color.FromRgb(255, 160, 122))
        },
        new PieSeries
        {
            Title = "پارچه حریر",
            Values = new ChartValues<double> { 20 },
            DataLabels = true,
            Fill = new SolidColorBrush(Color.FromRgb(152, 251, 152))
        },
        new PieSeries
        {
            Title = "پارچه مخمل",
            Values = new ChartValues<double> { 10 },
            DataLabels = true,
            Fill = new SolidColorBrush(Color.FromRgb(221, 160, 221))
        }
    };
        }
    }
}
