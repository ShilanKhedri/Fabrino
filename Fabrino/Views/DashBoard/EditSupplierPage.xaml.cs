using Fabrino.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class EditSupplierPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly Supplier _supplierToEdit;

        public EditSupplierPage(Supplier supplier)
        {
            InitializeComponent();
            _supplierToEdit = supplier ?? throw new ArgumentNullException(nameof(supplier));
            LoadSupplierData();
        }

        private void LoadSupplierData()
        {
            // بارگذاری داده‌های تامین‌کننده در فرم
            NameTextBox.Text = _supplierToEdit.Name;
            PhoneTextBox.Text = _supplierToEdit.Phone;
            EmailTextBox.Text = _supplierToEdit.Email;
            AddressTextBox.Text = _supplierToEdit.Address;
            TaxNumberTextBox.Text = _supplierToEdit.TaxNumber;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // اعتبارسنجی داده‌ها
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("نام تامین‌کننده الزامی است.", "خطا");
                    return;
                }

                // بروزرسانی اطلاعات تامین‌کننده
                _supplierToEdit.Name = NameTextBox.Text.Trim();
                _supplierToEdit.Phone = PhoneTextBox.Text?.Trim();
                _supplierToEdit.Email = EmailTextBox.Text?.Trim();
                _supplierToEdit.Address = AddressTextBox.Text?.Trim();
                _supplierToEdit.TaxNumber = TaxNumberTextBox.Text?.Trim();

                // ذخیره تغییرات
                _context.SaveChanges();

                MessageBox.Show("تغییرات با موفقیت ذخیره شد.", "موفقیت");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره تغییرات: {ex.Message}", "خطا");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // بازگشت به صفحه قبلی بدون ذخیره تغییرات
            NavigationService.GoBack();
        }
    }
}