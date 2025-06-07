using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Fabrino.Views.SellerDashBoard
{
    public partial class SellerSettingPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private readonly PasswordValidationService _passwordValidationService;
        private UserModel _currentUser;
        private bool _isLoading = true;

        public SellerSettingPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
            _passwordValidationService = new PasswordValidationService();
            _currentUser = user;

            LoadInitialData();
            SetupEventHandlers();
            _isLoading = false;
        }

        private void SetupEventHandlers()
        {
            // اضافه کردن event handler برای تغییرات فیلدها
            FullNameBox.TextChanged += (s, e) =>
            {
                if (!_isLoading) UpdateSaveButtonState();
            };

            EmailBox.TextChanged += (s, e) =>
            {
                if (!_isLoading) UpdateSaveButtonState();
            };

            PhoneBox.TextChanged += (s, e) =>
            {
                if (!_isLoading) UpdateSaveButtonState();
            };
        }

        private void UpdateSaveButtonState()
        {
            bool hasChanges = !string.IsNullOrWhiteSpace(FullNameBox.Text) &&
                            (FullNameBox.Text != _currentUser.full_name ||
                             EmailBox.Text != _currentUser.Email ||
                             PhoneBox.Text != _currentUser.Phone);

            SaveProfileButton.IsEnabled = hasChanges;
            SaveProfileButton.Opacity = hasChanges ? 1.0 : 0.5;
        }

        private void LoadInitialData()
        {
            try
            {
                // اطلاعات پایه کاربر
                UsernameBox.Text = _currentUser.username;
                FullNameBox.Text = _currentUser.full_name;
                EmailBox.Text = _currentUser.Email;
                PhoneBox.Text = _currentUser.Phone;

                // غیرفعال کردن دکمه ذخیره در ابتدا
                SaveProfileButton.IsEnabled = false;
                SaveProfileButton.Opacity = 0.5;

                // انتخاب سوال امنیتی فعلی کاربر
                if (!string.IsNullOrEmpty(_currentUser.security_question))
                {
                    foreach (ComboBoxItem item in SecurityQuestionComboBox.Items)
                    {
                        if (item.Content.ToString() == _currentUser.security_question)
                        {
                            SecurityQuestionComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"خطا در بارگذاری اطلاعات: {ex.Message}", true);
            }
        }

        private void ShowStatusMessage(string message, bool isError)
        {
            MessageBox.Show(
                message,
                isError ? "خطا" : "موفق",
                MessageBoxButton.OK,
                isError ? MessageBoxImage.Error : MessageBoxImage.Information
            );
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                ShowStatusMessage("لطفاً نام کامل را وارد کنید", true);
                return;
            }

            try
            {
                _userService.UpdateUserInfo(_currentUser, FullNameBox.Text, EmailBox.Text, PhoneBox.Text);
                ShowStatusMessage("اطلاعات با موفقیت ذخیره شد", false);

                // به‌روزرسانی وضعیت دکمه ذخیره
                UpdateSaveButtonState();
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"خطا در ذخیره اطلاعات: {ex.Message}", true);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword.Password) ||
                string.IsNullOrWhiteSpace(NewPassword.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword.Password))
            {
                MessageBox.Show("لطفاً تمام فیلدهای رمز عبور را پر کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewPassword.Password != ConfirmPassword.Password)
            {
                MessageBox.Show("رمز جدید و تکرار آن مطابقت ندارند", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_passwordValidationService.IsPasswordStrongEnough(NewPassword.Password))
            {
                MessageBox.Show("رمز عبور باید حداقل 8 کاراکتر و شامل حروف و اعداد باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string oldHash = SecurityHelper.ComputeSha256Hash(CurrentPassword.Password);
            if (_currentUser.password_hash != oldHash)
            {
                MessageBox.Show("رمز فعلی اشتباه است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string newHash = SecurityHelper.ComputeSha256Hash(NewPassword.Password);
                _currentUser.password_hash = newHash;
                _context.SaveChanges();

                CurrentPassword.Clear();
                NewPassword.Clear();
                ConfirmPassword.Clear();
                PasswordStrengthText.Text = "";

                MessageBox.Show("رمز عبور با موفقیت تغییر کرد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در تغییر رمز عبور: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSecurityQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (SecurityQuestionComboBox.SelectedItem == null)
            {
                MessageBox.Show("لطفاً یک سوال امنیتی انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(SecurityAnswerBox.Text))
            {
                MessageBox.Show("لطفاً پاسخ سوال امنیتی را وارد کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var selectedQuestion = ((ComboBoxItem)SecurityQuestionComboBox.SelectedItem).Content.ToString();
                _currentUser.security_question = selectedQuestion;
                _currentUser.security_answer_hash = SecurityHelper.ComputeSha256Hash(SecurityAnswerBox.Text);
                _context.SaveChanges();

                SecurityAnswerBox.Text = string.Empty;
                MessageBox.Show("سوال امنیتی با موفقیت ذخیره شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره سوال امنیتی: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var passwordBox = (PasswordBox)button.Tag;
            var path = (Path)button.Content;
            var parent = (Grid)passwordBox.Parent;

            if (parent == null) return;

            // اگر PasswordBox در حال نمایش است
            if (parent.Children[0] is PasswordBox)
            {
                // ایجاد TextBox جدید
                var textBox = new TextBox
                {
                    Text = passwordBox.Password,
                    Width = passwordBox.ActualWidth,
                    Height = passwordBox.ActualHeight,
                    VerticalAlignment = passwordBox.VerticalAlignment,
                    HorizontalAlignment = passwordBox.HorizontalAlignment,
                    Padding = passwordBox.Padding,
                    Background = passwordBox.Background,
                    BorderThickness = passwordBox.BorderThickness,
                    BorderBrush = passwordBox.BorderBrush,
                    HorizontalContentAlignment = passwordBox.HorizontalContentAlignment,
                    FlowDirection = passwordBox.FlowDirection,
                    ToolTip = passwordBox.ToolTip,
                    FontSize = passwordBox.FontSize
                };

                // تغییر به TextBox
                parent.Children.RemoveAt(0);
                parent.Children.Insert(0, textBox);

                // تغییر آیکون به چشم بسته
                path.Data = System.Windows.Media.Geometry.Parse("M12 7c2.76 0 5 2.24 5 5 0 .65-.13 1.26-.36 1.83l2.92 2.92c1.51-1.26 2.7-2.89 3.43-4.75-1.73-4.39-6-7.5-11-7.5-1.4 0-2.74.25-3.98.7l2.16 2.16C10.74 7.13 11.35 7 12 7zM2 4.27l2.28 2.28.46.46C3.08 8.3 1.78 10.02 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.42.42L19.73 22 21 20.73 3.27 3 2 4.27zM7.53 9.8l1.55 1.55c-.05.21-.08.43-.08.65 0 1.66 1.34 3 3 3 .22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53-2.76 0-5-2.24-5-5 0-.79.2-1.53.53-2.2zm4.31-.78l3.15 3.15.02-.16c0-1.66-1.34-3-3-3l-.17.01z");
            }
            // اگر TextBox در حال نمایش است
            else if (parent.Children[0] is TextBox)
            {
                var textBox = (TextBox)parent.Children[0];
                passwordBox.Password = textBox.Text;

                // تغییر به PasswordBox
                parent.Children.RemoveAt(0);
                parent.Children.Insert(0, passwordBox);

                // تغییر آیکون به چشم باز
                path.Data = System.Windows.Media.Geometry.Parse("M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z");
            }

            // به‌روزرسانی افکت‌های hover
            button.MouseEnter += (s, args) => {
                path.Fill = System.Windows.Media.Brushes.OrangeRed;
            };
            button.MouseLeave += (s, args) => {
                path.Fill = System.Windows.Media.Brushes.Gray;
            };
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = NewPassword.Password;
            var strength = _passwordValidationService.CheckPasswordStrength(password);

            PasswordStrengthText.Text = strength.GetDescription();
            PasswordStrengthText.Foreground = strength.GetColor();
            PasswordRequirementsText.Text = strength.GetRequirements();
        }
    }
}