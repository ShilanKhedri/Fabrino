using System.Windows;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Fabrino.Helpers;
using Fabrino.Controllers;

namespace Fabrino.Views
{
    public partial class ForgotPasswordStep2 : Window
    {
        string username;
        public ForgotPasswordStep2(string user, string question)
        {
            InitializeComponent();
            username = user;
            AnswerTextBox.Text = question;
            
            
        }


        private void RemoveText(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && tb.Text == "پاسخ")
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
                tb.Text = "پاسخ";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string answer = QuestionTextBox.Text;
            var controller = new ForgotPasswordController();

            if (controller.ValidateSecurityAnswer(username, answer))
            {
                ForgotPasswordStep3 step3 = new ForgotPasswordStep3(username);
                step3.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("پاسخ نادرست است.");
            }
        }
    }
}
