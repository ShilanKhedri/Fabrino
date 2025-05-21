using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fabrino.Views.DashBoard
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                RoleBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("لطفاً همه فیلدها را پر کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("رمز عبور و تکرار آن یکسان نیستند.", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

          MessageBox.Show("کاربر با موفقیت ثبت شد.", "ثبت کاربر", MessageBoxButton.OK, MessageBoxImage.Information);

            ErrorMessage.Visibility = Visibility.Collapsed;
            SuccessMessage.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ErrorMessage.Text = "لطفاً تمام فیلدها را پر کرده و رمز عبور را به‌درستی تکرار کنید.";
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // ذخیره اطلاعات...
            SuccessMessage.Text = "ثبت با موفقیت انجام شد.";
            SuccessMessage.Visibility = Visibility.Visible;
        }
        
    }
}

