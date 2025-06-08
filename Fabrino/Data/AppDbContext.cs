using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

/// <summary>
/// Main database context for the Fabrino application
/// Handles all database operations and entity relationships
/// </summary>
public class AppDbContext : DbContext
{
    // Entity Sets
    /// <summary>Database set for fabric products</summary>
    public DbSet<Fabric> Fabric { get; set; }
    /// <summary>Database set for customer information</summary>
    public DbSet<Customer> Customer { get; set; }
    /// <summary>Database set for customer orders</summary>
    public DbSet<Order> Order { get; set; }
    /// <summary>Database set for order line items</summary>
    public DbSet<OrderItem> OrderItem { get; set; }
    /// <summary>Database set for supplier information</summary>
    public DbSet<Supplier> Supplier { get; set; }
    /// <summary>Database set for user accounts</summary>
    public DbSet<UserModel> Users { get; set; }
    /// <summary>Database set for purchase orders</summary>
    public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
    /// <summary>Database set for purchase order items</summary>
    public DbSet<PurchaseOrderItem> PurchaseOrderItem { get; set; }
    /// <summary>Database set for inventory tracking</summary>
    public DbSet<Inventory> Inventories { get; set; }
    /// <summary>Database set for inventory transactions</summary>
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    /// <summary>Database set for payment records</summary>
    public DbSet<Payment> Payments { get; set; }
    /// <summary>Database set for suppliers (duplicate - consider removing)</summary>
    public DbSet<Supplier> Suppliers { get; set; }
    /// <summary>Database set for system logging</summary>
    public DbSet<SystemLog> SystemLogs { get; set; }
    /// <summary>Database set for support tickets</summary>
    public DbSet<SupportTicket> SupportTickets { get; set; }

    /// <summary>
    /// Configures the database connection
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=.;Database=Fabrino;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    /// <summary>
    /// Configures entity relationships and constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User Configuration
        modelBuilder.Entity<UserModel>()
            .Property(u => u.last_login)
            .HasColumnType("datetime2");
        
        // Purchase Order Configuration
        modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(po => po.OrderNumber)
                .IsUnique();

        // Inventory Configuration
        modelBuilder.Entity<Inventory>()
            .HasIndex(i => i.FabricID)
            .IsUnique();

        // Purchase Order Item Configuration
        modelBuilder.Entity<PurchaseOrderItem>()
            .Property(poi => poi.TotalPrice)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]");

        // Relationships Configuration
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
