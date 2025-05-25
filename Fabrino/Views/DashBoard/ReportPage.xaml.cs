using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace Fabrino.Views.DashBoard
{
    public partial class ReportPage : Page
    {
        public SeriesCollection FabricSalesSeries { get; set; }
        public List<string> FabricLabels { get; set; }

        public ReportPage()
        {
            InitializeComponent();
            LoadChartData();
            LoadBuyerData();
            LoadUserRoles();
            DataContext = this;
        }

        private void LoadChartData()
        {
            FabricLabels = new List<string> { "حریر", "ساتن", "نخی", "کتان" };

            FabricSalesSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "تعداد فروش",
                    Values = new ChartValues<int> { 20, 35, 12, 40 }
                }
            };
        }

        private void LoadBuyerData()
        {
            BuyersGrid.ItemsSource = new List<dynamic>
            {
                new { BuyerName = "احمدی", Product = "حریر", Price = "200,000", Date = "1403/02/21" },
                new { BuyerName = "جعفری", Product = "ساتن", Price = "300,000", Date = "1403/02/22" },
            };

            // علامت‌گذاری روزهای خرید در تقویم
            PurchaseCalendar.SelectedDates.Add(new DateTime(2025, 5, 2));
            PurchaseCalendar.SelectedDates.Add(new DateTime(2025, 5, 10));
            PurchaseCalendar.SelectedDates.Add(new DateTime(2025, 5, 18));

        }

        private void LoadUserRoles()
        {
            UserRolesGrid.ItemsSource = new List<dynamic>
            {
                new { Username = "sara_admin", Role = "مدیر" },
                new { Username = "ali_seller", Role = "فروشنده" },
            };
        }
    }
}
