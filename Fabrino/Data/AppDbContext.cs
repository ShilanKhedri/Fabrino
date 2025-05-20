using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using Fabrino.Models; // مسیر کلاس‌های مدل
using System.Diagnostics;

public class AppDbContext : DbContext
{
    public DbSet<Fabric> Fabric { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<UserModel> Users { get; set; }

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
    }
}
