using Fabrino.Models;
using Fabrino.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.SellerDashBoard
{
    public partial class SOrderPage : Page
    {
        private readonly SellerOrderService _service;

        public SOrderPage()
        {
            InitializeComponent();
            var db = new AppDbContext();
            _service = new SellerOrderService(db);

            LoadOrderIds();
        }

        private void LoadOrderIds()
        {
            var orders = _service.GetAllOrders();
            OrderList.Items.Clear();
            foreach (var order in orders)
            {
                OrderList.Items.Add(order.OrderID.ToString());
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string address = AddressTextBox.Text;
            string rawItems = ItemsTextBox.Text;

            var parsedItems = new List<(int, decimal)>();

            try
            {
                var items = rawItems.Split(',');
                foreach (string item in items)
                {
                    var parts = item.Split(':');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int fabricId) &&
                        decimal.TryParse(parts[1], out decimal qty))
                    {
                        parsedItems.Add((fabricId, qty));
                    }
                }

                _service.CreateOrder(name, phone, address, parsedItems);
                MessageBox.Show("سفارش با موفقیت ثبت شد.");
                LoadOrderIds();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ثبت سفارش: " + ex.Message);
            }
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            OrderIdTextBox.Clear();
            NameTextBox.Clear();
            PhoneTextBox.Clear();
            AddressTextBox.Clear();
            ItemsTextBox.Clear();
            ItemCountTextBox.Clear();
            TotalPriceTextBox.Clear();
        }
    }
}
