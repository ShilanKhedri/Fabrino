using Fabrino.Controllers;
using Fabrino.Views;
using System.Windows;

namespace Fabrino.Views
{
    public partial class ForgotPasswordStep1 : Window
    {
        public ForgotPasswordStep1()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            var controller = new ForgotPasswordController();
            string question = controller.GetSecurityQuestion(username);

            if (question != null)
            {
                ForgotPasswordStep2 step2 = new ForgotPasswordStep2(username, question);
                step2.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("کاربری با این نام یافت نشد.");
            }
        }
    }
}
