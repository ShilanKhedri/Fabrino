using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fabrino.Views.DashBoard
{
    public partial class Purchase : Window
    {
        private List<Supplier> _suppliers;

        public Purchase()
        {
            InitializeComponent();

            _suppliers = new List<Supplier>
            {
                new Supplier { Name = "شرکت الف", Phone = "0912000001", Email = "a@company.com", Type = "داخلی" },
                new Supplier { Name = "شرکت ب", Phone = "0912000002", Email = "b@company.com", Type = "خارجی" },
                new Supplier { Name = "شرکت ج", Phone = "0912000003", Email = "c@company.com", Type = "داخلی" }
            };

            SupplierGrid.ItemsSource = _suppliers;

            SortComboBox.SelectionChanged += FilterAndSort;
            FilterComboBox.SelectionChanged += FilterAndSort;
            SearchBox.TextChanged += FilterAndSort;
        }

        private void FilterAndSort(object sender, RoutedEventArgs e)
        {
            var filtered = _suppliers.AsEnumerable();

            // فیلتر بر اساس جستجو
            var searchText = SearchBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "جستجو...")
            {
                filtered = filtered.Where(s => s.Name.Contains(searchText) || s.Phone.Contains(searchText) || s.Email.Contains(searchText));
            }

            // فیلتر بر اساس نوع
            var filter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (filter != "همه تامین‌کننده‌ها")
            {
                filtered = filtered.Where(s => s.Type == filter);
            }

            // مرتب‌سازی
            var sort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            filtered = sort switch
            {
                "مرتب‌سازی بر اساس نام" => filtered.OrderBy(s => s.Name),
                "مرتب‌سازی بر اساس شماره تماس" => filtered.OrderBy(s => s.Phone),
                "مرتب‌سازی بر اساس ایمیل" => filtered.OrderBy(s => s.Email),
                _ => filtered
            };

            SupplierGrid.ItemsSource = filtered.ToList();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.Text == "جستجو...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "جستجو...";
                tb.Foreground = Brushes.Gray;
            }
        }
    }

    public class Supplier
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Type { get; set; } // داخلی یا خارجی
    }
}
