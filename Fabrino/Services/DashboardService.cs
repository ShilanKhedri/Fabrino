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
            ProductCount = _db.Fabric.Count(),
            TotalOrders = _db.Order.Count(),
            OutOfStockItems = _db.Fabric.Count(p => p.Quantity <= 0),
            CustomerCount = _db.Customer.Count()
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