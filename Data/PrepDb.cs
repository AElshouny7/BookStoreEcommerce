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
            // Migrate database
            ctx.Database.Migrate();
            SeedData(ctx);
        }

        private static void SeedData(StoreDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Categories.Any())
            {
                context.Categories.Add(new Category
                {
                    Name = "General",
                    Description = "Default category for seeded books"
                });
                context.SaveChanges();
            }

            var defaultCategoryId = context.Categories
                                           .Select(c => c.Id)
                                           .First();
            Console.WriteLine($"Default Category ID: {defaultCategoryId}");

            if (!context.Products.Any())
            {
                Console.WriteLine("Seeding Data...");

                context.Products.AddRange(
                    new Product
                    {
                        Title = "Book A",
                        Description = "Description for Book A",
                        Price = 9.99M,
                        ImageURL = "",
                        StockQuantity = 10,
                        CategoryId = defaultCategoryId
                    },
                    new Product
                    {
                        Title = "Book B",
                        Description = "Description for Book B",
                        Price = 14.99M,
                        ImageURL = "",
                        StockQuantity = 15,
                        CategoryId = defaultCategoryId
                    },
                    new Product
                    {
                        Title = "Book C",
                        Description = "Description for Book C",
                        Price = 29.99M,
                        ImageURL = "",
                        StockQuantity = 8,
                        CategoryId = defaultCategoryId
                    }
                );

                context.SaveChanges();
            }
            else
            {
                var defaultCategoryIdd = context.Categories
                                           .Select(c => c.Id)
                                           .First();
                Console.WriteLine($"Default Category ID: {defaultCategoryIdd}");
                Console.WriteLine("We already have data");
            }
        }

    }
}