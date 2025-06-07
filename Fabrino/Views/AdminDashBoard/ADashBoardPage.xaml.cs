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

namespace Fabrino.Views.AdminDashBoard
{
    public partial class ADashBoardPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public SeriesCollection RoleDistributionSeries { get; set; }
        public List<string> RoleLabels { get; set; }

        public ADashBoardPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadUsers();
            LoadSystemLogs();
            LoadRoleDistribution();
            DataContext = this;
        }

        private void LoadUsers()
        {
            try
            {
                var users = _context.Users
                    .OrderByDescending(u => u.last_login)
                    .Select(u => new
                    {
                        u.full_name,
                        u.role,
                        last_login = u.last_login.HasValue ? 
                            ToPersianDateTime(u.last_login.Value) : "عدم ورود"
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
                var logs = _context.SystemLogs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(50)
                    .Select(l => new
                    {
                        Message = l.Action,
                        Time = ToPersianDateTime(l.Timestamp),
                        l.UserId
                    })
                    .ToList();

                SystemLogsBox.ItemsSource = logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری لاگ‌ها: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                    RoleDistributionSeries.Clear(); // پاک کردن سری‌های قبلی
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

        // رویداد تغییر نقش کاربر
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is UserModel user)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("نام جدید:", "ویرایش نام", user.full_name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    user.full_name = newName;
                    _context.SaveChanges();
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک کاربر را انتخاب کنید.");
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is UserModel user)
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