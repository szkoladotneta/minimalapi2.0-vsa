using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Common.Helpers;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;
using MinimalApi20_vsa.Api.Endpoints;
using MinimalApi20_vsa.Api.Endpoints.Filters;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class CreateProduct
{
    public record Request(string Name, decimal Price, int? CategoryId);
    public record Response(int Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Price).GreaterThan(0);
            When(r => r.CategoryId.HasValue, () => {
                RuleFor(r => r.CategoryId!.Value).GreaterThan(0);
            });
        }
    }
    
    [EndpointGroup("Products")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPostWithHateoas<Request, HateoasResponse<Response>>("", Handler)
                .WithName("CreateProduct")
                .WithDescription("Creates a new product");
        }
    }
    
    public static async Task<IResult> Handler(Request request, AppDbContext context, HttpContext httpContext)
    {
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await context.Categories.AnyAsync(c => c.Id == request.CategoryId.Value);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with id {request.CategoryId.Value} not found");
            }
        }

        var product = new Product 
        { 
            Name = request.Name, 
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var response = new Response(product.Id);
        
        var hateoasResponse = new HateoasResponse<Response>(response);
        
        hateoasResponse.AddStandardResourceLinks("Products", product.Id);
        
        return Results.Created($"/api/products/{product.Id}", hateoasResponse);
    }
}