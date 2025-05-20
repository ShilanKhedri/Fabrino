using System.Windows;
using Fabrino.Controllers;
using Fabrino.Helpers;

namespace Fabrino.Views
{
    public partial class ForgotPasswordStep2 : Window
    {
        private readonly string _username;
        private readonly ForgotPasswordController _controller;

        public ForgotPasswordStep2(string username, string question)
        {
            InitializeComponent();
            _username = username;
            _controller = new ForgotPasswordController(); // ایجاد کنترلر EF

            AnswerTextBox.Text = question; // نمایش سوال امنیتی
            
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (QuestionTextBox.Text == "پاسخ")
            {
                QuestionTextBox.Text = "";
                QuestionTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnswerTextBox.Text))
            {
                QuestionTextBox.Text = "پاسخ";
                QuestionTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string answer = QuestionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(answer) || answer == "پاسخ")
            {
                MessageBox.Show("لطفاً پاسخ سوال امنیتی را وارد کنید.");
                return;
            }

            bool isValid = _controller.ValidateSecurityAnswer(_username, answer);

            if (isValid)
            {
                ForgotPasswordStep3 step3 = new ForgotPasswordStep3(_username);
                step3.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("پاسخ نادرست است. لطفاً مجدداً تلاش کنید.");
            }
        }

        private void QuestionTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            
        }
    }
}