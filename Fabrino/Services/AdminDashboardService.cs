using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Fabrino.Models;

namespace Fabrino.Services
{
    public class AdminDashboardService
    {
        private readonly AppDbContext _db;

        public AdminDashboardService(AppDbContext db)
        {
            _db = db;
        }

        public decimal GetTotalFabricStock()
        {
            return _db.Fabric.Sum(f => f.Quantity);
        }

        public (int todayOrders, int yesterdayOrders, decimal percentageChange) GetOrdersComparison()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            var todayOrders = _db.Order.Count(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == today);
            var yesterdayOrders = _db.Order.Count(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == yesterday);

            var percentageChange = yesterdayOrders == 0 ? 100 : 
                ((todayOrders - yesterdayOrders) / (decimal)yesterdayOrders) * 100;

            return (todayOrders, yesterdayOrders, percentageChange);
        }

        public List<(string Month, decimal Sales)> GetSalesChart()
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            
            var orders = _db.Order
                .Where(o => o.OrderDate >= sixMonthsAgo)
                .Select(o => new 
                {
                    o.OrderDate,
                    o.TotalAmount
                })
                .ToList();

            var monthlySales = orders
                .GroupBy(o => new { Year = o.OrderDate.Value.Year, Month = o.OrderDate.Value.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Sales = g.Sum(o => o.TotalAmount ?? 0)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return monthlySales.Select(x => (
                Month: x.Month.ToString("MMMM"),
                Sales: x.Sales
            )).ToList();
        }

        public class PopularFabricData
        {
            public string FabricName { get; set; }
            public int SalesCount { get; set; }
        }

        public List<PopularFabricData> GetPopularFabrics()
        {
            var orderItems = _db.OrderItem
                .Include(oi => oi.Fabric)
                .Select(oi => new
                {
                    FabricName = oi.Fabric.Name
                })
                .ToList();

            return orderItems
                .GroupBy(oi => oi.FabricName)
                .Select(g => new PopularFabricData
                {
                    FabricName = g.Key,
                    SalesCount = g.Count()
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(5)
                .ToList();
        }
    }
} 