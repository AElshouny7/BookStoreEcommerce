using Microsoft.EntityFrameworkCore;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{



    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItems> OrderItems => Set<OrderItems>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            // Properties
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.Description).HasColumnType("text");

        });

        // Product (FK --> Category)
        modelBuilder.Entity<Product>(e =>
        {
            // Properties
            e.Property(x => x.Title).IsRequired().HasMaxLength(120);
            e.Property(x => x.ImageURL).HasMaxLength(1024);
            e.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(x => x.Description).HasColumnType("text");

            // Relations
            e.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            e.HasIndex(x => x.CategoryId);
        });

        // User
        modelBuilder.Entity<User>(e =>
        {
            // Properties
            e.Property(x => x.FullName).IsRequired().HasMaxLength(50);
            e.Property(x => x.Email).IsRequired().HasMaxLength(255);
            e.Property(x => x.PasswordHash).IsRequired();
        });

        // Order (FK --> User)
        modelBuilder.Entity<Order>(e =>
        {
            // Properties
            e.Property(x => x.OrderDate).IsRequired();
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();

            e.Property(x => x.Status)
            //  "Pending","Completed","Cancelled"
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired();

            // Relations
            e.HasOne<User>()
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.UserId);
        });

        // OrderItems (FK --> Order, Product)
        modelBuilder.Entity<OrderItems>(e =>
        {
            // Properties
            e.Property(x => x.Quantity).IsRequired();
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();


            // Relations
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.OrderId, x.ProductId }).IsUnique();

            // FK to Order
            e.HasOne<Order>()
             .WithMany()
             .HasForeignKey(x => x.OrderId)
             // delete items when order is deleted
             .OnDelete(DeleteBehavior.Cascade);

            // FK to Product
            e.HasOne<Product>()
             .WithMany()
             .HasForeignKey(x => x.ProductId)
             // prevent deleting a product if items reference it
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.ProductId);
        });

    }
}