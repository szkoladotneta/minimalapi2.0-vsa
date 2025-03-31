using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Middleware;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("shop"));


var app = builder.Build();

app.UseErrorHandling();
app.UsePerformanceMonitoring();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapEndpoints();
app.Run();