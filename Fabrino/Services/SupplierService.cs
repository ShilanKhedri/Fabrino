
using Fabrino.Models;
using System.Collections.Generic;
using System.Linq;

public class SupplierService
{
    private readonly AppDbContext _db;

    public SupplierService(AppDbContext db)
    {
        _db = db;
    }

    public List<Supplier> GetAllSuppliers()
    {
        return _db.Suppliers.ToList();
    }
}
