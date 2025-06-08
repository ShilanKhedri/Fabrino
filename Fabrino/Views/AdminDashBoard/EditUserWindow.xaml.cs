using Fabrino.Models;
using Fabrino.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.AdminDashBoard
{
    public partial class EditUserWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly string _username;
        private UserModel _user;

        public EditUserWindow(string username)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _username = username;
            LoadUserData();
        }

        private void LoadUserData()
        {
            _user = _context.Users.FirstOrDefault(u => u.username == _username);
            if (_user == null)
            {
                MessageBox.Show("کاربر مورد نظر یافت نشد.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            UsernameBox.Text = _user.username;
            FullNameBox.Text = _user.full_name;
            PhoneBox.Text = _user.Phone;
            EmailBox.Text = _user.Email;

            // Set role
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Content.ToString() == _user.role)
                {
                    RoleComboBox.SelectedItem = item;
                    break;
                }
            }

            // Set security question
            if (!string.IsNullOrEmpty(_user.security_question))
            {
                for (int i = 0; i < SecurityQuestionComboBox.Items.Count; i++)
                {
                    var item = SecurityQuestionComboBox.Items[i] as ComboBoxItem;
                    if (item.Content.ToString() == _user.security_question)
                    {
                        SecurityQuestionComboBox.SelectedIndex = i;
                        break;
                    }
                }
                // Don't show the hashed security answer
                SecurityAnswerBox.Text = string.Empty;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Only validate full name as required
                if (string.IsNullOrWhiteSpace(FullNameBox.Text))
                {
                    MessageBox.Show("لطفاً نام کامل را وارد کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check password if changing
                if (!string.IsNullOrWhiteSpace(NewPasswordBox.Password))
                {
                    if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                    {
                        MessageBox.Show("رمز عبور جدید و تکرار آن مطابقت ندارند.", "خطا",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!IsValidPassword(NewPasswordBox.Password))
                    {
                        MessageBox.Show("رمز عبور باید حداقل 8 کاراکتر و شامل حروف بزرگ، کوچک، اعداد و علائم باشد.", "خطا",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _user.password_hash = SecurityHelper.ComputeSha256Hash(NewPasswordBox.Password);
                }

                // Update user
                _user.full_name = FullNameBox.Text.Trim();
                
                // Update role if selected
                if (RoleComboBox.SelectedItem != null)
                {
                    _user.role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();
                }
                
                // Update optional fields
                _user.Phone = PhoneBox.Text?.Trim() ?? _user.Phone;
                _user.Email = EmailBox.Text?.Trim() ?? _user.Email;

                // Update security question if selected
                if (SecurityQuestionComboBox.SelectedItem != null)
                {
                    _user.security_question = ((ComboBoxItem)SecurityQuestionComboBox.SelectedItem).Content.ToString();
                }

                // Only update security answer if a new one is provided
                if (!string.IsNullOrWhiteSpace(SecurityAnswerBox.Text))
                {
                    _user.security_answer_hash = SecurityHelper.ComputeSha256Hash(SecurityAnswerBox.Text.Trim());
                }

                _context.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ویرایش کاربر: {ex.Message}", "خطا",
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