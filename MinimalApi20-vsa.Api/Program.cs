using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Middleware;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;
using MinimalApi20_vsa.Api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("shop"));
builder.Services.AddDatabaseInitializer();

var app = builder.Build();

app.UseErrorHandling();
app.UsePerformanceMonitoring();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Apply migrations and seed data at startup
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Ensure the database is created with all seed data
    if (!dbContext.Products.Any())
    {
        dbContext.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

app.MapEndpoints();
await app.InitializeDatabaseAsync();
app.Run();

namespace MinimalApi20_vsa.Api{
    public abstract partial class Program
    {
    }
}