using Microsoft.AspNetCore.Builder;

namespace MinimalApi20_vsa.Api.Endpoints;

public interface IEndpointGroup
{
    string GroupName { get; }
    void ConfigureGroup(RouteGroupBuilder group);
}