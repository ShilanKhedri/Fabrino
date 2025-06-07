using Fabrino.Models;
using Fabrino.Services;
using System.Windows;

namespace Fabrino.Views.SellerDashBoard
{
    public partial class AddCustomerWindow : Window
    {
        private readonly SellerOrderService _service;
        public Customer NewCustomer { get; private set; }

        public AddCustomerWindow(SellerOrderService service)
        {
            InitializeComponent();
            _service = service;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("لطفاً نام مشتری را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("لطفاً شماره تماس را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                NewCustomer = _service.CreateCustomer(
                    FullNameTextBox.Text.Trim(),
                    PhoneTextBox.Text.Trim(),
                    AddressTextBox.Text.Trim()
                );

                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"خطا در ثبت مشتری: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 