using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using Fabrino.Controllers;
using Fabrino.Models;
using System.Windows.Input;
using Fabrino.Views;
using Fabrino.Helpers;

namespace Fabrino
{
    public partial class MainWindow : Window
    {
        // رشته اتصال به دیتابیس 
        private AuthController authController = new AuthController();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            SignUpWindow registerWindow = new SignUpWindow();
            registerWindow.Show();
            this.Close(); // یا Hide اگه بخوای فرم لاگین باز بمونه
        }

        private void GoToForgotPassword(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordStep1 forgotpassword = new ForgotPasswordStep1();
            forgotpassword.Show();
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && tb.Text == "نام کاربری")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "نام کاربری";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void RemovePassword(object sender, RoutedEventArgs e)
        {
            var pb = sender as System.Windows.Controls.PasswordBox;
            if (pb != null && pb.Password == "رمز عبور")
            {
                pb.Clear();
                pb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPassword(object sender, RoutedEventArgs e)
        {
            var pb = sender as System.Windows.Controls.PasswordBox;
            if (pb != null && string.IsNullOrWhiteSpace(pb.Password))
            {
                pb.Password = "رمز عبور";
                pb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string passwordHash = SecurityHelper.ComputeSha256Hash(PasswordTextBox.Password.Trim());
            var user = new UserModel
            {
                username = UsernameTextBox.Text.Trim(),
                password_hash = passwordHash
            };

            if (string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password_hash))
            {
                MessageBox.Show("لطفاً نام کاربری و رمز عبور را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (authController.IsValidUser(user))
            {
                MessageBox.Show("ورود موفق!", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}

