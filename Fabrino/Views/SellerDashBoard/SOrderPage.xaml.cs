using Fabrino.Models;
using Fabrino.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fabrino.Views.SellerDashBoard
{
    public partial class SOrderPage : Page
    {
        private readonly SellerOrderService _service;
        private readonly ObservableCollection<SelectedFabricItem> _selectedFabrics;
        private Customer _selectedCustomer;
        private Fabric _selectedFabric;

        public SOrderPage()
        {
            InitializeComponent();
            var db = new AppDbContext();
            _service = new SellerOrderService(db);
            _selectedFabrics = new ObservableCollection<SelectedFabricItem>();
            
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // بارگذاری لیست سفارش‌ها
            LoadOrderIds();

            // بارگذاری لیست مشتریان
            var customers = _service.GetAllCustomers();
            CustomerComboBox.ItemsSource = customers;
            CustomerComboBox.DisplayMemberPath = "FullName";
            CustomerComboBox.SelectedValuePath = "CustomerID";

            // بارگذاری لیست پارچه‌ها
            var fabrics = _service.GetAllFabrics();
            FabricComboBox.ItemsSource = fabrics;
            FabricComboBox.SelectedValuePath = "FabricID";

            // تنظیم ItemsSource برای لیست پارچه‌های انتخاب شده
            SelectedFabricsList.ItemsSource = _selectedFabrics;
        }

        private void LoadOrderIds()
        {
            var orders = _service.GetAllOrders();
            OrderList.Items.Clear();
            foreach (var order in orders)
            {
                OrderList.Items.Add($"سفارش شماره {order.OrderID} - {order.Customer?.FullName ?? "نامشخص"}");
            }
        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCustomer = CustomerComboBox.SelectedItem as Customer;
            if (_selectedCustomer != null)
            {
                CustomerNameText.Text = _selectedCustomer.FullName;
                CustomerPhoneText.Text = _selectedCustomer.Phone;
                CustomerAddressText.Text = _selectedCustomer.Address;
            }
        }

        private void FabricComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedFabric = FabricComboBox.SelectedItem as Fabric;
            if (_selectedFabric != null)
            {
                // اگر نیاز به نمایش اطلاعات اضافی پارچه انتخاب شده دارید
                // می‌توانید اینجا اضافه کنید
                MetrageTextBox.Focus();
            }
        }

        private void AddNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow(_service)
            {
                Owner = Window.GetWindow(this)
            };

            if (addCustomerWindow.ShowDialog() == true)
            {
                // بارگذاری مجدد لیست مشتریان
                var customers = _service.GetAllCustomers();
                CustomerComboBox.ItemsSource = customers;

                // انتخاب مشتری جدید
                CustomerComboBox.SelectedItem = addCustomerWindow.NewCustomer;
            }
        }

        private void AddFabric_Click(object sender, RoutedEventArgs e)
        {
            if (FabricComboBox.SelectedItem is Fabric selectedFabric &&
                decimal.TryParse(MetrageTextBox.Text, out decimal metrage))
            {
                // بررسی موجودی
                if (metrage > selectedFabric.Quantity)
                {
                    MessageBox.Show($"موجودی پارچه {selectedFabric.Name} کافی نیست. موجودی فعلی: {selectedFabric.Quantity} متر",
                        "خطای موجودی",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // افزودن به لیست انتخاب شده‌ها
                var item = new SelectedFabricItem
                {
                    Id = selectedFabric.FabricID,
                    Name = selectedFabric.Name,
                    Metrage = metrage,
                    PricePerMeter = selectedFabric.PricePerMeter
                };

                _selectedFabrics.Add(item);
                UpdateTotalCalculations();

                // پاک کردن فیلدها
                MetrageTextBox.Clear();
                FabricComboBox.SelectedIndex = -1;
            }
        }

        private void RemoveFabric_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int fabricId)
            {
                var itemToRemove = _selectedFabrics.FirstOrDefault(f => f.Id == fabricId);
                if (itemToRemove != null)
                {
                    _selectedFabrics.Remove(itemToRemove);
                    UpdateTotalCalculations();
                }
            }
        }

        private void UpdateTotalCalculations()
        {
            decimal totalPrice = _selectedFabrics.Sum(f => f.TotalPrice);
            decimal totalMetrage = _selectedFabrics.Sum(f => f.Metrage);

            TotalPriceText.Text = $"{totalPrice:N0} تومان";
            TotalMetrageText.Text = $"مجموع متراژ: {totalMetrage:N1} متر";
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedCustomer == null)
                {
                    MessageBox.Show("لطفاً مشتری را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_selectedFabrics.Any())
                {
                    MessageBox.Show("لطفاً حداقل یک پارچه انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // تبدیل به فرمت مورد نیاز سرویس
                var items = _selectedFabrics.Select(f => (f.Id, f.Metrage)).ToList();

                // ثبت سفارش
                _service.CreateOrder(
                    _selectedCustomer.FullName,
                    _selectedCustomer.Phone,
                    _selectedCustomer.Address,
                    items);

                MessageBox.Show("سفارش با موفقیت ثبت شد.", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // پاک کردن فرم
                ClearForm();
                
                // بروزرسانی لیست سفارش‌ها
                LoadOrderIds();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت سفارش: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            CustomerComboBox.SelectedIndex = -1;
            _selectedCustomer = null;
            CustomerNameText.Text = "";
            CustomerPhoneText.Text = "";
            CustomerAddressText.Text = "";
            
            FabricComboBox.SelectedIndex = -1;
            MetrageTextBox.Clear();
            
            _selectedFabrics.Clear();
            UpdateTotalCalculations();
        }
    }

    public class SelectedFabricItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Metrage { get; set; }
        public decimal PricePerMeter { get; set; }
        public decimal TotalPrice => Metrage * PricePerMeter;
    }
}
