using FluentValidation;
using MinimalApi20_vsa.Api.Common.Helpers;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;
using MinimalApi20_vsa.Api.Endpoints;
using MinimalApi20_vsa.Api.Endpoints.Filters;

namespace MinimalApi20_vsa.Api.Features.Categories;

public static class CreateCategory
{
    public record Request(string Name, string Description);
    public record Response(int Id);

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
            app.MapStandardPostWithHateoas<Request, HateoasResponse<Response>>("", Handler)
                .WithName("CreateCategory")
                .WithDescription("Creates a new category");
        }
    }
    
    public static async Task<IResult> Handler(Request request, AppDbContext context)
    {
        var category = new Category { Name = request.Name, Description = request.Description };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        // Create response with data
        var response = new Response(category.Id);
        
        // Wrap in HATEOAS container and add links
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        // Add standard resource links
        hateoasResponse.AddStandardResourceLinks("Categories", category.Id);
        
        // Add any additional specific links
        hateoasResponse.AddLink($"/api/categories/{category.Id}/products", "products", "GET");

        return Results.Created($"/api/categories/{category.Id}", hateoasResponse);
    }
}