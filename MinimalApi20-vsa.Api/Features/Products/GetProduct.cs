using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class GetProduct
{
    public record Response(int Id, string Name, decimal Price);

    [EndpointGroup("Products")]
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/{id}", Handler);
        }
    }

    public static async Task<Results<Ok<Response>, NotFound>> Handler(int id, AppDbContext context)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
        {
            return TypedResults.NotFound();
        }

        await Task.Delay(2000);
        return TypedResults.Ok(new Response(product.Id, product.Name, product.Price));
    }
}