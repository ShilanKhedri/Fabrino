using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class PersonalInfoControl : UserControl
    {
        public PersonalInfoControl()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password) ||
                RoleBox.SelectedItem == null)
            {
                ErrorMessage.Text = "لطفاً تمام فیلدها را پر کنید.";
                ErrorMessage.Visibility = Visibility.Visible;
                SuccessMessage.Visibility = Visibility.Collapsed;
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ErrorMessage.Text = "رمز عبور و تأیید آن یکسان نیستند.";
                ErrorMessage.Visibility = Visibility.Visible;
                SuccessMessage.Visibility = Visibility.Collapsed;
                return;
            }

            
            ErrorMessage.Visibility = Visibility.Collapsed;
            SuccessMessage.Text = "اطلاعات با موفقیت ذخیره شد.";
            SuccessMessage.Visibility = Visibility.Visible;

            
        }
    }
}
