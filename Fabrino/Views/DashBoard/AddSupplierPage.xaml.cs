using Fabrino.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class AddSupplierPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public AddSupplierPage()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // اعتبارسنجی داده‌های اجباری
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("نام تامین‌کننده الزامی است.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    NameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                {
                    MessageBox.Show("شماره تماس الزامی است.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    PhoneTextBox.Focus();
                    return;
                }

                // ایجاد تامین‌کننده جدید
                var newSupplier = new Supplier
                {
                    Name = NameTextBox.Text.Trim(),
                    ContactPerson = ContactPersonTextBox.Text?.Trim(),
                    Phone = PhoneTextBox.Text.Trim(),
                    Email = EmailTextBox.Text?.Trim(),
                    Address = AddressTextBox.Text?.Trim(),
                    TaxNumber = TaxNumberTextBox.Text?.Trim()
                };

                // ذخیره در دیتابیس
                _context.Supplier.Add(newSupplier);
                _context.SaveChanges();

                MessageBox.Show("تامین‌کننده جدید با موفقیت اضافه شد.", "موفقیت",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // بازگشت به صفحه قبلی
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره تامین‌کننده: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // بازگشت به صفحه قبلی بدون ذخیره تغییرات
            NavigationService.GoBack();
        }
    }
}