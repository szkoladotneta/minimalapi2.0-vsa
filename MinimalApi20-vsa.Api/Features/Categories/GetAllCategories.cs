using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Categories;

public static class GetAllCategories
{
    public record CategoryDto(int Id, string Name, string Description);
    public record Response(List<CategoryDto> Categories, int Total);

    [EndpointGroup("Categories")]
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGetWithHateoas<Response>("", Handler)
                .WithName("GetAllCategories")
                .WithDescription("Gets all categories");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(AppDbContext context)
    {
        var categories = await context.Categories
            .ToListAsync();

        var categoryDtos = categories.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.Description
        )).ToList();

        var response = new Response(categoryDtos, categoryDtos.Count);
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        // Add links relevant to category collection
        hateoasResponse.Links.Add(new Link("self", "/api/categories", "GET"));
        hateoasResponse.Links.Add(new Link("create", "/api/categories", "POST"));
        
        // Add links for each category
        foreach (var category in categoryDtos)
        {
            hateoasResponse.Embedded.Add(
                $"categories/{category.Id}", 
                new Dictionary<string, object>
                {
                    ["id"] = category.Id,
                    ["name"] = category.Name,
                    ["description"] = category.Description,
                    ["_links"] = new List<Link>
                    {
                        new Link("self", $"/api/categories/{category.Id}", "GET"),
                        new Link("update", $"/api/categories/{category.Id}", "PUT"),
                        new Link("delete", $"/api/categories/{category.Id}", "DELETE"),
                        new Link("products", $"/api/categories/{category.Id}/products", "GET")
                    }
                }
            );
        }

        return TypedResults.Ok(hateoasResponse);
    }
}
