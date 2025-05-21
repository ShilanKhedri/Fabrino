using Fabrino.Models;
using Fabrino.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class OrderPage : Page
    {
        private readonly PurchaseOrderService _orderService;
        private ObservableCollection<Supplier> _suppliers;
        private ObservableCollection<Fabric> _fabrics;

        public OrderPage()
        {
            InitializeComponent();
            _orderService = new PurchaseOrderService(new AppDbContext());
            LoadData();
        }

        private void LoadData()
        {
            _suppliers = new ObservableCollection<Supplier>(_orderService.GetSuppliers());
            _fabrics = new ObservableCollection<Fabric>(_orderService.GetFabrics());

            SupplierName.ItemsSource = _suppliers;
            SupplierName.DisplayMemberPath = "Name";
            SupplierName.SelectedValuePath = "SupplierID";

            ProductName.ItemsSource = _fabrics;
            ProductName.DisplayMemberPath = "Name";
            ProductName.SelectedValuePath = "FabricID";
        }

        private void SupplierName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupplierName.SelectedItem is Supplier selectedSupplier)
            {
                SupplierPhone.Text = selectedSupplier.Phone;
                SupplierAddress.Text = selectedSupplier.Address;
            }
        }

        private void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newOrder = new PurchaseOrder
                {
                    SupplierID = (int)SupplierName.SelectedValue,
                    ExpectedDeliveryDate = DeliveryDate.SelectedDate,
                    Notes = "سفارش جدید ثبت شده از طریق سیستم"
                };

                var orderItem = new PurchaseOrderItem
                {
                    FabricID = (int)ProductName.SelectedValue,
                    Quantity = decimal.Parse(Quantity.Text),
                    UnitPrice = decimal.Parse(UnitPrice.Text)
                };

                _orderService.CreatePurchaseOrder(newOrder, new List<PurchaseOrderItem> { orderItem });

                MessageBox.Show($"سفارش خرید با شماره {newOrder.OrderNumber} ثبت شد!",
                    "موفقیت",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // ریست فرم
                Quantity.Text = "";
                UnitPrice.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت سفارش: {ex.Message}",
                    "خطا",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SearchOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = _orderService.GetOrderByNumber(SearchOrderNumber.Text);
            if (order != null)
            {
                SupplierName.SelectedValue = order.SupplierID;
                ProductName.SelectedValue = order.PurchaseOrderItems.FirstOrDefault()?.FabricID;
                Quantity.Text = order.PurchaseOrderItems.FirstOrDefault()?.Quantity.ToString();
                UnitPrice.Text = order.PurchaseOrderItems.FirstOrDefault()?.UnitPrice.ToString();
                DeliveryDate.SelectedDate = order.ExpectedDeliveryDate;
            }
            else
            {
                MessageBox.Show("سفارشی با این شماره یافت نشد!",
                    "هشدار",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

    }
}