using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class SettingsPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private readonly SignUpController _signUpController;
        private UserModel _currentUser;

        public SettingsPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
            _signUpController = new SignUpController();
            _currentUser = user;

            LoadUser();
            RefreshUsersGrid();
        }

        private void LoadUser()
        {
            FullNameBox.Text = _currentUser.full_name;
            EmailBox.Text = _currentUser.Email;
            PhoneBox.Text = _currentUser.Phone;
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            _userService.UpdateUserInfo(_currentUser, FullNameBox.Text, EmailBox.Text, PhoneBox.Text);
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

            string newHash = SecurityHelper.ComputeSha256Hash(NewPassword.Password);
            _currentUser.password_hash = newHash;
            _context.SaveChanges();

            MessageBox.Show("رمز عبور با موفقیت تغییر کرد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var role = (RoleCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (string.IsNullOrWhiteSpace(NewUsername.Text) || string.IsNullOrWhiteSpace(NewUserPassword.Password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("همه فیلدها الزامی هستند.");
                return;
            }

            var hashedPass = SecurityHelper.ComputeSha256Hash(NewUserPassword.Password);
            var newUser = new UserModel
            {
                username = NewUsername.Text,
                password_hash = hashedPass,
                role = role,
                full_name = NewFullName.Text.Trim(),
                Email = "",
                Phone = "",
                created_at = DateTime.Now,
                //is_active = true
            };

            bool result = _signUpController.RegisterUser(newUser);

            if (result)
            {
                MessageBox.Show("کاربر با موفقیت افزوده شد.", "موفق");
                NewUsername.Text = "";
                NewUserPassword.Password = "";
                RoleCombo.SelectedIndex = -1;
                RefreshUsersGrid();
            }
        }

        private void RefreshUsersGrid()
        {
            UsersGrid.ItemsSource = _context.Users
                .Where(u => u.is_active)
                .ToList();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is UserModel user)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("نام جدید:", "ویرایش نام", user.full_name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    user.full_name = newName;
                    _context.SaveChanges();
                    RefreshUsersGrid();
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک کاربر را انتخاب کنید.");
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is UserModel user)
            {
                var confirm = MessageBox.Show($"آیا می‌خواهید کاربر {user.username} غیرفعال شود؟", "تأیید حذف", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    user.is_active = false;
                    _context.SaveChanges();
                    RefreshUsersGrid();
                }
            }
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
    }
}
