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
                    bool hasChanges = false;

                    // Only update if changed
                    if (user.full_name != FullNameBox.Text)
                    {
                        user.full_name = FullNameBox.Text;
                        hasChanges = true;
                    }

                    if (user.Email != EmailBox.Text)
                    {
                        user.Email = EmailBox.Text;
                        hasChanges = true;
                    }

                    if (user.Phone != PhoneBox.Text)
                    {
                        user.Phone = PhoneBox.Text;
                        hasChanges = true;
                    }

                    // Update security question if changed
                    var selectedQuestion = ((ComboBoxItem)SecurityQuestionComboBox.SelectedItem)?.Content.ToString();
                    if (selectedQuestion != null && user.security_question != selectedQuestion)
                    {
                        user.security_question = selectedQuestion;
                        hasChanges = true;
                    }

                    // Only update security answer if provided and changed
                    if (!string.IsNullOrWhiteSpace(SecurityAnswerBox.Text))
                    {
                        var hashedAnswer = SecurityHelper.ComputeSha256Hash(SecurityAnswerBox.Text);
                        if (user.security_answer_hash != hashedAnswer)
                        {
                            user.security_answer_hash = hashedAnswer;
                            hasChanges = true;
                        }
                    }

                    if (hasChanges)
                    {
                        _context.SaveChanges();
                        MessageBox.Show("اطلاعات با موفقیت ذخیره شد.", "موفقیت", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("تغییری در اطلاعات ایجاد نشده است.", "اطلاع", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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

                // Ensure the backup directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

                // Use parameterized query to prevent SQL injection
                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_context.Database.GetConnectionString()))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "BACKUP DATABASE @dbName TO DISK = @path";
                        command.Parameters.AddWithValue("@dbName", "Fabrino");
                        command.Parameters.AddWithValue("@path", backupPath);
                        command.CommandTimeout = 300; // 5 minutes timeout

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("پشتیبان‌گیری با موفقیت انجام شد.", "موفقیت", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("دسترسی به مسیر انتخاب شده امکان‌پذیر نیست. لطفاً مسیر دیگری را انتخاب کنید یا با دسترسی ادمین برنامه را اجرا کنید.", 
                    "خطای دسترسی", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                string errorMessage = "خطا در پشتیبان‌گیری از دیتابیس: ";
                if (ex.Number == 1807) // Cannot open backup device
                    errorMessage += "امکان ایجاد فایل پشتیبان در مسیر انتخاب شده وجود ندارد. لطفاً مطمئن شوید SQL Server به این مسیر دسترسی دارد.";
                else if (ex.Number == 3201) // Cannot open backup device
                    errorMessage += "خطا در دسترسی به فایل پشتیبان. لطفاً مطمئن شوید مسیر معتبر است و SQL Server به آن دسترسی دارد.";
                else
                    errorMessage += ex.Message;

                MessageBox.Show(errorMessage, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    // First check if the file exists and is accessible
                    if (!File.Exists(dialog.FileName))
                    {
                        MessageBox.Show("فایل پشتیبان انتخاب شده وجود ندارد.", "خطا", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var result = MessageBox.Show(
                        "بازیابی نسخه پشتیبان باعث جایگزینی تمام اطلاعات فعلی با اطلاعات نسخه پشتیبان خواهد شد. آیا مطمئن هستید؟",
                        "هشدار",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_context.Database.GetConnectionString()))
                        {
                            connection.Open();
                            
                            // Close all existing connections
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = @"
                                    USE [master];
                                    ALTER DATABASE [Fabrino] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                                command.ExecuteNonQuery();
                            }

                            // Restore database
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "RESTORE DATABASE @dbName FROM DISK = @path WITH REPLACE";
                                command.Parameters.AddWithValue("@dbName", "Fabrino");
                                command.Parameters.AddWithValue("@path", dialog.FileName);
                                command.CommandTimeout = 300; // 5 minutes timeout

                                command.ExecuteNonQuery();
                            }

                            // Set database back to multi-user mode
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "ALTER DATABASE [Fabrino] SET MULTI_USER";
                                command.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("بازیابی با موفقیت انجام شد. لطفاً برنامه را مجدداً اجرا کنید.", "موفقیت", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("دسترسی به فایل پشتیبان امکان‌پذیر نیست. لطفاً مطمئن شوید دسترسی کافی دارید.", 
                    "خطای دسترسی", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                string errorMessage = "خطا در بازیابی دیتابیس: ";
                if (ex.Number == 3201)
                    errorMessage += "خطا در دسترسی به فایل پشتیبان. لطفاً مطمئن شوید SQL Server به فایل دسترسی دارد.";
                else if (ex.Number == 3154)
                    errorMessage += "فایل پشتیبان معتبر نیست یا آسیب دیده است.";
                else
                    errorMessage += ex.Message;

                MessageBox.Show(errorMessage, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بازیابی: {ex.Message}", "خطا", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                try
                {
                    // Always try to set database back to multi-user mode in case of errors
                    using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_context.Database.GetConnectionString()))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "ALTER DATABASE [Fabrino] SET MULTI_USER";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    // Ignore errors in cleanup
                }
            }
        }
    }
}
