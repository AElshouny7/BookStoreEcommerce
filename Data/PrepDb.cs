using BookStoreEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreEcommerce.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var ctx = serviceScope.ServiceProvider.GetRequiredService<StoreDbContext>();
            ctx.Database.Migrate();
            SeedData(ctx);
        }

        private static void SeedData(StoreDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Products.Any())
            {
                Console.WriteLine("Seeding Data...");

                context.Products.AddRange(
                    new Product() { Title = "Book A", Description = "Description for Book A", Price = 9.99M },
                    new Product() { Title = "Book B", Description = "Description for Book B", Price = 14.99M },
                    new Product() { Title = "Book C", Description = "Description for Book C", Price = 29.99M }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We already have data");
            }
        }
    }
}