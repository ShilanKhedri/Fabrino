// DashBoardPage.xaml.cs
using Fabrino.Services;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Collections.Generic;

namespace Fabrino.Views.DashBoard
{
    public partial class DashBoardPage : Page
    {
        private readonly DashboardService _dashboardService;

        public DashBoardPage()
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

            // دریافت داده‌های واقعی پارچه از دیتابیس
            var fabricData = _dashboardService.GetFabricsByMaterial();
            
            var colors = new List<Color>
            {
                Color.FromRgb(135, 206, 250), // آبی روشن
                Color.FromRgb(255, 160, 122), // نارنجی روشن
                Color.FromRgb(152, 251, 152), // سبز روشن
                Color.FromRgb(221, 160, 221), // بنفش روشن
                Color.FromRgb(255, 182, 193), // صورتی روشن
            };

            var seriesCollection = new SeriesCollection();
            int colorIndex = 0;

            foreach (var item in fabricData)
            {
                seriesCollection.Add(new PieSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<double> { (double)item.Value },
                    DataLabels = true,
                    Fill = new SolidColorBrush(colors[colorIndex % colors.Count])
                });
                colorIndex++;
            }

            SalesPieChart.Series = seriesCollection;
        }
    }
}