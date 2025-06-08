using Fabrino.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class AddSupplierPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        // Add event for supplier addition notification
        public event EventHandler SupplierAdded;

        public AddSupplierPage()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                {
                    MessageBox.Show("لطفاً نام و شماره تماس را وارد کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create new supplier
                var supplier = new Supplier
                {
                    Name = NameTextBox.Text.Trim(),
                    ContactPerson = ContactPersonTextBox.Text?.Trim(),
                    Phone = PhoneTextBox.Text.Trim(),
                    Email = EmailTextBox.Text?.Trim(),
                    Address = AddressTextBox.Text?.Trim(),
                    TaxNumber = TaxNumberTextBox.Text?.Trim(),
                    is_active = true
                };

                // Save to database
                _context.Supplier.Add(supplier);
                _context.SaveChanges();

                MessageBox.Show("تامین‌کننده جدید با موفقیت اضافه شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Notify listeners that supplier was added
                SupplierAdded?.Invoke(this, EventArgs.Empty);
                
                // Navigate back
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره اطلاعات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}