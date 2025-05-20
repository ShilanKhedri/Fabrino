using Fabrino.Controllers;
using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fabrino.Views
{
    public partial class SignUpWindow : Window
    {
        private readonly SignUpController _controller;
        private readonly PasswordValidationService _passwordValidator;


        public SignUpWindow()
        {
            InitializeComponent();
            _controller = new SignUpController();
            _passwordValidator = new PasswordValidationService();

        }
        //registering user
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string fullname = FullNameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string securityAnswer = SecurityHelper.ComputeSha256Hash(SecurityAnswerTextBox.Text.Trim());
            string question = (SecurityQuestionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();


            // اعتبارسنجی‌ها
            if (!UserValidationService.ValidatePassword(password, confirmPassword))
            {
                MessageBox.Show("رمز عبور باید حداقل ۸ کاراکتر و با تکرار آن مطابقت داشته باشد!");
                return;
            }

            if (!UserValidationService.ValidateUsername(username))
            {
                MessageBox.Show("نام کاربری باید حداقل ۳ کاراکتر باشد!");
                return;
            }

            if (!UserValidationService.ValidateSecurityQuestion(question))
            {
                MessageBox.Show("لطفاً یک سوال امنیتی انتخاب کنید!");
                return;
            }

            if (!UserValidationService.ValidateSecurityAnswer(securityAnswer))
            {
                MessageBox.Show("پاسخ سوال امنیتی نمی‌تواند خالی باشد!");
                return;
            }

            if (!UserValidationService.ValidateUsernameFormat(UsernameTextBox.Text.Trim()))
            {
                MessageBox.Show("نام کاربری فقط می‌تواند شامل حروف انگلیسی، اعداد و زیرخط باشد");
                return;
            }

            var user = new UserModel
            {
                username = username,
                full_name = fullname,
                password_hash = SecurityHelper.ComputeSha256Hash(password),
                security_question = question,
                security_answer_hash = securityAnswer,
                role = "owner",
                created_at = DateTime.Now
            };

            bool success = _controller.RegisterUser(user);

            if (success)
            {
                MessageBox.Show("ثبت‌نام با موفقیت انجام شد!");
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("خطا در ثبت‌نام. لطفاً دوباره تلاش کنید.");
            }
        }

        private void FullNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FullNameTextBox.IsFocused)
            {
                bool isValid = UserValidationService.ValidatePersianName(FullNameTextBox.Text);
                FullNameTextBox.BorderBrush = isValid ? Brushes.Gray : Brushes.Red;
           
            }
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // نمایش/مخفی کردن placeholder
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;

            // نمایش قدرت رمز عبور
            var strength = _passwordValidator.CheckPasswordStrength(PasswordBox.Password);
            PasswordStrengthText.Text = strength.GetDescription();
            PasswordStrengthText.Foreground = strength.GetColor();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;

            if (!string.IsNullOrEmpty(PasswordBox.Password) &&
               !string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MatchPasswordsText.Visibility = PasswordBox.Password == ConfirmPasswordBox.Password
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow backToMain = new MainWindow();
            backToMain.Show();

            this.Close(); // back to login page
        }

        // Placeholder for TextBox
        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Foreground == System.Windows.Media.Brushes.Gray)
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb.Name == "UsernameTextBox") tb.Text = "نام کاربری";
                else if (tb.Name == "FullNameTextBox") tb.Text = "نام کامل";
                else if (tb.Name == "SecurityAnswerTextBox") tb.Text = "پاسخ سوال امنیتی";

                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsernameTextBox.IsFocused) // فقط وقتی کاربر تایپ می‌کند
            {
                UserValidationService.ValidateUsernameRealTime(UsernameTextBox);
            }
        }
        
    }
}
