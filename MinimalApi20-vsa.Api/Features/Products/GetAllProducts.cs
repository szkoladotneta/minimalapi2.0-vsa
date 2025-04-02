using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class GetAllProducts
{
    public record CategoryDto(int Id, string Name);
    public record ProductDto(int Id, string Name, decimal Price, CategoryDto? Category);
    public record Response(List<ProductDto> Products, int Total);

    [EndpointGroup("Products")]
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGetWithHateoas<Response>("", Handler)
                .WithName("GetAllProducts")
                .WithDescription("Gets all products");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(AppDbContext context)
    {
        var products = await context.Products
            .Include(p => p.Category)
            .ToListAsync();

        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Price,
            p.Category != null ? new CategoryDto(p.Category.Id, p.Category.Name) : null
        )).ToList();

        var response = new Response(productDtos, productDtos.Count);
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        // Add links relevant to product collection
        hateoasResponse.Links.Add(new Link("self", "/api/products", "GET"));
        hateoasResponse.Links.Add(new Link("create", "/api/products", "POST"));
        
        // Add links for each product
        foreach (var product in productDtos)
        {
            hateoasResponse.Embedded.Add(
                $"products/{product.Id}", 
                new Dictionary<string, object>
                {
                    ["id"] = product.Id,
                    ["name"] = product.Name,
                    ["price"] = product.Price,
                    ["_links"] = new List<Link>
                    {
                        new Link("self", $"/api/products/{product.Id}", "GET"),
                        new Link("update", $"/api/products/{product.Id}", "PUT"),
                        new Link("delete", $"/api/products/{product.Id}", "DELETE")
                    }
                }
            );
        }

        return TypedResults.Ok(hateoasResponse);
    }
}
