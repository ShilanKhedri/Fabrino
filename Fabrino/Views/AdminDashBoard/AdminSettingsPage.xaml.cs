using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.AdminDashBoard
{
    public partial class AdminSettingsPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly UserModel _currentUser;

        public AdminSettingsPage(UserModel user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUser();
        }

        private void LoadUser()
        {
            FullNameBox.Text = _currentUser.full_name;
            EmailBox.Text = _currentUser.Email;
            PhoneBox.Text = _currentUser.Phone;
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            _currentUser.full_name = FullNameBox.Text.Trim();
            _currentUser.Email = EmailBox.Text.Trim();
            _currentUser.Phone = PhoneBox.Text.Trim();

            _context.SaveChanges();
            MessageBox.Show("اطلاعات ذخیره شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldHash = SecurityHelper.ComputeSha256Hash(CurrentPassword.Password);
            if (_currentUser.password_hash != oldHash)
            {
                MessageBox.Show("رمز فعلی اشتباه است.", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewPassword.Password != ConfirmPassword.Password)
            {
                MessageBox.Show("رمزهای جدید همخوانی ندارند.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentUser.password_hash = SecurityHelper.ComputeSha256Hash(NewPassword.Password);
            _context.SaveChanges();
            MessageBox.Show("رمز عبور با موفقیت تغییر کرد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "Backup Files (*.bak)|*.bak",
                    FileName = $"FabrinoBackup_{DateTime.Now:yyyyMMdd_HHmmss}.bak"
                };

                if (dlg.ShowDialog() == true)
                {
                    var query = $"BACKUP DATABASE [Fabrino] TO DISK = '{dlg.FileName}' WITH FORMAT";
                    using var conn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                    conn.Open();
                    using var cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("بک‌آپ با موفقیت ذخیره شد.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بک‌آپ:\n" + ex.Message);
            }
        }

        private void RestoreDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Filter = "Backup Files (*.bak)|*.bak"
                };

                if (dlg.ShowDialog() == true)
                {
                    string backupPath = dlg.FileName;
                    string dbName = "Fabrino";

                    var query = $@"
                    USE master;
                    ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    RESTORE DATABASE [{dbName}] FROM DISK = '{backupPath}' WITH REPLACE;
                    ALTER DATABASE [{dbName}] SET MULTI_USER;
                    ";

                    using var conn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                    conn.Open();
                    using var cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("بازیابی بک‌آپ با موفقیت انجام شد.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بازیابی بک‌آپ:\n" + ex.Message);
            }
        }
    }
}
