using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MinimalApi20_vsa.Api.Common.Models;

/// <summary>
/// Represents a link in a HATEOAS response
/// </summary>
public class Link
{
    public string Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }

    public Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}

/// <summary>
/// Wrapper class for resources that includes HATEOAS links
/// </summary>
/// <typeparam name="T">The type of resource being wrapped</typeparam>
public class HateoasResponse<T>
{
    public T Data { get; set; }
    public List<Link> Links { get; set; } = new List<Link>();
    public Dictionary<string, object> Embedded { get; set; } = new Dictionary<string, object>();

    public HateoasResponse(T data)
    {
        Data = data;
    }

    public HateoasResponse<T> AddLink(string href, string rel, string method)
    {
        Links.Add(new Link(href, rel, method));
        return this;
    }
}