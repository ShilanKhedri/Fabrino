using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class BackupControl : UserControl
    {
        public BackupControl()
        {
            InitializeComponent();
        }

        private void BrowsePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "انتخاب مسیر ذخیره پشتیبان",
                Filter = "فایل پشتیبان (*.bak)|*.bak",
                FileName = "backup_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak"
            };

            if (dialog.ShowDialog() == true)
            {
                BackupPathBox.Text = dialog.FileName;
            }
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            string path = BackupPathBox.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("لطفاً مسیر ذخیره پشتیبان را مشخص کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // نمونه ایجاد فایل پشتیبان فرضی
                File.WriteAllText(path, "این یک فایل پشتیبان آزمایشی است.\nتاریخ: " + System.DateTime.Now.ToString());

                BackupMessage.Text = "پشتیبان با موفقیت ذخیره شد.";
                BackupMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                BackupMessage.Visibility = Visibility.Visible;
            }
            catch
            {
                BackupMessage.Text = "خطا در ذخیره فایل پشتیبان.";
                BackupMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                BackupMessage.Visibility = Visibility.Visible;
            }
        }
    }
}
