using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class Purchase2Way : Page
    {
        public Purchase2Way()
        {
            InitializeComponent();
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            // هدایت به صفحه ثبت سفارش
            NavigationService.Navigate(new OrderPage());
        }

        private void btnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            // هدایت به صفحه لیست تأمین‌کنندگان
            NavigationService.Navigate(new PurchasePage());
        }
    }
}