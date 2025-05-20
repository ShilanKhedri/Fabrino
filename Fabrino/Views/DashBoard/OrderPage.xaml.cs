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

namespace Fabrino.Views.DashBoard
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
        }
        
        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag?.ToString() == "add")
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("نام کالای جدید را وارد کنید:", "افزودن کالا");
                if (!string.IsNullOrWhiteSpace(input))
                {
                    var newItem = new ComboBoxItem { Content = input };
                    ProductComboBox.Items.Insert(ProductComboBox.Items.Count - 1, newItem);
                    ProductComboBox.SelectedItem = newItem;
                }
            }
        }

        private void SupplierName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupplierName.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag?.ToString() == "add")
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("نام تامین‌کننده جدید را وارد کنید:", "افزودن تامین‌کننده");
                if (!string.IsNullOrWhiteSpace(input))
                {
                    var newItem = new ComboBoxItem { Content = input };
                    SupplierName.Items.Insert(SupplierName.Items.Count - 1, newItem);
                    SupplierName.SelectedItem = newItem;
                }
            }
        }
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuantityBox.Text, out int current))
            {
                QuantityBox.Text = (current + 1).ToString();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuantityBox.Text, out int current) && current > 0)
            {
                QuantityBox.Text = (current - 1).ToString();
            }
        }

        private void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            // بررسی مقدار کالا
            if (ProductComboBox.SelectedItem == null ||
                (ProductComboBox.SelectedItem is ComboBoxItem item1 && string.IsNullOrWhiteSpace(item1.Content?.ToString())))
            {
                MessageBox.Show("لطفاً یک کالا انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی مقدار تعداد
            if (string.IsNullOrWhiteSpace(QuantityBox.Text) || !int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("تعداد نامعتبر است.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی قیمت
            if (string.IsNullOrWhiteSpace(UnitPrice.Text) || !decimal.TryParse(UnitPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("قیمت واحد نامعتبر است.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی تاریخ
            if (DeliveryDate.SelectedDate == null)
            {
                MessageBox.Show("لطفاً تاریخ تحویل را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی تأمین‌کننده
            if (SupplierName.SelectedItem == null ||
                (SupplierName.SelectedItem is ComboBoxItem item2 && string.IsNullOrWhiteSpace(item2.Content?.ToString())))
            {
                MessageBox.Show("لطفاً یک تامین‌کننده انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی شماره تماس
            if (string.IsNullOrWhiteSpace(SupplierPhone.Text))
            {
                MessageBox.Show("لطفاً شماره تماس تامین‌کننده را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // بررسی آدرس
            if (string.IsNullOrWhiteSpace(SupplierAddress.Text))
            {
                MessageBox.Show("لطفاً آدرس تامین‌کننده را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // اگر همه‌چیز اوکی بود
            MessageBox.Show("سفارش با موفقیت ثبت شد!", "ثبت سفارش", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
