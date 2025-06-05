using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fabrino.Models;

namespace Fabrino.Services
{
    public class SellerOrderService
    {
        private readonly AppDbContext _db;

        public SellerOrderService(AppDbContext db)
        {
            _db = db;
        }

        public List<Order> GetAllOrders()
        {
            return _db.Order
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _db.Order
                .Where(o => o.OrderID == orderId)
                .FirstOrDefault();
        }

        public List<Fabric> GetAllFabrics()
        {
            return _db.Fabric.ToList();
        }

        public List<Customer> GetAllCustomers()
        {
            return _db.Customer.ToList();
        }

        public void CreateOrder(string customerName, string phone, string address, List<(int fabricId, decimal quantity)> items)
        {
            var customer = _db.Customer.FirstOrDefault(c => c.FullName == customerName && c.Phone == phone);
            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = customerName,
                    Phone = phone,
                    Address = address
                };
                _db.Customer.Add(customer);
                _db.SaveChanges();
            }

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in items)
            {
                var fabric = _db.Fabric.FirstOrDefault(f => f.FabricID == item.fabricId);
                if (fabric == null) continue;

                var orderItem = new OrderItem
                {
                    FabricID = item.fabricId,
                    Quantity = item.quantity,
                    PricePerMeter = fabric.PricePerMeter
                };
                order.TotalAmount += orderItem.TotalPrice;
                order.OrderItems.Add(orderItem);
            }

            _db.Order.Add(order);
            _db.SaveChanges();
        }
    }
}

