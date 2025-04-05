using System;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace Fabrino
{
    public partial class MainWindow : Window
    {
        // رشته اتصال به دیتابیس (باید مقادیر صحیح را جایگزین کنی)
        private string connectionString = "Server=SHILAN;Database=Fabrino;Integrated Security=True;Encrypt=False;";

        public MainWindow()
        {
            InitializeComponent();
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
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("لطفاً نام کاربری و رمز عبور را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsValidUser(username, password))
            {
                MessageBox.Show("ورود موفق!", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                // اینجا می‌تونی کاربر رو به صفحه اصلی هدایت کنی
            }
            else
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است!", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidUser(string username, string password)
        {
            bool isValid = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password_hash = HASHBYTES('SHA2_256', @password)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    isValid = count > 0;
                }
            }
            return isValid;
        }
    }

}

