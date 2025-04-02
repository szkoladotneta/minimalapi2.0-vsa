using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Common.Models;

namespace MinimalApi20_vsa.Api.Common.Helpers;

public static class HateoasResults
{
    /// <summary>
    /// Creates a 201 Created result with a HATEOAS-wrapped response
    /// </summary>
    public static IResult CreatedWithLinks<T>(string uri, T data, params Link[] links)
    {
        var response = new HateoasResponse<T>(data);
        
        foreach (var link in links)
        {
            response.Links.Add(link);
        }
        
        return Results.Created(uri, response);
    }
    
    /// <summary>
    /// Creates a 200 OK result with a HATEOAS-wrapped response
    /// </summary>
    public static IResult OkWithLinks<T>(T data, params Link[] links)
    {
        var response = new HateoasResponse<T>(data);
        
        foreach (var link in links)
        {
            response.Links.Add(link);
        }
        
        return Results.Ok(response);
    }
}