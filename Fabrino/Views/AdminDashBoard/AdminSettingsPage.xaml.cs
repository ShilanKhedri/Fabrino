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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Fabrino.Views.AdminDashBoard
{
    public class UserViewModel
    {
        public string username { get; set; }
        public string full_name { get; set; }
        public string role { get; set; }
        public string last_login_display { get; set; }
        public string status_display { get; set; }
        public Brush status_color { get; set; }
        public string toggle_status_text { get; set; }
        public Brush toggle_status_color { get; set; }
        public bool is_active { get; set; }
    }

    public partial class AdminSettingsPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserModel _currentUser;
        private ObservableCollection<UserViewModel> _users;

        public AdminSettingsPage(UserModel currentUser)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _currentUser = currentUser;
            _users = new ObservableCollection<UserViewModel>();
            LoadUsers();
            LoadCurrentUserData();
        }

        private void LoadUsers()
        {
            var users = _context.Users
                .OrderByDescending(u => u.last_login)
                .ToList()
                .Select(u => new UserViewModel
                {
                    username = u.username,
                    full_name = u.full_name,
                    role = u.role,
                    last_login_display = DateTimeHelper.ToPersianDateTime(u.last_login),
                    status_display = u.is_active ? "فعال" : "غیرفعال",
                    status_color = u.is_active ? 
                        (Brush)new BrushConverter().ConvertFrom("#4CAF50") : 
                        (Brush)new BrushConverter().ConvertFrom("#F44336"),
                    toggle_status_text = u.is_active ? "غیرفعال‌سازی" : "فعال‌سازی",
                    toggle_status_color = u.is_active ? 
                        (Brush)new BrushConverter().ConvertFrom("#F44336") : 
                        (Brush)new BrushConverter().ConvertFrom("#4CAF50"),
                    is_active = u.is_active
                })
                .ToList();

            _users.Clear();
            foreach (var user in users)
            {
                _users.Add(user);
            }
            UsersGrid.ItemsSource = _users;
        }

        private void LoadCurrentUserData()
        {
            UsernameBox.Text = _currentUser.username;
            FullNameBox.Text = _currentUser.full_name;
            EmailBox.Text = _currentUser.Email;
            PhoneBox.Text = _currentUser.Phone;
            
            if (!string.IsNullOrEmpty(_currentUser.security_question))
            {
                // Find the matching question in ComboBox
                for (int i = 0; i < SecurityQuestionComboBox.Items.Count; i++)
                {
                    var item = SecurityQuestionComboBox.Items[i] as ComboBoxItem;
                    if (item.Content.ToString() == _currentUser.security_question)
                    {
                        SecurityQuestionComboBox.SelectedIndex = i;
                        break;
                    }
                }
                SecurityAnswerBox.Text = _currentUser.security_answer_hash;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow();
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button.DataContext as UserViewModel;
            
            var editUserWindow = new EditUserWindow(user.username);
            if (editUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void ToggleUserStatus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button.DataContext as UserViewModel;

            if (user.username == _currentUser.username)
            {
                MessageBox.Show("شما نمی‌توانید وضعیت خود را تغییر دهید.", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dbUser = _context.Users.FirstOrDefault(u => u.username == user.username);
            if (dbUser != null)
            {
                dbUser.is_active = !dbUser.is_active;
                _context.SaveChanges();
                LoadUsers();
            }
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.username == _currentUser.username);
                if (user != null)
                {
                    user.full_name = FullNameBox.Text;
                    user.Email = EmailBox.Text;
                    user.Phone = PhoneBox.Text;

                    // Update security question directly in user model
                    var selectedQuestion = ((ComboBoxItem)SecurityQuestionComboBox.SelectedItem).Content.ToString();
                    user.security_question = selectedQuestion;
                    user.security_answer_hash = SecurityAnswerBox.Text;

                    _context.SaveChanges();
                    MessageBox.Show("اطلاعات با موفقیت ذخیره شد.", "موفقیت", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره اطلاعات: {ex.Message}", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.username == _currentUser.username);
                if (user != null)
                {
                    string currentPasswordHash = SecurityHelper.ComputeSha256Hash(CurrentPassword.Password);
                    if (currentPasswordHash != user.password_hash)
                    {
                        MessageBox.Show("رمز عبور فعلی اشتباه است.", "خطا", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (NewPassword.Password != ConfirmPassword.Password)
                    {
                        MessageBox.Show("رمز عبور جدید و تکرار آن مطابقت ندارند.", "خطا", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsValidPassword(NewPassword.Password))
                    {
                        MessageBox.Show("رمز عبور باید حداقل 8 کاراکتر و شامل حروف بزرگ، کوچک، اعداد و علائم باشد.", "خطا",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    user.password_hash = SecurityHelper.ComputeSha256Hash(NewPassword.Password);
                    _context.SaveChanges();

                    CurrentPassword.Clear();
                    NewPassword.Clear();
                    ConfirmPassword.Clear();

                    MessageBox.Show("رمز عبور با موفقیت تغییر کرد.", "موفقیت", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در تغییر رمز عبور: {ex.Message}", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectBackupPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                BackupPathBox.Text = dialog.FolderName;
            }
        }

        private void BackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(BackupPathBox.Text))
                {
                    MessageBox.Show("لطفاً مسیر ذخیره‌سازی را انتخاب کنید.", "خطا", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string backupFileName = $"FabrinoBackup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string backupPath = Path.Combine(BackupPathBox.Text, backupFileName);

                string query = $"BACKUP DATABASE [Fabrino] TO DISK = '{backupPath}'";
                _context.Database.ExecuteSqlRaw(query);

                MessageBox.Show("پشتیبان‌گیری با موفقیت انجام شد.", "موفقیت", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در پشتیبان‌گیری: {ex.Message}", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Backup files (*.bak)|*.bak|All files (*.*)|*.*",
                    Title = "انتخاب فایل پشتیبان"
                };

                if (dialog.ShowDialog() == true)
                {
                    string query = $@"
                        USE [master];
                        ALTER DATABASE [Fabrino] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        RESTORE DATABASE [Fabrino] FROM DISK = '{dialog.FileName}' WITH REPLACE;
                        ALTER DATABASE [Fabrino] SET MULTI_USER;";

                    _context.Database.ExecuteSqlRaw(query);

                    MessageBox.Show("بازیابی با موفقیت انجام شد.", "موفقیت", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بازیابی: {ex.Message}", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
