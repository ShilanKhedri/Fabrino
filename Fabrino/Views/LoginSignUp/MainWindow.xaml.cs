using Fabrino.Controllers;
using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Views;
using Fabrino.Views.DashBoard;
using Fabrino.Views.SellerDashBoard;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


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
            //CreateTestSeller();
        }

        private void CreateTestSeller()
        {
            using var db = new AppDbContext();

            if (!db.Users.Any(u => u.username == "seller1"))
            {
                var seller = new UserModel
                {
                    username = "seller1",
                    password_hash = SecurityHelper.ComputeSha256Hash("12345678"),
                    full_name = "فروشنده تست",
                    role = "seller",
                    security_question = "نام اولین ماشینتان چیست؟",
                    security_answer_hash = SecurityHelper.ComputeSha256Hash("پیکان"),
                    created_at = DateTime.Now
                };

                db.Users.Add(seller);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
                }

            }
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
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            if (VisiblePasswordTextBox != null && PasswordTextBox != null && EyeIcon != null)
            {
                VisiblePasswordTextBox.Text = PasswordTextBox.Password;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                VisiblePasswordTextBox.Visibility = Visibility.Visible;
                EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/eye_open.png"));
            }
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            if (VisiblePasswordTextBox != null && PasswordTextBox != null && EyeIcon != null)
            {
                PasswordTextBox.Password = VisiblePasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Visible;
                VisiblePasswordTextBox.Visibility = Visibility.Collapsed;
                EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/eye_closed.png"));
            }
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (VisiblePasswordTextBox != null &&
                VisiblePasswordTextBox.Visibility == Visibility.Visible)
            {
                VisiblePasswordTextBox.Text = PasswordTextBox.Password;
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

                if (user.role == "owner")
                {
                    var dashboard = new Dashboard(user);
                    dashboard.Show();
                }
                else if (user.role == "seller")
                {
                    var sellerDashboard = new SellerDashBoard();
                    sellerDashboard.Show();
                }
                else
                {
                    MessageBox.Show("نقش کاربر معتبر نیست.");
                    return;
                }
             }
            else
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است!");
            }
        }
    }

}

