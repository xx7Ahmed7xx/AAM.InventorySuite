using AAM.Inventory.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AAM.Inventory.Core.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for Inventory Suite
/// </summary>
public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.HasIndex(e => e.Barcode).IsUnique().HasFilter("[Barcode] IS NOT NULL");
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.MinimumStockLevel).HasDefaultValue(0);

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // StockMovement configuration
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.StockMovements)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Role).HasConversion<int>();
        });
    }
}

