using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class DeleteProduct
{
    [EndpointGroup("Products")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardDelete("/{id}", Handler)
                .WithName("DeleteProduct")
                .WithDescription("Deletes a product by ID");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(int id, AppDbContext context)
    {
        var product = await context.Products.FindAsync(id);
        
        if (product is null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
