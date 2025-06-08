// Services/OrderService.cs

using Fabrino.Models;

namespace Fabrino.Services
{
    /// <summary>
    /// Handles order processing and management operations
    /// </summary>
    public class OrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        // Retrieves reference data for order creation
        public List<Supplier> GetSuppliers() => _db.Supplier.ToList();
        public List<Fabric> GetProducts() => _db.Fabric.ToList();

        // Processes new order creation
        public void CreateOrder(Order order)
        {
            _db.Order.Add(order);
            _db.SaveChanges();
        }

        /*public Order GetOrderByNumber(string orderNumber)
        {
            return _db.Order
                .//Include(o => o.Product)
                .Include(o => o.Supplier)
                .FirstOrDefault(o => o.OrderNumber == orderNumber);
        }*/
    }
}