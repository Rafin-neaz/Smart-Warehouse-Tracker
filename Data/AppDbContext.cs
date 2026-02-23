using Microsoft.EntityFrameworkCore;
using WarehouseTracker.Enums;
using WarehouseTracker.Models;

namespace WarehouseTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<SubCategory> SubCategories => Set<SubCategory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RowVersion).IsRowVersion();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SubCategory)
                      .WithMany(sc => sc.Products)
                      .HasForeignKey(e => e.SubCategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(p => p.Name)
              .HasMaxLength(125)
              .IsRequired();
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.HasOne(sc => sc.Category)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(sc => sc.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var categories = new[]
            {
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Clothing" },
            new Category { Id = 3, Name = "Food & Beverage" },
            new Category { Id = 4, Name = "Machinery" },
        };

            var subCategories = new[]
            {
            new SubCategory { Id = 1, Name = "Smartphones", CategoryId = 1 },
            new SubCategory { Id = 2, Name = "Laptops", CategoryId = 1 },
            new SubCategory { Id = 3, Name = "Accessories", CategoryId = 1 },
            new SubCategory { Id = 4, Name = "Men's Wear", CategoryId = 2 },
            new SubCategory { Id = 5, Name = "Women's Wear", CategoryId = 2 },
            new SubCategory { Id = 6, Name = "Footwear", CategoryId = 2 },
            new SubCategory { Id = 7, Name = "Perishable", CategoryId = 3 },
            new SubCategory { Id = 8, Name = "Packaged Goods", CategoryId = 3 },
            new SubCategory { Id = 9, Name = "Industrial", CategoryId = 4 },
            new SubCategory { Id = 10, Name = "Agricultural", CategoryId = 4 },
        };

            var random = new Random(42);
            var statuses = Enum.GetValues<ProductStatus>();
            var origins = new[] { "Dhaka", "Chittagong", "Shanghai", "Mumbai", "Dubai", "London" };
            var destinations = new[] { "New York", "Paris", "Tokyo", "Sydney", "Berlin", "Toronto" };

            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                var categoryId = (i % 4) + 1;
                var subCategoryMap = new Dictionary<int, int[]>
            {
                { 1, new[] { 1, 2, 3 } },
                { 2, new[] { 4, 5, 6 } },
                { 3, new[] { 7, 8 } },
                { 4, new[] { 9, 10 } },
            };
                var subIds = subCategoryMap[categoryId];
                var subCategoryId = subIds[random.Next(subIds.Length)];
                var stock = random.Next(0, 150);
                var status = stock == 0
                    ? ProductStatus.OutOfStock
                    : statuses[random.Next(statuses.Length)];

                products.Add(new Product
                {
                    Id = i,
                    Name = $"Product-{i:D3} {(categoryId == 1 ? "Electronic" : categoryId == 2 ? "Apparel" : categoryId == 3 ? "Food" : "Machine")} Item",
                    Price = Math.Round((decimal)(random.NextDouble() * 999 + 1), 2),
                    StockQuantity = stock,
                    Status = status,
                    CategoryId = categoryId,
                    SubCategoryId = subCategoryId,
                    CreatedAt = new DateTime(2024, 1, 1),
                    UpdatedAt = new DateTime(2024, 1, 1),
                });
            }

            modelBuilder.Entity<Category>().HasData(categories);
            modelBuilder.Entity<SubCategory>().HasData(subCategories);
            modelBuilder.Entity<Product>().HasData(products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.StockQuantity,
                p.Status,
                p.CategoryId,
                p.SubCategoryId,
                p.CreatedAt,
                p.UpdatedAt
            }));
        }
    }
}
