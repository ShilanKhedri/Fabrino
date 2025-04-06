using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fabrino.Views
{
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        // Placeholder برای TextBoxها
        private void RemoveText(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && (tb.Text == "نام کاربری" || tb.Text == "نام کامل" || tb.Text == "پاسخ سوال امنیتی"))
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb.Name == "UsernameTextBox") tb.Text = "نام کاربری";
                else if (tb.Name == "FullNameTextBox") tb.Text = "نام کامل";
                else if (tb.Name == "SecurityAnswerTextBox") tb.Text = "پاسخ سوال امنیتی";

                tb.Foreground = Brushes.Gray;
            }
        }

        // Placeholder برای PasswordBoxها
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        // ثبت‌نام
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // بررسی فیلدها
            if (UsernameTextBox.Text == "نام کاربری" || string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                FullNameTextBox.Text == "نام کامل" || string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password) ||
                SecurityQuestionComboBox.SelectedItem == null ||
                SecurityAnswerTextBox.Text == "پاسخ سوال امنیتی" || string.IsNullOrWhiteSpace(SecurityAnswerTextBox.Text))
            {
                MessageBox.Show("لطفاً همه‌ی فیلدها را کامل کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی تطابق رمز
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("رمز عبور و تکرار آن یکسان نیستند.", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // گرفتن سوال امنیتی انتخاب شده
            var selectedItem = SecurityQuestionComboBox.SelectedItem as ComboBoxItem;
            string securityQuestion = selectedItem?.Content.ToString();

            // حالا اینجا می‌تونی اطلاعات رو توی دیتابیس ذخیره کنی
            // فعلاً فقط پیام موفقیت:
            MessageBox.Show("ثبت‌نام با موفقیت انجام شد!", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);

            // بستن پنجره یا بردن به صفحه ورود
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            Close();
        }
    }
}
