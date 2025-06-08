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
        private Fabric _selectedFabric;

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
            InventoryGrid.ItemsSource = items;

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

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            _selectedFabric = (Fabric)button.DataContext;

            // Load suppliers into edit popup combobox
            var suppliers = _supplierService.GetAllSuppliers();
            EditSupplierComboBox.Items.Clear();
            foreach (var s in suppliers)
            {
                var item = new ComboBoxItem
                {
                    Content = s.Name,
                    Tag = s.SupplierID
                };
                EditSupplierComboBox.Items.Add(item);
            }

            // Find and select the current supplier
            for (int i = 0; i < EditSupplierComboBox.Items.Count; i++)
            {
                var item = (ComboBoxItem)EditSupplierComboBox.Items[i];
                if (int.Parse(item.Tag.ToString()) == _selectedFabric.SupplierID)
                {
                    EditSupplierComboBox.SelectedIndex = i;
                    break;
                }
            }

            EditItemNameBox.Text = _selectedFabric.Name;
            EditItemColorBox.Text = _selectedFabric.Color;
            EditItemMaterialBox.Text = _selectedFabric.Material;
            EditItemWidthBox.Text = _selectedFabric.Width.ToString();
            EditItemQuantityBox.Text = _selectedFabric.Quantity.ToString();
            EditItemPriceBox.Text = _selectedFabric.PricePerMeter.ToString();

            EditPopup.IsOpen = true;
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedFabric == null) return;

                _selectedFabric.Name = EditItemNameBox.Text;
                _selectedFabric.Color = EditItemColorBox.Text;
                _selectedFabric.Material = EditItemMaterialBox.Text;
                _selectedFabric.Width = decimal.Parse(EditItemWidthBox.Text);
                _selectedFabric.Quantity = decimal.Parse(EditItemQuantityBox.Text);
                _selectedFabric.PricePerMeter = decimal.Parse(EditItemPriceBox.Text);
                _selectedFabric.SupplierID = int.Parse(((ComboBoxItem)EditSupplierComboBox.SelectedItem).Tag.ToString());

                _service.UpdateItem(_selectedFabric);

                MessageBox.Show("تغییرات با موفقیت ذخیره شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                EditPopup.IsOpen = false;
                LoadInventoryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره تغییرات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            EditPopup.IsOpen = false;
            _selectedFabric = null;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            _selectedFabric = (Fabric)button.DataContext;
            DeleteConfirmationPopup.IsOpen = true;
        }

        private void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedFabric == null) return;

                _service.DeleteItem(_selectedFabric);

                MessageBox.Show("کالا با موفقیت حذف شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                DeleteConfirmationPopup.IsOpen = false;
                LoadInventoryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در حذف کالا: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteConfirmationPopup.IsOpen = false;
            _selectedFabric = null;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text?.Trim() ?? "";
            var allItems = _service.GetAll();
            var filtered = allItems.Where(i =>
                string.IsNullOrEmpty(searchText) || i.Name.Contains(searchText)
            );
            InventoryGrid.ItemsSource = filtered.ToList();
        }
    }
}
