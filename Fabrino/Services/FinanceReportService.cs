using Fabrino.Models;
using System;
using System.Linq;

namespace Fabrino.Services
{
    public class FinanceReportService
    {
        private readonly AppDbContext _context;

        public FinanceReportService(AppDbContext context)
        {
            _context = context;
        }

        public FinanceSummary GetSummary(DateTime from, DateTime to)
        {
            var sales = _context.Order
                .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            var purchases = _context.PurchaseOrder
                .Where(p => p.OrderDate >= from && p.OrderDate <= to)
                .Sum(p => (decimal?)p.TotalAmount) ?? 0;

            return new FinanceSummary
            {
                TotalSales = sales,
                TotalPurchases = purchases,
                Profit = sales - purchases
            };
        }
    }

    public class FinanceSummary
    {
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal Profit { get; set; }
    }
}
