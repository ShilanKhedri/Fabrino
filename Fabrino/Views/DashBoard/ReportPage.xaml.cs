// ReportPage.xaml.cs
using Fabrino.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;


namespace Fabrino.Views.DashBoard
{
    public partial class ReportPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public SeriesCollection SalesChartSeries { get; set; }
        public SeriesCollection PurchaseChartSeries { get; set; }
        public SeriesCollection MonthlySalesChart { get; set; }
        public List<string> FabricLabels { get; set; }
        public List<string> MonthLabels { get; set; }

        public ReportPage()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            InitializeComponent();
            LoadSales();
            LoadPurchases();
            LoadFabrics();
            LoadUsers();
            LoadCharts();
            DataContext = this;
        }

        private string ToPersianDate(DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date):0000}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}";
        }

        private void LoadSales()
        {
            try
            {
                var orders = _context.Order
                    .Include(o => o.Customer)
                    .AsEnumerable()
                    .Select(o => new
                    {
                        PersianOrderDate = o.OrderDate.HasValue ?
                            ToPersianDate(o.OrderDate.Value) : "نامشخص",
                        CustomerName = o.Customer?.FullName ?? "بدون مشتری",
                        TotalAmount = o.TotalAmount ?? 0
                        // فقط فیلدهای موجود استفاده شده‌اند
                    })
                    .ToList();

                SalesGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری اطلاعات: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
 

        private void LoadPurchases()
        {
            var purchases = _context.PurchaseOrder.Include(p => p.Supplier).ToList()
                .Select(p => new
                {
                    PersianOrderDate = ToPersianDate(p.OrderDate),
                    p.Supplier,
                    p.TotalAmount,
                    p.Status
                }).ToList();

            PurchaseGrid.ItemsSource = purchases;
        }

        private void LoadFabrics()
        {
            FabricsGrid.ItemsSource = _context.Fabric.ToList();
        }

        private void LoadUsers()
        {
            UsersGrid.ItemsSource = _context.Users.ToList()
                .Select(u => new
                {
                    u.full_name,
                    u.username,
                    u.role,
                    PersianLoginDate = u.last_login.HasValue ? ToPersianDate(u.last_login.Value) : "-"
                }).ToList();
        }

        private void LoadCharts()
        {
            // فروش بر اساس نوع پارچه
            var salesData = _context.OrderItem.Include(o => o.Fabric).ToList()
                .GroupBy(o => o.Fabric.Name)
                .Select(g => new { g.Key, Total = g.Sum(x => x.Quantity) }).ToList();

            FabricLabels = salesData.Select(x => x.Key).ToList();
            SalesChartSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "فروش بر حسب متر",
                    Values = new ChartValues<decimal>(salesData.Select(x => x.Total))
                }
            };

            // نمودار دایره‌ای خرید
            var purchaseData = _context.PurchaseOrder.Include(p => p.Supplier).ToList()
                .GroupBy(p => p.Supplier.Name)
                .Select(g => new { g.Key, Total = g.Sum(p => p.TotalAmount) }).ToList();

            PurchaseChartSeries = new SeriesCollection();
            foreach (var item in purchaseData)
            {
                PurchaseChartSeries.Add(new PieSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<decimal> { item.Total },
                    DataLabels = true
                });
            }

            // نمودار فروش ماهانه
            var currentYear = DateTime.Now.Year;
            var monthlyData = _context.Order
              .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == currentYear)

                .ToList()
                .GroupBy(o => o.OrderDate.HasValue ? o.OrderDate.Value.Month : -1)
                .OrderBy(g => g.Key)
                .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToList();

            MonthLabels = monthlyData.Select(m => $"ماه {m.Month}").ToList();
            MonthlySalesChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "مجموع فروش ماهانه",
                    Values = new ChartValues<double>(monthlyData
    .Select(m => m.Total.HasValue ? (double)m.Total.Value : double.NaN))
                }
            };
        }

        private void FilterSales_Click(object sender, RoutedEventArgs e)
        {
            if (FromDatePicker.SelectedDate == null || ToDatePicker.SelectedDate == null)
            {
                MessageBox.Show("تاریخ شروع و پایان را انتخاب کنید.");
                return;
            }

            var from = FromDatePicker.SelectedDate.Value;
            var to = ToDatePicker.SelectedDate.Value;

            var filtered = _context.Order.Include(o => o.Customer)
                .Where(o => o.OrderDate >= from && o.OrderDate <= to).ToList()
                .Select(o => new
                {
                    PersianOrderDate = ToPersianDate(o.OrderDate ?? DateTime.MinValue),
                    o.Customer,
                    o.TotalAmount
                }).ToList();

            SalesGrid.ItemsSource = filtered;
        }

        private void ExportByFormat(DataGrid grid, string fileName, ComboBox formatCombo)
        {
            var format = ((ComboBoxItem)formatCombo.SelectedItem)?.Content.ToString();
            if (format == "PDF")
                ExportDataGridToPdf(grid, fileName);
            else
                ExportDataGridToExcel(grid, fileName);
        }

        private void ExportDataGridToPdf(DataGrid grid, string fileName)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = fileName + ".pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, new FileStream(dlg.FileName, FileMode.Create));
                doc.Open();

                PdfPTable table = new PdfPTable(grid.Columns.Count);
                foreach (var col in grid.Columns)
                    table.AddCell(new Phrase(col.Header.ToString()));

                foreach (var item in grid.ItemsSource)
                {
                    foreach (var col in grid.Columns)
                    {
                        var cellContent = col.GetCellContent(item) as TextBlock;
                        table.AddCell(new Phrase(cellContent?.Text ?? ""));
                    }
                }

                doc.Add(table);
                doc.Close();
                MessageBox.Show("فایل PDF ذخیره شد.");
            }
        }

        private void ExportDataGridToExcel(DataGrid grid, string fileName)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = fileName + ".xlsx"
            };

            if (dlg.ShowDialog() == true)
            {

                using (ExcelPackage pck = new ExcelPackage())
                {
                    var ws = pck.Workbook.Worksheets.Add("گزارش");
                    for (int i = 0; i < grid.Columns.Count; i++)
                        ws.Cells[1, i + 1].Value = grid.Columns[i].Header;

                    int row = 2;
                    foreach (var item in grid.ItemsSource)
                    {
                        for (int i = 0; i < grid.Columns.Count; i++)
                        {
                            var cellContent = grid.Columns[i].GetCellContent(item) as TextBlock;
                            ws.Cells[row, i + 1].Value = cellContent?.Text;
                        }
                        row++;
                    }

                    File.WriteAllBytes(dlg.FileName, pck.GetAsByteArray());
                    MessageBox.Show("فایل Excel ذخیره شد.");
                }
            }
        }

        private void ExportSales_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(SalesGrid, "SalesReport", SalesExportFormat);

        private void ExportPurchase_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(PurchaseGrid, "PurchaseReport", PurchaseExportFormat);

        private void ExportFabrics_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(FabricsGrid, "FabricReport", FabricExportFormat);

        private void ExportUsers_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(UsersGrid, "UserReport", UserExportFormat);
    }
}
