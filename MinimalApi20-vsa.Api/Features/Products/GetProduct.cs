using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class GetProduct
{
    public record CategoryDto(int Id, string Name);
    public record Response(int Id, string Name, decimal Price, CategoryDto? Category);

    [EndpointGroup("Products")]
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGetWithHateoas<Response>("/{id}", Handler)
                .WithName("GetProduct")
                .WithDescription("Gets a product by ID");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(int id, AppDbContext context)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }
        
        var response = MapToResponse(product);
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        // Add links relevant to this product
        hateoasResponse.Links.Add(new Link("self", $"/api/products/{id}", "GET"));
        hateoasResponse.Links.Add(new Link("update", $"/api/products/{id}", "PUT"));
        hateoasResponse.Links.Add(new Link("delete", $"/api/products/{id}", "DELETE"));
        hateoasResponse.Links.Add(new Link("all", "/api/products", "GET"));
        
        // Add link to category if present
        if (product.Category != null)
        {
            hateoasResponse.Links.Add(new Link("category", $"/api/categories/{product.Category.Id}", "GET"));
        }

        return TypedResults.Ok(hateoasResponse);
    }

    private static Response MapToResponse(Product product)
    {
        CategoryDto? categoryDto = null;
        
        if (product.Category != null)
        {
            categoryDto = new CategoryDto(product.Category.Id, product.Category.Name);
        }

        return new Response(
            product.Id,
            product.Name,
            product.Price,
            categoryDto
        );
    }
}