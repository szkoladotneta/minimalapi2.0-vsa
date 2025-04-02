using System;
using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Common.Models;

namespace MinimalApi20_vsa.Api.Common.Helpers;

public static class LinkGenerator
{
    /// <summary>
    /// Creates a self link with the current request URL
    /// </summary>
    public static Link Self(HttpContext httpContext) =>
        new(httpContext.Request.Path.Value ?? "", "self", httpContext.Request.Method);

    /// <summary>
    /// Creates a link to a resource
    /// </summary>
    public static Link To(string href, string rel, string method) =>
        new(href, rel, method);

    /// <summary>
    /// Creates common resource links for a specific resource
    /// </summary>
    public static void AddStandardResourceLinks<T, TId>(
        this HateoasResponse<T> response, 
        string resourceName, 
        TId id,
        bool includeCollection = true)
    {
        var resourcePath = $"/api/{resourceName.ToLowerInvariant()}";
        var resourceIdPath = $"{resourcePath}/{id}";
        
        // Self link to this specific resource
        response.AddLink(resourceIdPath, "self", "GET");
        
        // Link to update this resource
        response.AddLink(resourceIdPath, "update", "PUT");
        
        // Link to delete this resource
        response.AddLink(resourceIdPath, "delete", "DELETE");
        
        // Link to the collection (optional)
        if (includeCollection)
        {
            response.AddLink(resourcePath, "collection", "GET");
        }
    }
}