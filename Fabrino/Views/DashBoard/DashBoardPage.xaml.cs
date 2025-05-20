// DashBoardPage.xaml.cs
using Fabrino.Services;
using System.Windows.Controls;

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
            CustomerCountText.Text = data.CustomerCount.ToString("N0") ; // فرمت هزارگان
        }
    }
}