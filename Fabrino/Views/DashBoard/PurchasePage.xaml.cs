using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Fabrino.Views.DashBoard
{
    public partial class PurchasePage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private List<Supplier> _allSuppliers;
        private CollectionViewSource _suppliersViewSource;

        public PurchasePage()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            _allSuppliers = _context.Supplier.ToList();
            _suppliersViewSource = new CollectionViewSource { Source = _allSuppliers };
            SupplierGrid.ItemsSource = _suppliersViewSource.View;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppliersViewSource?.View == null) return;

            var searchText = SearchBox.Text == "جستجو..." ? "" : SearchBox.Text.ToLower();

            _suppliersViewSource.View.Filter = item =>
            {
                if (item is Supplier supplier)
                {
                    return supplier.Name.ToLower().Contains(searchText) ||
                           (supplier.Phone?.ToLower().Contains(searchText) ?? false) ||
                           (supplier.Email?.ToLower().Contains(searchText) ?? false) ||
                           (supplier.Address?.ToLower().Contains(searchText) ?? false);
                }
                return false;
            };
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppliersViewSource?.View == null) return;

            using (_suppliersViewSource.View.DeferRefresh())
            {
                switch (SortComboBox.SelectedIndex)
                {
                    case 0: // نام
                        _suppliersViewSource.SortDescriptions.Clear();
                        _suppliersViewSource.SortDescriptions.Add(
                            new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
                        break;
                    case 1: // شماره تماس
                        _suppliersViewSource.SortDescriptions.Clear();
                        _suppliersViewSource.SortDescriptions.Add(
                            new System.ComponentModel.SortDescription("Phone", System.ComponentModel.ListSortDirection.Ascending));
                        break;
                    case 2: // ایمیل
                        _suppliersViewSource.SortDescriptions.Clear();
                        _suppliersViewSource.SortDescriptions.Add(
                            new System.ComponentModel.SortDescription("Email", System.ComponentModel.ListSortDirection.Ascending));
                        break;
                }
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppliersViewSource?.View == null) return;

            switch (FilterComboBox.SelectedIndex)
            {
                case 0: // همه
                    _suppliersViewSource.View.Filter = null;
                    break;
                case 1: // داخلی
                    _suppliersViewSource.View.Filter = item =>
                        (item as Supplier)?.Phone?.StartsWith("0") ?? false;
                    break;
                case 2: // خارجی
                    _suppliersViewSource.View.Filter = item =>
                        !(item as Supplier)?.Phone?.StartsWith("0") ?? true;
                    break;
            }
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierGrid.SelectedItem is Supplier selectedSupplier)
            {
                // انتقال به صفحه ویرایش با ارسال تامین‌کننده انتخاب شده
                var editPage = new EditSupplierPage(selectedSupplier);
                this.NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("لطفاً یک تامین‌کننده را انتخاب کنید.", "هشدار");
            }
        }

        private async void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierGrid.SelectedItem is not Supplier selectedSupplier || selectedSupplier.SupplierID == 0)
            {
                MessageBox.Show("لطفاً یک تامین‌کننده معتبر را انتخاب کنید.");
                return;
            }

            try
            {
                // مرحله 1: بررسی وجود سفارشات مرتبط
                bool hasOrders = await _context.PurchaseOrder
                    .AnyAsync(po => po.SupplierID == selectedSupplier.SupplierID);

                if (hasOrders)
                {
                    var confirm = MessageBox.Show(
                        "این تامین‌کننده در سفارشات استفاده شده است. آیا می‌خواهید ارتباط را قطع کنید؟",
                        "تایید عملیات",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (confirm != MessageBoxResult.Yes) return;
                }

                // مرحله 2: عملیات حذف در تراکنش
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // بروزرسانی سفارشات مرتبط به صورت مستقیم در دیتابیس
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE PurchaseOrder SET SupplierID = NULL WHERE SupplierID = {selectedSupplier.SupplierID}");

                    // حذف تامین‌کننده
                    _context.Supplier.Remove(selectedSupplier);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // به روزرسانی لیست
                    LoadSuppliers();
                    MessageBox.Show("عملیات با موفقیت انجام شد.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    MessageBox.Show($"خطا در عملیات دیتابیس: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطای غیرمنتظره: {ex.Message}");
            }
        }

        private void AddNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            // انتقال به صفحه افزودن جدید
            var addPage = new AddSupplierPage();
            this.NavigationService.Navigate(addPage);
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "جستجو...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "جستجو...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}