using Fabrino.Models;
using Fabrino.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.AdminDashBoard
{
    public partial class AddUserWindow : Window
    {
        private readonly AppDbContext _context;

        public AddUserWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                    string.IsNullOrWhiteSpace(FullNameBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                    string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password) ||
                    RoleComboBox.SelectedItem == null ||
                    SecurityQuestionComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(SecurityAnswerBox.Text))
                {
                    MessageBox.Show("لطفاً تمام فیلدهای ضروری را پر کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("رمز عبور و تکرار آن مطابقت ندارند.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if username exists
                var existingUser = _context.Users.FirstOrDefault(u => u.username == UsernameBox.Text.Trim());
                if (existingUser != null)
                {
                    MessageBox.Show("این نام کاربری قبلاً ثبت شده است.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create new user
                var newUser = new UserModel
                {
                    username = UsernameBox.Text.Trim(),
                    full_name = FullNameBox.Text.Trim(),
                    password_hash = SecurityHelper.ComputeSha256Hash(PasswordBox.Password),
                    role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString(),
                    Phone = PhoneBox.Text.Trim(),
                    is_active = true,
                    created_at = DateTime.Now,
                    security_question = ((ComboBoxItem)SecurityQuestionComboBox.SelectedItem).Content.ToString(),
                    security_answer_hash = SecurityAnswerBox.Text.Trim()
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت کاربر جدید: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 