using Fabrino.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fabrino.Services
{
    /// <summary>
    /// Manages fabric inventory operations including stock tracking and updates
    /// </summary>
    public class InventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db)
        {
            _db = db;
        }

        // Retrieves complete fabric inventory list
        public List<Fabric> GetAll()
        {
            return _db.Fabric.ToList();
        }

        // Handles new fabric item creation with supplier assignment
        public void AddItem(string name, string color, string material, decimal width, decimal quantity, decimal pricePerMeter, int supplierId)
        {
            var item = new Fabric
            {
                Name = name,
                Color = color,
                Material = material,
                Width = width,
                Quantity = quantity,
                PricePerMeter = pricePerMeter,
                SupplierID = supplierId
            };

            _db.Fabric.Add(item);
            _db.SaveChanges();
        }

        public void UpdateItem(Fabric fabric)
        {
            var existingFabric = _db.Fabric.Find(fabric.FabricID);
            if (existingFabric != null)
            {
                existingFabric.Name = fabric.Name;
                existingFabric.Color = fabric.Color;
                existingFabric.Material = fabric.Material;
                existingFabric.Width = fabric.Width;
                existingFabric.Quantity = fabric.Quantity;
                existingFabric.PricePerMeter = fabric.PricePerMeter;
                existingFabric.SupplierID = fabric.SupplierID;

                _db.SaveChanges();
            }
        }

        public void DeleteItem(Fabric fabric)
        {
            var existingFabric = _db.Fabric.Find(fabric.FabricID);
            if (existingFabric != null)
            {
                _db.Fabric.Remove(existingFabric);
                _db.SaveChanges();
            }
        }
    }
}
