using Fabrino.Controllers;
using Fabrino.Helpers;
using Fabrino.Models;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views
{
    public partial class SignUpWindow : Window
    {
        private readonly SignUpController signUpController = new SignUpController();

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string fullname = FullNameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string securityAnswer = SecurityHelper.ComputeSha256Hash(SecurityAnswerTextBox.Text.Trim());


            ComboBoxItem selectedQuestion = SecurityQuestionComboBox.SelectedItem as ComboBoxItem;
            string question = selectedQuestion != null ? selectedQuestion.Content.ToString() : "";

            if (password != confirmPassword)
            {
                MessageBox.Show("رمز عبور و تکرار آن یکسان نیستند.");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullname)
                || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(securityAnswer)
                || string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("لطفاً همه‌ی فیلدها را پر کنید.");
                return;
            }

            string passwordHash = SecurityHelper.ComputeSha256Hash(password);

            var user = new UserModel
            {
                username = username,
                full_name = fullname,
                password_hash = passwordHash,
                security_question = question,
                role = "owner",
                security_answer_hash = securityAnswer
            };

            bool success = signUpController.RegisterUser(user);

            if (success)
            {
                
                MainWindow main = new MainWindow();
                this.Close();
                main.Show(); ;// یا رفتن به صفحه لاگین
                MessageBox.Show("ثبت‌نام با موفقیت انجام شد!");

            }
            else
            {
                MessageBox.Show("خطا در ثبت‌نام. لطفاً دوباره تلاش کنید.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow backToMain = new MainWindow();
            backToMain.Show();

            this.Close(); // یا برگشت به صفحه قبل
        }

        // Placeholder برای TextBox
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
