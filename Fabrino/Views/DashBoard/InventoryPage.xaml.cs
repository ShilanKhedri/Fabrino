using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class InventoryPage : Page
    {
        public ObservableCollection<Item> Items { get; set; }

        public InventoryPage()
        {
            InitializeComponent();
            Items = new ObservableCollection<Item>
            {
                new Item { Name = "پارچه کتان", Category = "پارچه", Quantity = 40, UnitPrice = 50000 },
                new Item { Name = "نخ پنبه‌ای", Category = "نخ", Quantity = 25, UnitPrice = 15000 },
            };
            InventoryGrid.ItemsSource = Items;
            DataContext = this;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemNameBox.Text)) return;

            Items.Add(new Item
            {
                Name = ItemNameBox.Text,
                Category = ((ComboBoxItem)ItemCategoryBox.SelectedItem)?.Content?.ToString() ?? "",
                Quantity = int.TryParse(ItemQuantityBox.Text, out int q) ? q : 0,
                UnitPrice = int.TryParse(ItemPriceBox.Text, out int p) ? p : 0
            });

            ItemNameBox.Clear();
            ItemQuantityBox.Clear();
            ItemPriceBox.Clear();
        }

        public int TotalItems => Items.Count;
        public string MinStockItem => Items.Count > 0 ? $"{Items[0].Name} - {Items[0].Quantity}" : "ندارد";
        public string MaxStockItem => Items.Count > 0 ? $"{Items[0].Name} - {Items[0].Quantity}" : "ندارد";
    }

    public class Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
