using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using Fabrino.Models; // مسیر کلاس‌های مدل

public class AppDbContext : DbContext
{
    public DbSet<Fabric> Fabrics { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<UserModel> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=.;Database=Fabrino;Trusted_Connection=True;");
    }
}
