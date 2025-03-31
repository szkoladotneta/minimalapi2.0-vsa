using FluentValidation;
using MinimalApi20_vsa.Api.Domain;
using MinimalApi20_vsa.Api.Domain.Entities;
using MinimalApi20_vsa.Api.Endpoints;
using MinimalApi20_vsa.Api.Endpoints.Filters;
using ValidationException = MinimalApi20_vsa.Api.Common.Exceptions.ValidationException;

namespace MinimalApi20_vsa.Api.Features.Products;

public static class CreateProduct
{
    public record Request(string Name, decimal Price);
    public record Response(int Id, string Name, decimal Price);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Price).GreaterThan(0);
        }
    }
    [EndpointGroup("Products")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("", Handler)
                           .AddEndpointFilter<ValidationFilter<Request>>();

        }
    }
    
    public static async Task<IResult> Handler(Request request, AppDbContext context)
    {
        // No need to validate here as it's handled by the filter
        var product = new Product { Name = request.Name, Price = request.Price };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return Results.Ok(new Response(product.Id, product.Name, product.Price));
    }
}