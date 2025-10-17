using Microsoft.EntityFrameworkCore;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Data;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{



    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItems> OrderItems => Set<OrderItems>();

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     // Example fluent config (think Spring JPA annotations)
    //     modelBuilder.Entity<Category>()
    //         .HasIndex(c => c.Name).IsUnique();   // unique category name

    //     modelBuilder.Entity<Product>()
    //         .Property(p => p.Price).HasPrecision(18, 2); // decimal precision

    //     modelBuilder.Entity<Product>()
    //         .HasOne(p => p.Category)
    //         .WithMany(c => c.Products)
    //         .HasForeignKey(p => p.CategoryId)
    //         .OnDelete(DeleteBehavior.Restrict);
    // }
}