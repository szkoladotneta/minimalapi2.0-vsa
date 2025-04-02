using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MinimalApi20_vsa.Api.Endpoints;

namespace MinimalApi20_vsa.Api.Features.Categories;

public class CategoriesGroup : IEndpointGroup
{
    public string GroupName => "Categories";
    
    public void ConfigureGroup(RouteGroupBuilder group)
    {
        group.WithTags("Categories");
        // Add other common configurations:
        // group.RequireAuthorization();
        // group.AddEndpointFilter<YourCustomFilter>();
    }
}
