using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Domain.Entities;

namespace MinimalApi20_vsa.Api.Domain;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; init; }
    public DbSet<Category> Categories { get; init; }
}