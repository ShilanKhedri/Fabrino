using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using Fabrino.Models; 
using System.Diagnostics;

public class AppDbContext : DbContext
{
    public DbSet<Fabric> Fabric { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItem { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SecurityQuestion> SecurityQuestion { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=.;Database=Fabrino;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // تنظیمات اضافی برای مدل
        modelBuilder.Entity<UserModel>()
            .Property(u => u.last_login)
            .HasColumnType("datetime2");
        
        modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(po => po.OrderNumber)
                .IsUnique();

        modelBuilder.Entity<Inventory>()
            .HasIndex(i => i.FabricID)
            .IsUnique();

        // محاسبه خودکار TotalPrice
        modelBuilder.Entity<PurchaseOrderItem>()
            .Property(poi => poi.TotalPrice)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]");

        modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.PurchaseOrderItems)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderID);

        modelBuilder.Entity<PurchaseOrder>()
        .Property(p => p.SupplierID)
        .IsRequired(false);

        modelBuilder.Entity<PurchaseOrderItem>()
            .HasOne(poi => poi.Fabric)
            .WithMany(f => f.PurchaseOrderItems)
            .HasForeignKey(poi => poi.FabricID);

        modelBuilder.Entity<Supplier>()
            .HasMany(s => s.PurchaseOrders)
            .WithOne(po => po.Supplier)
            .HasForeignKey(po => po.SupplierID);
    }
}
