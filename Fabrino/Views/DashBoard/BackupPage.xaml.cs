using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Fabrino.Views.DashBoard
{
    public partial class BackupPage : Page
    {
        private readonly AppDbContext _context;

        public BackupPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
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

                // Ensure the backup directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(BackupPathBox.Text));

                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "BACKUP DATABASE @dbName TO DISK = @path";
                        command.Parameters.AddWithValue("@dbName", "Fabrino");
                        command.Parameters.AddWithValue("@path", BackupPathBox.Text);
                        command.CommandTimeout = 300; // 5 minutes timeout

                        await command.ExecuteNonQueryAsync();
                    }
                }

                BackupProgress.IsIndeterminate = false;
                BackupProgress.Value = 100;
                StatusText.Text = "نسخه پشتیبان با موفقیت ایجاد شد";
                
                MessageBox.Show("عملیات پشتیبان‌گیری با موفقیت انجام شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("دسترسی به مسیر انتخاب شده امکان‌پذیر نیست. لطفاً مطمئن شوید دسترسی کافی دارید.", 
                    "خطای دسترسی", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "خطا در تهیه نسخه پشتیبان";
            }
            catch (SqlException ex)
            {
                string errorMessage = "خطا در پشتیبان‌گیری از دیتابیس: ";
                if (ex.Number == 3201)
                    errorMessage += "خطا در دسترسی به فایل پشتیبان. لطفاً مطمئن شوید SQL Server به مسیر دسترسی دارد.";
                else if (ex.Number == 3154)
                    errorMessage += "مشکل در نوشتن فایل پشتیبان.";
                else
                    errorMessage += ex.Message;

                MessageBox.Show(errorMessage, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "خطا در تهیه نسخه پشتیبان";
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