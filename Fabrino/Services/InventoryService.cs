using Fabrino.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fabrino.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db)
        {
            _db = db;
        }

        public List<Fabric> GetAll()
        {
            return _db.Fabric.ToList();
        }

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
    }
}
