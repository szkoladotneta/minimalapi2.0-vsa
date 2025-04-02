using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Categories;

public static class UpdateCategory
{
    public record Request(string Name, string Description);
    public record Response(int Id, string Name, string Description);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Description).NotEmpty();
        }
    }
    
    [EndpointGroup("Categories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPutWithValidation<Request>("/{id}", Handler)
                .WithName("UpdateCategory")
                .WithDescription("Updates an existing category");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(int id, Request request, AppDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        
        if (category is null)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        // Update category properties
        category.Name = request.Name;
        category.Description = request.Description;

        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
