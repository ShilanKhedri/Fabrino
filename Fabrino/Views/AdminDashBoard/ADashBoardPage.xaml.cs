using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Fabrino.Services;
using Fabrino.Helpers;

namespace Fabrino.Views.AdminDashBoard
{
    public class LogViewModel
    {
        public string Message { get; set; }
        public string Time { get; set; }
    }

    public partial class ADashBoardPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly AdminDashboardService _dashboardService;

        public SeriesCollection RoleDistributionSeries { get; set; }
        public List<string> RoleLabels { get; set; }
        public SeriesCollection SalesSeries { get; set; }
        public SeriesCollection PopularFabricsSeries { get; set; }
        public ObservableCollection<Transaction> RecentTransactions { get; set; }
        public ObservableCollection<LowStockItem> LowStockItems { get; set; }

        public ADashBoardPage()
        {
            InitializeComponent();
            _dashboardService = new AdminDashboardService(new AppDbContext());
            DataContext = this;
            LoadUsers();
            LoadSystemLogs();
            LoadRoleDistribution();
            LoadDashboardData();
        }

        private void LoadUsers()
        {
            try
            {
                var users = _context.Users
                    .OrderByDescending(u => u.last_login)
                    .ToList()
                    .Select(u => new
                    {
                        full_name = u.full_name,
                        role = u.role,
                        last_login = DateTimeHelper.ToPersianDateTime(u.last_login)
                    })
                    .ToList();

                UserGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری کاربران: {ex.Message}", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSystemLogs()
        {
            try
            {
                if (_context.SystemLogs == null)
                {
                    SystemLogsBox.ItemsSource = new List<LogViewModel>
                    {
                        new LogViewModel 
                        { 
                            Message = "سیستم لاگ فعال نیست",
                            Time = DateTimeHelper.ToPersianDateTime(DateTime.Now)
                        }
                    };
                    return;
                }

                var logs = _context.SystemLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(50)
                    .ToList()
                    .Select(l => new LogViewModel
                    {
                        Message = l.Action,
                        Time = DateTimeHelper.ToPersianDateTime(l.Timestamp)
                    })
                    .ToList();

                if (!logs.Any())
                {
                    logs = new List<LogViewModel>
                    {
                        new LogViewModel 
                        { 
                            Message = "هیچ لاگی ثبت نشده است",
                            Time = DateTimeHelper.ToPersianDateTime(DateTime.Now)
                        }
                    };
                }

                SystemLogsBox.ItemsSource = logs;
            }
            catch (Exception ex)
            {
                SystemLogsBox.ItemsSource = new List<LogViewModel>
                {
                    new LogViewModel 
                    { 
                        Message = "خطا در بارگذاری لاگ‌ها",
                        Time = DateTimeHelper.ToPersianDateTime(DateTime.Now)
                    }
                };
            }
        }

        private void LoadRoleDistribution()
        {
            try
            {
                var roleStats = _context.Users
                    .GroupBy(u => u.role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .OrderByDescending(r => r.Count)
                    .ToList();

                RoleLabels = roleStats.Select(r => r.Role).ToList();

                if (RoleDistributionSeries == null)
                {
                    RoleDistributionSeries = new SeriesCollection();
                }
                else
                {
                    RoleDistributionSeries.Clear();
                }

                foreach (var role in roleStats)
                {
                    RoleDistributionSeries.Add(new PieSeries
                    {
                        Title = role.Role,
                        Values = new ChartValues<int> { role.Count },
                        DataLabels = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری آمار نقش‌ها: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDashboardData()
        {
            // موجودی کل پارچه
            var totalStock = _dashboardService.GetTotalFabricStock();
            TotalFabricStock.Text = $"{totalStock:N0} متر";

            // مقایسه سفارشات
            var ordersComparison = _dashboardService.GetOrdersComparison();
            TodayOrders.Text = ordersComparison.todayOrders.ToString();
            var changeText = ordersComparison.percentageChange >= 0 ? "↑" : "↓";
            var changeColor = ordersComparison.percentageChange >= 0 ? "#4CAF50" : "#F44336";
            OrdersComparison.Text = $"{changeText} {Math.Abs(ordersComparison.percentageChange):N1}% از دیروز";
            OrdersComparison.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(changeColor);

            // نمودار فروش
            LoadSalesChart();

            // نمودار پرفروش‌ترین پارچه‌ها
            LoadPopularFabricsChart();
        }

        private void LoadSalesChart()
        {
            var salesData = _dashboardService.GetSalesChart();
            
            SalesSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "فروش",
                    Values = new ChartValues<decimal>(salesData.Select(x => x.Sales)),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    LineSmoothness = 0
                }
            };

            ((CartesianChart)FindName("SalesChart")).Series = SalesSeries;
            ((CartesianChart)FindName("SalesChart")).AxisX.Clear();
            ((CartesianChart)FindName("SalesChart")).AxisX.Add(new Axis
            {
                Title = "ماه",
                Labels = salesData.Select(x => x.Month).ToList()
            });
        }

        private void LoadPopularFabricsChart()
        {
            var popularFabrics = _dashboardService.GetPopularFabrics();
            
            PopularFabricsSeries = new SeriesCollection();
            foreach (var fabric in popularFabrics)
            {
                PopularFabricsSeries.Add(new PieSeries
                {
                    Title = fabric.FabricName,
                    Values = new ChartValues<int> { fabric.SalesCount },
                    DataLabels = true,
                    LabelPoint = point => $"{fabric.FabricName}"
                });
            }

            ((PieChart)FindName("PopularFabricsChart")).Series = PopularFabricsSeries;
        }

        private string ToPersianDateTime(DateTime date)
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(date):0000}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00} - " +
                       $"{date.Hour:00}:{date.Minute:00}";
            }
            catch
            {
                return "تاریخ نامعتبر";
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = UserGrid.SelectedItem;
            if (selectedItem != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.full_name == selectedItem.GetType().GetProperty("full_name").GetValue(selectedItem).ToString());
                if (user != null)
                {
                    string newName = Microsoft.VisualBasic.Interaction.InputBox("نام جدید:", "ویرایش نام", user.full_name);
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        user.full_name = newName;
                        _context.SaveChanges();
                        LoadUsers();
                    }
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک کاربر را انتخاب کنید.");
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = UserGrid.SelectedItem;
            if (selectedItem != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.full_name == selectedItem.GetType().GetProperty("full_name").GetValue(selectedItem).ToString());
                if (user != null)
                {
                    var confirm = MessageBox.Show($"آیا می‌خواهید کاربر {user.username} غیرفعال شود؟", "تأیید حذف", MessageBoxButton.YesNo);
                    if (confirm == MessageBoxResult.Yes)
                    {
                        user.is_active = false;
                        _context.SaveChanges();
                        LoadUsers();
                    }
                }
            }
        }
    }

    public class Transaction
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string FabricName { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
    }

    public class LowStockItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CurrentStock { get; set; }
        public string MinStock { get; set; }
    }
}