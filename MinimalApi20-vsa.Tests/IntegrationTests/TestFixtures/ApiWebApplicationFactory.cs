using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApi20_vsa.Api;
using MinimalApi20_vsa.Api.Domain;

namespace MinimalApi20_vsa.Tests.IntegrationTests.TestFixtures;

/// <summary>
/// Web application factory for integration tests that uses an in-memory database
/// </summary>
public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database context
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database
            services.AddDbContext<AppDbContext>(options =>
            {
                // Use a unique name for each test to avoid conflicts
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Initialize the database with test data
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<ApiWebApplicationFactory>>();

            try
            {
                // Ensure the database is created
                db.Database.EnsureCreated();

                // Initialize with test data
                SeedTestData(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
            }
        });
    }

    private void SeedTestData(AppDbContext dbContext)
    {
        // Add test categories
        dbContext.Categories.AddRange(
            new Api.Domain.Entities.Category { Id = 1, Name = "Category 1", Description = "Test Category 1" },
            new Api.Domain.Entities.Category { Id = 2, Name = "Category 2", Description = "Test Category 2" }
        );

        // Add test products
        dbContext.Products.AddRange(
            new Api.Domain.Entities.Product { Id = 1, Name = "Product 1", Price = 10.0m, CategoryId = 1 },
            new Api.Domain.Entities.Product { Id = 2, Name = "Product 2", Price = 20.0m, CategoryId = 1 },
            new Api.Domain.Entities.Product { Id = 3, Name = "Product 3", Price = 30.0m, CategoryId = 2 }
        );

        dbContext.SaveChanges();
    }
}