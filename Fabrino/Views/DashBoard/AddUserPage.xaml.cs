using System;
using System.Windows;
using System.Windows.Controls;
using Fabrino.Models;
using Fabrino.Services;
using Fabrino.Helpers;

namespace Fabrino.Views.DashBoard
{
    public partial class AddUserPage : Page
    {
        private readonly AppDbContext _context;
        private readonly SignUpController _signUpController;

        public AddUserPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _signUpController = new SignUpController();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password) ||
                string.IsNullOrEmpty(FullNameBox.Text) ||
                string.IsNullOrEmpty(EmailBox.Text) ||
                RoleComboBox.SelectedItem == null)
            {
                MessageBox.Show("لطفا تمام فیلدها را پر کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var hashedPass = SecurityHelper.ComputeSha256Hash(PasswordBox.Password);
            var newUser = new UserModel
            {
                username = UsernameBox.Text,
                password_hash = hashedPass,
                role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                full_name = FullNameBox.Text,
                Email = EmailBox.Text,
                Phone = "",
                created_at = DateTime.Now,
                is_active = true
            };

            bool result = _signUpController.RegisterUser(newUser);

            if (result)
            {
                MessageBox.Show("کاربر با موفقیت اضافه شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("خطا در ثبت کاربر", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 