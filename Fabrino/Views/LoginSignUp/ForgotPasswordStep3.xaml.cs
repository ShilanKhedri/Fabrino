using System.Windows;
using Fabrino.Controllers;
using Fabrino.Helpers;
using Fabrino.Services;

namespace Fabrino.Views
{
    public partial class ForgotPasswordStep3 : Window
    {
        private readonly string _username;
        private readonly ForgotPasswordController _controller;
        private readonly PasswordValidationService _passwordValidator;

        public ForgotPasswordStep3(string username)
        {
            InitializeComponent();
            _username = username;
            _controller = new ForgotPasswordController();
            _passwordValidator = new PasswordValidationService();

            // تنظیمات اولیه
            NewPasswordPlaceholder.Visibility = Visibility.Visible;
            ConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // نمایش/مخفی کردن placeholder
            NewPasswordPlaceholder.Visibility = string.IsNullOrEmpty(NewPasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;

            // اعتبارسنجی قدرت رمز 
            var strength = _passwordValidator.CheckPasswordStrength(NewPasswordBox.Password);
            PasswordStrengthText.Text = strength.GetDescription();
            PasswordStrengthText.Foreground = strength.GetColor();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;

            // نمایش خطا اگر رمزها مطابقت ندارند (اختیاری)
            if (!string.IsNullOrEmpty(NewPasswordBox.Password) &&
                !string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MatchPasswordsText.Visibility = NewPasswordBox.Password == ConfirmPasswordBox.Password
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string newPass = NewPasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            // اعتبارسنجی اولیه
            if (string.IsNullOrWhiteSpace(newPass))
            {
                MessageBox.Show("لطفاً رمز عبور جدید را وارد کنید.");
                return;
            }

            if (newPass != confirm)
            {
                MessageBox.Show("رمز عبور و تکرار آن یکسان نیستند.");
                return;
            }

            // اعتبارسنجی قدرت رمز
            if (!_passwordValidator.IsPasswordStrongEnough(newPass))
            {
                MessageBox.Show("رمز عبور باید حداقل ۸ کاراکتر و شامل حروف و اعداد باشد.");
                return;
            }

            // تغییر رمز عبور
            bool success = _controller.ResetPassword(_username, newPass);

            if (success)
            {
                MessageBox.Show("رمز عبور با موفقیت تغییر کرد.");
                this.Close();
            }
            else
            {
                MessageBox.Show("خطا در تغییر رمز عبور. لطفاً مجدداً تلاش کنید.");
            }
        }
    }
}