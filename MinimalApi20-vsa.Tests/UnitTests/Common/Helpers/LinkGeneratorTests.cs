using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Common.Helpers;
using MinimalApi20_vsa.Api.Common.Models;
using Moq;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Common.Helpers;

[Trait("Category", TestCategories.Unit)]
public class LinkGeneratorTests
{
    [Fact]
    public void Self_ReturnsCorrectLink()
    {
        // Arrange
        var httpContextMock = new Mock<HttpContext>();
        var requestMock = new Mock<HttpRequest>();
        
        requestMock.Setup(r => r.Path).Returns(new PathString("/api/test"));
        requestMock.Setup(r => r.Method).Returns("GET");
        httpContextMock.Setup(h => h.Request).Returns(requestMock.Object);
        
        // Act
        var result = LinkGenerator.Self(httpContextMock.Object);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("/api/test", result.Href);
        Assert.Equal("self", result.Rel);
        Assert.Equal("GET", result.Method);
    }
    
    [Fact]
    public void To_ReturnsCorrectLink()
    {
        // Act
        var result = LinkGenerator.To("/api/test", "test", "GET");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("/api/test", result.Href);
        Assert.Equal("test", result.Rel);
        Assert.Equal("GET", result.Method);
    }
    
    [Fact]
    public void AddStandardResourceLinks_AddsCorrectLinks()
    {
        // Arrange
        var response = new HateoasResponse<object>(new { Id = 1 });
        
        // Act
        response.AddStandardResourceLinks("products", 1);
        
        // Assert
        Assert.Equal(4, response.Links.Count);
        Assert.Contains(response.Links, l => l.Href == "/api/products/1" && l.Rel == "self" && l.Method == "GET");
        Assert.Contains(response.Links, l => l.Href == "/api/products/1" && l.Rel == "update" && l.Method == "PUT");
        Assert.Contains(response.Links, l => l.Href == "/api/products/1" && l.Rel == "delete" && l.Method == "DELETE");
        Assert.Contains(response.Links, l => l.Href == "/api/products" && l.Rel == "collection" && l.Method == "GET");
    }
    
    [Fact]
    public void AddStandardResourceLinks_WithoutCollection_OmitsCollectionLink()
    {
        // Arrange
        var response = new HateoasResponse<object>(new { Id = 1 });
        
        // Act
        response.AddStandardResourceLinks("products", 1, includeCollection: false);
        
        // Assert
        Assert.Equal(3, response.Links.Count);
        Assert.Contains(response.Links, l => l.Href == "/api/products/1" && l.Rel == "self" && l.Method == "GET");
        Assert.Contains(response.Links, l => l.Href == "/api/products/1" && l.Rel == "update" && l.Method == "PUT");
        Assert.Contains(response.Links, l => l.Href == "/api/products/1" && l.Rel == "delete" && l.Method == "DELETE");
        Assert.DoesNotContain(response.Links, l => l.Rel == "collection");
    }
}
