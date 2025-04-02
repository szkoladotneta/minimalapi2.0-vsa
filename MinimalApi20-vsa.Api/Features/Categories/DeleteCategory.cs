using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Categories;

public static class DeleteCategory
{
    [EndpointGroup("Categories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardDelete("/{id}", Handler)
                .WithName("DeleteCategory")
                .WithDescription("Deletes a category by ID");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(int id, AppDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        
        if (category is null)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
