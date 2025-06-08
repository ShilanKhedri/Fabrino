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
using Fabrino.Services;
using System.Text;

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
        private FinanceReportService _financeService = new FinanceReportService(new AppDbContext());


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
            try
            {
                var pc = new PersianCalendar();
                return $"{pc.GetYear(date)}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}";
            }
            catch
            {
                return "نامشخص";
            }
        }

        private DateTime? FromPersianDate(string persianDate)
        {
            try
            {
                var parts = persianDate.Split('/');
                if (parts.Length != 3) return null;

                var pc = new PersianCalendar();
                return pc.ToDateTime(
                    int.Parse(parts[0]),
                    int.Parse(parts[1]),
                    int.Parse(parts[2]),
                    0, 0, 0, 0);
            }
            catch
            {
                return null;
            }
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
                        TotalAmount = string.Format("{0:N0}", o.TotalAmount ?? 0) + " تومان"
                    })
                    .ToList();

                SalesGrid.ItemsSource = orders;

                // تنظیم مقادیر اولیه برای تاریخ‌ها
                FromDatePicker.SelectedDate = DateTime.Now.AddMonths(-1).Date;
                ToDatePicker.SelectedDate = DateTime.Now.Date;
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
            try
            {
                if (FromDatePicker.SelectedDate == null || ToDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("لطفاً تاریخ شروع و پایان را انتخاب کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var from = FromDatePicker.SelectedDate.Value.Date;
                var to = ToDatePicker.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);

                var filtered = _context.Order
                    .Include(o => o.Customer)
                    .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                    .AsEnumerable()
                    .Select(o => new
                    {
                        PersianOrderDate = o.OrderDate.HasValue ?
                            ToPersianDate(o.OrderDate.Value) : "نامشخص",
                        CustomerName = o.Customer?.FullName ?? "بدون مشتری",
                        TotalAmount = string.Format("{0:N0}", o.TotalAmount ?? 0) + " تومان"
                    })
                    .ToList();

                SalesGrid.ItemsSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در فیلتر کردن اطلاعات: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                try
                {
                    Document doc = new Document(PageSize.A4.Rotate());
                    PdfWriter.GetInstance(doc, new FileStream(dlg.FileName, FileMode.Create));
                    doc.Open();

                    PdfPTable table = new PdfPTable(grid.Columns.Count);
                    table.WidthPercentage = 100;

                    // Add headers
                    foreach (var col in grid.Columns)
                    {
                        table.AddCell(new PdfPCell(new Phrase(col.Header.ToString()))
                        {
                            BackgroundColor = new BaseColor(240, 240, 240),
                            Padding = 5
                        });
                    }

                    // Add data
                    foreach (var item in grid.ItemsSource)
                    {
                        foreach (var col in grid.Columns)
                        {
                            var cellContent = col.GetCellContent(item) as TextBlock;
                            table.AddCell(new PdfPCell(new Phrase(cellContent?.Text ?? ""))
                            {
                                Padding = 5
                            });
                        }
                    }

                    doc.Add(table);
                    doc.Close();
                    MessageBox.Show("فایل PDF با موفقیت ذخیره شد.", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ایجاد فایل PDF: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
        private void ExportTextContent(string content, string fileName, ComboBox formatCombo)
        {
            var format = ((ComboBoxItem)formatCombo.SelectedItem)?.Content.ToString();

            if (format == "PDF")
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        var doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, new FileStream(dlg.FileName, FileMode.Create));
                        doc.Open();

                        foreach (var line in content.Split('\n'))
                        {
                            doc.Add(new Paragraph(line));
                        }

                        doc.Close();
                        MessageBox.Show("فایل PDF با موفقیت ذخیره شد.", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطا در ایجاد فایل PDF: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (format == "Excel")
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (dlg.ShowDialog() == true)
                {
                    using var package = new OfficeOpenXml.ExcelPackage();
                    var sheet = package.Workbook.Worksheets.Add("گزارش مالی");

                    // هر خط به یه سلول بریز
                    var lines = content.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                        sheet.Cells[i + 1, 1].Value = lines[i];

                    File.WriteAllBytes(dlg.FileName, package.GetAsByteArray());
                    MessageBox.Show("فایل Excel ذخیره شد.");
                }
            }
        }

        private void ExportFinance_Click(object sender, RoutedEventArgs e)
        {
            string content = $"درآمد: {TotalSalesText.Text}\n" +
                             $"هزینه: {TotalPurchasesText.Text}\n" +
                             $"سود خالص: {ProfitText.Text}\n" +
                             $"بازه: {FinanceFromDate.SelectedDate:yyyy/MM/dd} تا {FinanceToDate.SelectedDate:yyyy/MM/dd}";

            ExportTextContent(content, "FinanceReport", FinanceExportFormat);
        }


        // اینا متدهای ذخیره خروجی که قبلاً هم استفاده کردی (EPPlus / iTextSharp)


        private void ExportSales_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(SalesGrid, "SalesReport", SalesExportFormat);

        private void ExportPurchase_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(PurchaseGrid, "PurchaseReport", PurchaseExportFormat);

        private void ExportFabrics_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(FabricsGrid, "FabricReport", FabricExportFormat);

        private void ExportUsers_Click(object sender, RoutedEventArgs e) =>
            ExportByFormat(UsersGrid, "UserReport", UserExportFormat);

        private void CalculateFinance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FinanceFromDate.SelectedDate == null || FinanceToDate.SelectedDate == null)
                {
                    MessageBox.Show("لطفاً بازه تاریخ را کامل وارد کنید.", "خطا",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var from = FinanceFromDate.SelectedDate.Value.Date;
                var to = FinanceToDate.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);

                var summary = _financeService.GetSummary(from, to);

                // نمایش مقادیر با فرمت مناسب
                TotalSalesText.Text = string.Format("{0:N0}", summary.TotalSales) + " تومان";
                TotalPurchasesText.Text = string.Format("{0:N0}", summary.TotalPurchases) + " تومان";
                ProfitText.Text = string.Format("{0:N0}", summary.Profit) + " تومان";

                // تنظیم رنگ سود/زیان
                ProfitText.Foreground = summary.Profit >= 0 ? 
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 125, 50)) :  // سبز
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 40, 40));   // قرمز

                // به‌روزرسانی نمودار
                FinanceChart.Series = new LiveCharts.SeriesCollection
                {
                    new LiveCharts.Wpf.ColumnSeries
                    {
                        Title = "درآمد",
                        Values = new LiveCharts.ChartValues<decimal> { summary.TotalSales },
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 125, 50))
                    },
                    new LiveCharts.Wpf.ColumnSeries
                    {
                        Title = "هزینه",
                        Values = new LiveCharts.ChartValues<decimal> { summary.TotalPurchases },
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 40, 40))
                    },
                    new LiveCharts.Wpf.ColumnSeries
                    {
                        Title = "سود خالص",
                        Values = new LiveCharts.ChartValues<decimal> { summary.Profit },
                        Fill = new System.Windows.Media.SolidColorBrush(
                            summary.Profit >= 0 ? 
                                System.Windows.Media.Color.FromRgb(46, 125, 50) : 
                                System.Windows.Media.Color.FromRgb(198, 40, 40))
                    }
                };

                FinanceChart.AxisX.Clear();
                FinanceChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Title = "گزارش مالی",
                    Labels = new[] { $"از {ToPersianDate(from)} تا {ToPersianDate(to)}" }
                });

                FinanceChart.AxisY.Clear();
                FinanceChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    Title = "مبلغ (تومان)",
                    LabelFormatter = value => string.Format("{0:N0}", value)
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در محاسبات مالی: {ex.Message}", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
