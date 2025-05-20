using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using Fabrino.Controllers;
using Fabrino.Models;
using System.Windows.Input;
using Fabrino.Views;
using Fabrino.Helpers;
using Fabrino.Views.DashBoard;


namespace Fabrino
{
    public partial class MainWindow : Window
    {
        private IUserRepository repository;
        private AuthController authController;

        public MainWindow()
        {
            InitializeComponent();
            var dbContext = new AppDbContext();
            repository = new SqlUserRepository(dbContext);
            authController = new AuthController(repository);
        }


        private void GoToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            SignUpWindow registerWindow = new SignUpWindow();
            registerWindow.Show();
            this.Close(); 
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
            string username = UsernameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwordHash))
            {
                MessageBox.Show("لطفاً نام کاربری و رمز عبور را وارد کنید.");
                return;
            }

            if (authController.Login(username, passwordHash))
            {
                var user = repository.GetUserByUsername(username);
                var dashboard = new Dashboard(user);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است!");
            }
        }
    }

}

