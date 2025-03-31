using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Products;

public class ProductsGroup : IEndpointGroup
{
    public string GroupName => "Products";
    
    public void ConfigureGroup(RouteGroupBuilder group)
    {
        group.WithTags("Products");
        // Add other common configurations:
        // group.RequireAuthorization();
        // group.AddEndpointFilter<YourCustomFilter>();
    }
}