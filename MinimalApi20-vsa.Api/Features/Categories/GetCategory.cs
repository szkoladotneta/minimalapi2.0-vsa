using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Categories;

public static class GetCategory
{
    public record Response(int Id, string Name, string Description);

    [EndpointGroup("Categories")]
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGetWithHateoas<Response>("/{id}", Handler)
                .WithName("GetCategory")
                .WithDescription("Gets a category by ID");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(int id, AppDbContext context)
    {
        var category = await context.Categories.FindAsync(id);

        if (category is null)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        var response = new Response(category.Id, category.Name, category.Description);
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        // Add links relevant to this category
        hateoasResponse.Links.Add(new Link("self", $"/api/categories/{id}", "GET"));
        hateoasResponse.Links.Add(new Link("products", $"/api/categories/{id}/products", "GET"));

        return TypedResults.Ok(hateoasResponse);
    }
}
