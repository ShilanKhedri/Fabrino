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
    public partial class InventoryPage : Page
    {
        public InventoryPage()
        {
            InitializeComponent();
            Loaded += InventoryPage_Loaded;
        }

        private void InventoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            LoadInventoryData();
        }

        private void LoadInventoryData()
        {
            // اینجا می‌توانید داده‌ها را از دیتابیس یا سرویس دریافت کنید
            // نمونه داده‌های تستی:
            InventoryGrid.ItemsSource = new[]
            {
                new { Name = "پارچه کتان", Category = "پارچه", Quantity = 150, UnitPrice = "120,000 تومان" },
                new { Name = "نخ پنبه", Category = "نخ", Quantity = 85, UnitPrice = "45,000 تومان" },
                new { Name = "پارچه حریر", Category = "پارچه", Quantity = 42, UnitPrice = "210,000 تومان" }
            };

            // مقادیر خلاصه
            TotalItems = "235 قلم";
            MinStockItem = "پارچه حریر (42)";
            MaxStockItem = "پارچه کتان (150)";
        }

        // Properties for data binding
        public string TotalItems { get; set; }
        public string MinStockItem { get; set; }
        public string MaxStockItem { get; set; }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            // منطق افزودن کالای جدید
            if (string.IsNullOrWhiteSpace(ItemNameBox.Text) ||
                string.IsNullOrWhiteSpace(ItemQuantityBox.Text) ||
                string.IsNullOrWhiteSpace(ItemPriceBox.Text))
            {
                MessageBox.Show("لطفاً تمام فیلدهای ضروری را پر کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // اینجا کد ذخیره به دیتابیس یا سرویس
            MessageBox.Show("کالا با موفقیت افزوده شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);

            // ریست فرم
            ItemNameBox.Text = "";
            ItemCategoryBox.SelectedIndex = -1;
            ItemQuantityBox.Text = "";
            ItemPriceBox.Text = "";

            // رفرش داده‌ها
            LoadInventoryData();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            // منطق فیلتر کردن داده‌ها
            string searchText = SearchBox.Text;
            string selectedCategory = (CategoryFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            // اینجا کد فیلتر کردن داده‌ها
            MessageBox.Show($"فیلتر اعمال شد: {searchText} - {selectedCategory}", "فیلتر", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}


