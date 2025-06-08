using Fabrino.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class EditSupplierPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly Supplier _supplier;
        
        // Add event for supplier update notification
        public event EventHandler SupplierUpdated;

        public EditSupplierPage(Supplier supplier)
        {
            InitializeComponent();
            _supplier = supplier;
            LoadSupplierData();
        }

        private void LoadSupplierData()
        {
            // بارگذاری داده‌های تامین‌کننده در فرم
            NameTextBox.Text = _supplier.Name;
            ContactPersonTextBox.Text = _supplier.ContactPerson;
            PhoneTextBox.Text = _supplier.Phone;
            EmailTextBox.Text = _supplier.Email;
            AddressTextBox.Text = _supplier.Address;
            TaxNumberTextBox.Text = _supplier.TaxNumber;
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
                _supplier.Name = NameTextBox.Text.Trim();
                _supplier.ContactPerson = ContactPersonTextBox.Text?.Trim();
                _supplier.Phone = PhoneTextBox.Text?.Trim();
                _supplier.Email = EmailTextBox.Text?.Trim();
                _supplier.Address = AddressTextBox.Text?.Trim();
                _supplier.TaxNumber = TaxNumberTextBox.Text?.Trim();

                // ذخیره تغییرات
                _context.Supplier.Update(_supplier);
                _context.SaveChanges();

                MessageBox.Show("تغییرات با موفقیت ذخیره شد.", "موفقیت");
                
                // Notify listeners that supplier was updated
                SupplierUpdated?.Invoke(this, EventArgs.Empty);
                
                // بازگشت به صفحه قبلی بدون ذخیره تغییرات
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