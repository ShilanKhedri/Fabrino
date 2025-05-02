using System.Windows;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Fabrino.Helpers;
using Fabrino.Controllers;

namespace Fabrino.Views
{
    public partial class ForgotPasswordStep3 : Window
    {
        string username;
        public ForgotPasswordStep3(string user)
        {
            InitializeComponent();
            username = user;
        }



        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            NewPasswordPlaceholder.Visibility = string.IsNullOrEmpty(NewPasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newPass = NewPasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (newPass != confirm)
            {
                MessageBox.Show("رمز عبور و تکرار آن یکسان نیستند.");
                return;
            }

            var controller = new ForgotPasswordController();
            if (controller.ResetPassword(username, newPass))
            {
                MessageBox.Show("رمز عبور با موفقیت تغییر کرد.");
                this.Close(); // یا برگرد به صفحه ورود
            }
            else
            {
                MessageBox.Show("خطا در تغییر رمز عبور.");
            }
        }
    }
}
