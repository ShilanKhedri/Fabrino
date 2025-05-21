using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabrino.Services
{
    public class PurchaseOrderService
    {
        private readonly AppDbContext _context;

        public PurchaseOrderService(AppDbContext context)
        {
            _context = context;
        }

        public List<Supplier> GetSuppliers()
        {
            return _context.Supplier.ToList();
        }

        public List<Fabric> GetFabrics()
        {
            return _context.Fabric.ToList();
        }

        public void CreatePurchaseOrder(PurchaseOrder order, List<PurchaseOrderItem> items)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // تولید شماره سفارش
                order.OrderNumber = GenerateOrderNumber();
                order.OrderDate = DateTime.Now;
                order.TotalAmount = items.Sum(i => i.TotalPrice);

                _context.PurchaseOrders.Add(order);
                _context.SaveChanges();

                foreach (var item in items)
                {
                    item.PurchaseOrderID = order.PurchaseOrderID;
                    _context.PurchaseOrderItems.Add(item);
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public PurchaseOrder GetOrderByNumber(string orderNumber)
        {
            return _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Fabric)
                .FirstOrDefault(po => po.OrderNumber == orderNumber);
        }

        private string GenerateOrderNumber()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");
            var day = DateTime.Now.Day.ToString("D2");
            var random = new Random().Next(1000, 9999);

            return $"PO-{year}{month}{day}-{random}";
        }
    }
}