// Services/OrderService.cs

using Fabrino.Models;

namespace Fabrino.Services
{
    public class OrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        public List<Supplier> GetSuppliers() => _db.Supplier.ToList();
        public List<Fabric> GetProducts() => _db.Fabric.ToList();

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