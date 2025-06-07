using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.SellerDashBoard
{
    public partial class SellerSettingPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private UserModel _currentUser;

        public SellerSettingPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
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

                MessageBox.Show("رمز عبور با موفقیت تغییر کرد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در تغییر رمز عبور: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}