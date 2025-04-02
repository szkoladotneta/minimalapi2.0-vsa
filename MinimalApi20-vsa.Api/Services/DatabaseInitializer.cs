using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;

namespace MinimalApi20_vsa.Api.Services;

public class DatabaseInitializer
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        AppDbContext dbContext,
        ILogger<DatabaseInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {

            // Check if we need to seed data
            if (!await _dbContext.Categories.AnyAsync() && !await _dbContext.Products.AnyAsync())
            {
                _logger.LogInformation("Seeding database...");
                await SeedDataAsync();
                _logger.LogInformation("Database seeded successfully");
            }
            else
            {
                _logger.LogInformation("Database already contains data - skipping seed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private async Task SeedDataAsync()
    {
        // Seed Categories
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
            new Category { Id = 2, Name = "Books", Description = "Physical and digital books" },
            new Category { Id = 3, Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { Id = 4, Name = "Home & Kitchen", Description = "Household and kitchen items" }
        };
        
        await _dbContext.Categories.AddRangeAsync(categories);
        await _dbContext.SaveChangesAsync();
        
        // Seed Products
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Smartphone", Price = 799.99m, CategoryId = 1 },
            new Product { Id = 2, Name = "Laptop", Price = 1299.99m, CategoryId = 1 },
            new Product { Id = 3, Name = "Headphones", Price = 199.99m, CategoryId = 1 },
            new Product { Id = 4, Name = "Novel", Price = 14.99m, CategoryId = 2 },
            new Product { Id = 5, Name = "Cookbook", Price = 24.99m, CategoryId = 2 },
            new Product { Id = 6, Name = "T-Shirt", Price = 19.99m, CategoryId = 3 },
            new Product { Id = 7, Name = "Jeans", Price = 49.99m, CategoryId = 3 },
            new Product { Id = 8, Name = "Blender", Price = 79.99m, CategoryId = 4 },
            new Product { Id = 9, Name = "Coffee Maker", Price = 129.99m, CategoryId = 4 }
        };
        
        await _dbContext.Products.AddRangeAsync(products);
        await _dbContext.SaveChangesAsync();
    }
}

// Extension method to add the database initializer to services
public static class DatabaseInitializerExtension
{
    public static IServiceCollection AddDatabaseInitializer(this IServiceCollection services)
    {
        return services.AddScoped<DatabaseInitializer>();
    }
    
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }
}
