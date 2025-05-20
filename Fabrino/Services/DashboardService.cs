// Services/DashboardService.cs
using Fabrino.Models;
using System.Linq;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public DashboardData GetDashboardData()
    {
        return new DashboardData
        {
            ProductCount = _db.Fabrics.Count(),
            TotalOrders = _db.Orders.Count(),
            OutOfStockItems = _db.Fabrics.Count(p => p.Quantity <= 0),
            CustomerCount = _db.Customers.Count()
        };
    }
}

public class DashboardData
{
    public int ProductCount { get; set; }
    public int TotalOrders { get; set; }
    public int OutOfStockItems { get; set; }
    public int CustomerCount { get; set; }
}