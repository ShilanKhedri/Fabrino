using Fabrino.Models;
using Fabrino.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace Fabrino.Views.DashBoard
{
    public partial class OrderPage : Page
    {
        private readonly PurchaseOrderService _service;
        private readonly List<CartItem> _cartItems = new List<CartItem>();
        private decimal _totalAmount = 0;
        private readonly CultureInfo _persianCulture = new CultureInfo("fa-IR");
        private static readonly Regex _numericRegex = new Regex("[^0-9.]"); // فقط اعداد و نقطه اعشار

        public OrderPage()
        {
            InitializeComponent();
            _service = new PurchaseOrderService(new AppDbContext());

            // تنظیم event handlers
            UnitPrice.PreviewTextInput += NumericTextBox_PreviewTextInput;
            QuantityBox.PreviewTextInput += NumericTextBox_PreviewTextInput;
            UnitPrice.TextChanged += UnitPrice_TextChanged;
            QuantityBox.TextChanged += QuantityBox_TextChanged;

            LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                // Load suppliers - only active suppliers
                var suppliers = _service.GetSuppliers()
                    .Where(s => s.is_active)
                    .OrderBy(s => s.Name)
                    .ToList();
                SupplierName.ItemsSource = suppliers;
                SupplierName.DisplayMemberPath = "Name";
                SupplierName.SelectedValuePath = "SupplierID";

                // Load fabrics
                var fabrics = _service.GetFabrics()
                    .OrderBy(f => f.Name)
                    .ToList();
                ProductName.ItemsSource = fabrics;
                ProductName.DisplayMemberPath = "Name";
                ProductName.SelectedValuePath = "FabricID";

                // Initialize cart
                CartGrid.ItemsSource = _cartItems;
                UpdateTotalAmount();

                // تنظیم مقدار اولیه متراژ
                QuantityBox.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری اطلاعات: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatQuantity(decimal value)
        {
            // اگر عدد صحیح است، بدون اعشار نمایش بده
            if (value == Math.Floor(value))
            {
                return value.ToString("N0", _persianCulture);
            }
            // در غیر این صورت با یک رقم اعشار نمایش بده
            return value.ToString("N1", _persianCulture);
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numericRegex.IsMatch(e.Text);
        }

        private void UnitPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                // حذف کاراکترهای غیر عددی به جز نقطه اعشار
                string numericValue = new string(textBox.Text.Where(c => char.IsDigit(c) || c == '.').ToArray());
                
                if (decimal.TryParse(numericValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
                {
                    // ذخیره موقعیت کرسر
                    int caretIndex = textBox.CaretIndex;
                    
                    // فرمت کردن عدد با جداکننده هزارگان
                    textBox.Text = value.ToString("N0", _persianCulture);
                    
                    // بازگرداندن کرسر به موقعیت مناسب
                    if (caretIndex <= textBox.Text.Length)
                        textBox.CaretIndex = caretIndex;
                }
            }
        }

        private void QuantityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                // حذف کاراکترهای غیر عددی به جز نقطه اعشار
                string numericValue = new string(textBox.Text.Where(c => char.IsDigit(c) || c == '.').ToArray());
                
                if (decimal.TryParse(numericValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
                {
                    // ذخیره موقعیت کرسر
                    int caretIndex = textBox.CaretIndex;
                    
                    // فرمت کردن عدد با یک رقم اعشار فقط اگر لازم باشد
                    textBox.Text = FormatQuantity(value);
                    
                    // بازگرداندن کرسر به موقعیت مناسب
                    if (caretIndex <= textBox.Text.Length)
                        textBox.CaretIndex = caretIndex;
                }
            }
        }

        private void SupplierName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupplierName.SelectedItem is Supplier supplier)
            {
                SupplierPhone.Text = supplier.Phone;
                SupplierAddress.Text = supplier.Address;
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(QuantityBox.Text.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
            {
                QuantityBox.Text = FormatQuantity(quantity + 1);
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(QuantityBox.Text.Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity) && quantity > 1)
            {
                QuantityBox.Text = FormatQuantity(quantity - 1);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductName.SelectedItem is not Fabric selectedFabric)
                {
                    MessageBox.Show("لطفاً پارچه مورد نظر را انتخاب کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var quantityText = QuantityBox.Text.Replace(",", "").Replace("٫", "").Trim();
                if (!decimal.TryParse(quantityText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
                {
                    MessageBox.Show("لطفاً متراژ معتبر وارد کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // حذف همه جداکننده‌های هزارگان (کاما و ممیز فارسی)
                var unitPriceText = UnitPrice.Text.Replace(",", "").Replace("٫", "").Trim();
                decimal unitPrice;
                // سعی در تبدیل با فرهنگ فارسی
                if (!decimal.TryParse(unitPriceText, NumberStyles.Any, _persianCulture, out unitPrice))
                {
                    // اگر با فرهنگ فارسی نشد، با فرهنگ انگلیسی امتحان کن
                    if (!decimal.TryParse(unitPriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out unitPrice))
                    {
                        MessageBox.Show("لطفاً قیمت معتبر وارد کنید.", "خطا",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var cartItem = new CartItem
                {
                    FabricID = selectedFabric.FabricID,
                    FabricName = selectedFabric.Name,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = quantity * unitPrice
                };

                _cartItems.Add(cartItem);
                CartGrid.Items.Refresh();
                UpdateTotalAmount();

                // Reset input fields
                ProductName.SelectedIndex = -1;
                QuantityBox.Text = "1";
                UnitPrice.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در افزودن به سبد خرید: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CartItem item)
            {
                _cartItems.Remove(item);
                CartGrid.Items.Refresh();
                UpdateTotalAmount();
            }
        }

        private void UpdateTotalAmount()
        {
            _totalAmount = _cartItems.Sum(item => item.TotalPrice);
            TotalAmountText.Text = $"{_totalAmount.ToString("N0", _persianCulture)} تومان";
        }

        private void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SupplierName.SelectedItem is not Supplier selectedSupplier)
                {
                    MessageBox.Show("لطفاً تامین‌کننده را انتخاب کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_cartItems.Any())
                {
                    MessageBox.Show("سبد خرید خالی است.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var order = new PurchaseOrder
                {
                    SupplierID = selectedSupplier.SupplierID,
                    OrderDate = DateTime.Now,
                    ExpectedDeliveryDate = DateTime.Now, // تاریخ تحویل همان زمان حال
                    TotalAmount = _totalAmount,
                    Status = "Pending"
                };

                var orderItems = _cartItems.Select(item => new PurchaseOrderItem
                {
                    FabricID = item.FabricID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ReceivedQuantity = 0
                }).ToList();

                _service.CreatePurchaseOrder(order, orderItems);

                MessageBox.Show("سفارش با موفقیت ثبت شد.", "موفقیت",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                _cartItems.Clear();
                CartGrid.Items.Refresh();
                UpdateTotalAmount();
                SupplierName.SelectedIndex = -1;
                SupplierPhone.Text = "";
                SupplierAddress.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت سفارش: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Allow digits and one decimal point
            if (e.Text == "." && sender is TextBox textBox)
            {
                // Only allow one decimal point
                if (textBox.Text.Contains("."))
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
                return;
            }

            e.Handled = !decimal.TryParse(e.Text, out _);
        }

        private void NumberValidationKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }

    public class CartItem
    {
        public int FabricID { get; set; }
        public string FabricName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public string QuantityFormatted => FormatQuantity(Quantity);
        public string UnitPriceFormatted => UnitPrice.ToString("N0", new CultureInfo("fa-IR"));
        public string TotalPriceFormatted => TotalPrice.ToString("N0", new CultureInfo("fa-IR"));

        private string FormatQuantity(decimal value)
        {
            var culture = new CultureInfo("fa-IR");
            return value == Math.Floor(value) ? 
                value.ToString("N0", culture) : 
                value.ToString("N1", culture);
        }
    }
}