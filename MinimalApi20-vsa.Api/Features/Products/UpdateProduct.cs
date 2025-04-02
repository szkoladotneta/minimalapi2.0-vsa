using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class UpdateProduct
{
    public record Request(string Name, decimal Price, int? CategoryId);
    public record Response(int Id, string Name, decimal Price, int? CategoryId);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Price).GreaterThan(0);
            When(r => r.CategoryId.HasValue, () => {
                RuleFor(r => r.CategoryId!.Value).GreaterThan(0);
            });
        }
    }
    
    [EndpointGroup("Products")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPutWithValidation<Request>("/{id}", Handler)
                .WithName("UpdateProduct")
                .WithDescription("Updates an existing product");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(int id, Request request, AppDbContext context)
    {
        var product = await context.Products.FindAsync(id);
        
        if (product is null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await context.Categories.AnyAsync(c => c.Id == request.CategoryId.Value);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with id {request.CategoryId.Value} not found");
            }
        }

        // Update product properties
        product.Name = request.Name;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
