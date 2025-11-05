using Microsoft.EntityFrameworkCore;

namespace MyMicroservicesSaga.InventoryService.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options) { }

        public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InventoryItem>().HasData(
                new InventoryItem
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ProductName = "Laptop",
                    Stock = 20,
                    CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UpdatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new InventoryItem
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    ProductName = "Mobile",
                    Stock = 50,
                    CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UpdatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new InventoryItem
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    ProductName = "Headphones",
                    Stock = 15,
                    CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UpdatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new InventoryItem
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    ProductName = "Tablet",
                    Stock = 30,
                    CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UpdatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                },
                new InventoryItem
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    ProductName = "Smartwatch",
                    Stock = 10,
                    CreatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedBy = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UpdatedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}