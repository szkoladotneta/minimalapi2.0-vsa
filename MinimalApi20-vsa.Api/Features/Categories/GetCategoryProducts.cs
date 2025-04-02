using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Categories;

public static class GetCategoryProducts
{
    public record ProductResponse(int Id, string Name, decimal Price);
    public record Response(IEnumerable<ProductResponse> Products);

    [EndpointGroup("Categories")]
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGetWithHateoas<Response>("/{id}/products", Handler)
                .WithName("GetCategoryProducts")
                .WithDescription("Gets all products in a specific category");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(int id, AppDbContext context)
    {
        // First check if the category exists
        var categoryExists = await context.Categories.AnyAsync(c => c.Id == id);
        
        if (!categoryExists)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        // Get all products for the category
        var products = await context.Products
            .Where(p => p.CategoryId == id)
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Price))
            .ToListAsync();

        var response = new Response(products);
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        // Add links
        hateoasResponse.Links.Add(new Link("self", $"/api/categories/{id}/products", "GET"));
        hateoasResponse.Links.Add(new Link("category", $"/api/categories/{id}", "GET"));
        
        // Add links to each product
        foreach (var product in products)
        {
            hateoasResponse.Links.Add(new Link($"product-{product.Id}", $"/api/products/{product.Id}", "GET"));
        }

        return TypedResults.Ok(hateoasResponse);
    }
}
