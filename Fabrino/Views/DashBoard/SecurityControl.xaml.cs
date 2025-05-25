using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class SecurityControl : UserControl
    {
        public SecurityControl()
        {
            InitializeComponent();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string current = CurrentPasswordBox.Password;
            string newPass = NewPasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirm))
            {
                PasswordMessage.Text = "لطفاً همه‌ی فیلدها را پر کنید.";
                PasswordMessage.Visibility = Visibility.Visible;
                return;
            }

            if (newPass != confirm)
            {
                PasswordMessage.Text = "رمز عبور جدید با تکرار آن مطابقت ندارد.";
                PasswordMessage.Visibility = Visibility.Visible;
                return;
            }

            // اگر بررسی رمز فعلی صحیح باشد:
            if (current != "password123") // نمونه بررسی؛ در پروژه واقعی رمز فعلی را از پایگاه‌داده بررسی کنید
            {
                PasswordMessage.Text = "رمز عبور فعلی اشتباه است.";
                PasswordMessage.Visibility = Visibility.Visible;
                return;
            }

            // ذخیره رمز جدید
            PasswordMessage.Text = "رمز عبور با موفقیت تغییر یافت.";
            PasswordMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            PasswordMessage.Visibility = Visibility.Visible;
        }

        private void ChangeEmail_Click(object sender, RoutedEventArgs e)
        {
            string currentEmail = CurrentEmailBox.Text;
            string newEmail = NewEmailBox.Text;

            if (string.IsNullOrWhiteSpace(currentEmail) || string.IsNullOrWhiteSpace(newEmail))
            {
                EmailMessage.Text = "لطفاً هر دو ایمیل را وارد کنید.";
                EmailMessage.Visibility = Visibility.Visible;
                return;
            }

            if (currentEmail != "user@example.com") // نمونه بررسی
            {
                EmailMessage.Text = "ایمیل فعلی صحیح نیست.";
                EmailMessage.Visibility = Visibility.Visible;
                return;
            }

            // ذخیره ایمیل جدید
            EmailMessage.Text = "ایمیل با موفقیت تغییر یافت.";
            EmailMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            EmailMessage.Visibility = Visibility.Visible;
        }
    }
}
