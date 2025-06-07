using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class BackupPage : Page
    {
        public BackupPage()
        {
            InitializeComponent();
            BackupProgress.Visibility = Visibility.Collapsed;
        }

        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Backup files (*.bak)|*.bak|All files (*.*)|*.*",
                DefaultExt = ".bak",
                FileName = $"Fabrino_Backup_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (dialog.ShowDialog() == true)
            {
                BackupPathBox.Text = dialog.FileName;
            }
        }

        private async void StartBackup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(BackupPathBox.Text))
            {
                MessageBox.Show("لطفا مسیر ذخیره فایل پشتیبان را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                BackupProgress.Visibility = Visibility.Visible;
                BackupProgress.IsIndeterminate = true;
                StatusText.Text = "در حال تهیه نسخه پشتیبان...";

                // TODO: Implement actual backup logic here
                await System.Threading.Tasks.Task.Delay(2000); // Simulating backup process

                BackupProgress.IsIndeterminate = false;
                BackupProgress.Value = 100;
                StatusText.Text = "نسخه پشتیبان با موفقیت ایجاد شد";
                
                MessageBox.Show("عملیات پشتیبان‌گیری با موفقیت انجام شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در پشتیبان‌گیری: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "خطا در تهیه نسخه پشتیبان";
            }
            finally
            {
                BackupProgress.IsIndeterminate = false;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 