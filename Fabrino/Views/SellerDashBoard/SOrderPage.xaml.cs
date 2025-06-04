using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fabrino.Views.SellerDashBoard
{
    /// <summary>
    /// Interaction logic for SOrderPage.xaml
    /// </summary>
    public partial class SOrderPage : Page
    {
        public SOrderPage()
        {
            InitializeComponent();
            LoadOrders();
        }
        private void LoadOrders()
        {
            OrderList.Items.Add("123456");
            OrderList.Items.Add("123457");
            OrderList.Items.Add("123458");
            OrderList.Items.Add("123459");
            OrderList.Items.Add("123460");
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string orderId = OrderIdTextBox.Text;
            string name = NameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string address = AddressTextBox.Text;
            string items = ItemsTextBox.Text;
            string itemCount = ItemCountTextBox.Text;
            string totalPrice = TotalPriceTextBox.Text;

            if (string.IsNullOrWhiteSpace(orderId) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("لطفاً تمامی فیلدهای ضروری را پر کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("سفارش ثبت شد.", "تایید", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearForm();
        }

        private void GenerateInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فاکتور ایجاد شد.", "فاکتور", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearForm()
        {
            OrderIdTextBox.Text = "";
            NameTextBox.Text = "";
            PhoneTextBox.Text = "";
            AddressTextBox.Text = "";
            ItemsTextBox.Text = "";
            ItemCountTextBox.Text = "";
            TotalPriceTextBox.Text = "";
        }
    }
}

    }

}
