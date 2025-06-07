using System.Windows;
using System.Windows.Controls;
using Fabrino.Models;
using Fabrino.Services;
using Fabrino.Helpers;

namespace Fabrino.Views.DashBoard
{
    public partial class PasswordSettingsPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserModel _currentUser;

        public PasswordSettingsPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _currentUser = user;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("لطفا تمام فیلدها را پر کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("رمز عبور جدید و تکرار آن مطابقت ندارند", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string oldHash = SecurityHelper.ComputeSha256Hash(CurrentPasswordBox.Password);
            if (_currentUser.password_hash != oldHash)
            {
                MessageBox.Show("رمز عبور فعلی اشتباه است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string newHash = SecurityHelper.ComputeSha256Hash(NewPasswordBox.Password);
            _currentUser.password_hash = newHash;
            _context.SaveChanges();

            MessageBox.Show("رمز عبور با موفقیت تغییر کرد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 