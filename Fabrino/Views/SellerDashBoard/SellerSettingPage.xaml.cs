using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.SellerDashBoard
{
    public partial class SellerSettingPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private readonly PasswordValidationService _passwordValidationService;
        private UserModel _currentUser;

        public SellerSettingPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
            _passwordValidationService = new PasswordValidationService();
            _currentUser = user;

            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // اطلاعات پایه کاربر
            UsernameBox.Text = _currentUser.username;
            FullNameBox.Text = _currentUser.full_name;
            EmailBox.Text = _currentUser.Email;
            PhoneBox.Text = _currentUser.Phone;
            CreatedAtBox.Text = _currentUser.created_at.ToString("yyyy/MM/dd HH:mm");
            LastLoginBox.Text = _currentUser.last_login?.ToString("yyyy/MM/dd HH:mm") ?? "هنوز وارد نشده";

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

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("لطفاً نام کامل را وارد کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _userService.UpdateUserInfo(_currentUser, FullNameBox.Text, EmailBox.Text, PhoneBox.Text);
                MessageBox.Show("اطلاعات با موفقیت ذخیره شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره اطلاعات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (string.IsNullOrWhiteSpace(SecurityAnswerBox.Password))
            {
                MessageBox.Show("لطفاً پاسخ سوال امنیتی را وارد کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var selectedQuestion = ((ComboBoxItem)SecurityQuestionComboBox.SelectedItem).Content.ToString();
                _currentUser.security_question = selectedQuestion;
                _currentUser.security_answer_hash = SecurityHelper.ComputeSha256Hash(SecurityAnswerBox.Password);
                _context.SaveChanges();

                SecurityAnswerBox.Clear();
                MessageBox.Show("سوال امنیتی با موفقیت ذخیره شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره سوال امنیتی: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = NewPassword.Password;
            var strength = _passwordValidationService.CheckPasswordStrength(password);
            
            PasswordStrengthText.Text = strength.GetDescription();
            PasswordStrengthText.Foreground = strength.GetColor();
        }
    }
}