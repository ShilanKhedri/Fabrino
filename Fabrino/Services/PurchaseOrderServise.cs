using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
            try
            {
                return _context.Supplier
                    .Where(s => s.is_active)
                    .OrderBy(s => s.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در دریافت لیست تامین‌کنندگان: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Supplier>();
            }
        }

        public List<Fabric> GetFabrics()
        {
            try
            {
                return _context.Fabric
                    .OrderBy(f => f.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در دریافت لیست پارچه‌ها: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Fabric>();
            }
        }

        public void CreatePurchaseOrder(PurchaseOrder order, List<PurchaseOrderItem> items)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // تولید شماره سفارش
                order.OrderNumber = GenerateOrderNumber();
                order.OrderDate = DateTime.Now;
                order.TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice);

                _context.PurchaseOrder.Add(order);
                _context.SaveChanges();

                foreach (var item in items)
                {
                    item.PurchaseOrderID = order.PurchaseOrderID;
                    _context.PurchaseOrderItem.Add(item);

                    // 🟢 افزایش مقدار پارچه
                    var fabric = _context.Fabric.FirstOrDefault(f => f.FabricID == item.FabricID);
                    if (fabric != null)
                    {
                        fabric.Quantity += item.Quantity;
                        _context.Fabric.Update(fabric);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();

                MessageBox.Show("سفارش با موفقیت ثبت شد.", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"خطا در ثبت سفارش: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                if (ex.InnerException != null)
                    MessageBox.Show($"جزئیات خطا: {ex.InnerException.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public PurchaseOrder GetOrderByNumber(string orderNumber)
        {
            try
            {
                return _context.PurchaseOrder
                    .Include(po => po.Supplier)
                    .Include(po => po.PurchaseOrderItems)
                        .ThenInclude(poi => poi.Fabric)
                    .FirstOrDefault(po => po.OrderNumber == orderNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در دریافت اطلاعات سفارش: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
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