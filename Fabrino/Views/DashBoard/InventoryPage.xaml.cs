using Fabrino.Models;
using Fabrino.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class InventoryPage : Page
    {
        private InventoryService _service;
        private SupplierService _supplierService;

        public InventoryPage()
        {
            InitializeComponent();
            var db = new AppDbContext();
            _service = new InventoryService(db);
            _supplierService = new SupplierService(db);
            Loaded += InventoryPage_Loaded;
        }

        private void InventoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInventoryData();
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            var suppliers = _supplierService.GetAllSuppliers();

            SupplierComboBox.Items.Clear();
            foreach (var s in suppliers)
            {
                var item = new ComboBoxItem
                {
                    Content = s.Name,
                    Tag = s.SupplierID
                };
                SupplierComboBox.Items.Add(item);
            }

            if (SupplierComboBox.Items.Count > 0)
                SupplierComboBox.SelectedIndex = 0;
        }


        private void LoadInventoryData()
        {
            var items = _service.GetAll();

            InventoryGrid.ItemsSource = items.Select(x => new
            {
                Name = x.Name,
                Quantity = x.Quantity,
                PricePerMeter = $"{x.PricePerMeter:N0} تومان"
            }).ToList();

            if (items.Any())
            {
                TotalItemsText.Text = $"{items.Sum(i => i.Quantity)} متر";
                MaxStockText.Text = items.OrderByDescending(i => i.Quantity).First().Name + $" ({items.Max(i => i.Quantity)})";
                MinStockText.Text = items.OrderBy(i => i.Quantity).First().Name + $" ({items.Min(i => i.Quantity)})";
            }
            else
            {
                TotalItemsText.Text = MinStockText.Text = MaxStockText.Text = "ندارد";
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemNameBox.Text) ||
                string.IsNullOrWhiteSpace(ItemQuantityBox.Text) ||
                string.IsNullOrWhiteSpace(ItemPriceBox.Text))
            {
                MessageBox.Show("لطفاً همه فیلدها را پر کنید");
                return;
            }

            

            try
            {
                string name = ItemNameBox.Text;
                string color = ItemColorBox.Text;
                string material = ItemMaterialBox.Text;
                decimal width = decimal.Parse(ItemWidthBox.Text);
                decimal quantity = decimal.Parse(ItemQuantityBox.Text);
                decimal price = decimal.Parse(ItemPriceBox.Text);
                int supplierId = int.Parse((SupplierComboBox.SelectedItem as ComboBoxItem).Tag.ToString());
                var selectedSupplier = SupplierComboBox.SelectedItem as ComboBoxItem;
                if (selectedSupplier == null)
                {
                    MessageBox.Show("لطفاً تأمین‌کننده را انتخاب کنید.");
                    return;
                }

                _service.AddItem(name, color, material, width, quantity, price, supplierId);

                MessageBox.Show("کالا با موفقیت افزوده شد");
                LoadInventoryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}");
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text?.Trim() ?? "";

            var allItems = _service.GetAll();

            var filtered = allItems.Where(i =>
                string.IsNullOrEmpty(searchText) || i.Name.Contains(searchText)
            );

            InventoryGrid.ItemsSource = filtered.Select(x => new
            {
                Name = x.Name,
                Quantity = x.Quantity,
                PricePerMeter = $"{x.PricePerMeter:N0} تومان"
            }).ToList();
        }
    }
}
