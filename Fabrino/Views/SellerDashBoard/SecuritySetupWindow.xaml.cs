using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace Fabrino.Views
{
    public partial class SecuritySetupWindow : Window
    {
        private readonly UserModel _user;
        private readonly AppDbContext _context;

        public SecuritySetupWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            _context = new AppDbContext();
        }

        

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionCombo.SelectedItem is not SecurityQuestion selectedQuestion ||
                string.IsNullOrWhiteSpace(AnswerBox.Text))
            {
                MessageBox.Show("لطفاً یک سؤال انتخاب کرده و پاسخ را وارد کنید.");
                return;
            }

            _user.security_question = selectedQuestion.QuestionText;
            _user.security_answer_hash = AnswerBox.Text.Trim();
            _context.Users.Update(_user);
            _context.SaveChanges();

            MessageBox.Show("سؤال امنیتی ثبت شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
