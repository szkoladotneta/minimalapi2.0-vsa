using MinimalApi20_vsa.Api.Common.Models;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Common.Models;

[Trait("Category", TestCategories.Unit)]
public class HateoasResponseTests
{
    [Fact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        // Arrange
        var testData = new { Id = 1, Name = "Test" };
        
        // Act
        var response = new HateoasResponse<object>(testData);
        
        // Assert
        Assert.Equal(testData, response.Data);
        Assert.NotNull(response.Links);
        Assert.Empty(response.Links);
        Assert.NotNull(response.Embedded);
        Assert.Empty(response.Embedded);
    }
    
    [Fact]
    public void AddLink_AddsLinkCorrectly()
    {
        // Arrange
        var response = new HateoasResponse<object>(new { });
        
        // Act
        var result = response.AddLink("/test", "test", "GET");
        
        // Assert
        Assert.Same(response, result); // Returns the same instance for chaining
        Assert.Single(response.Links);
        
        var link = Assert.Single(response.Links);
        Assert.Equal("/test", link.Href);
        Assert.Equal("test", link.Rel);
        Assert.Equal("GET", link.Method);
    }
    
    [Fact]
    public void Link_Constructor_InitializesPropertiesCorrectly()
    {
        // Act
        var link = new Link("/test", "test", "GET");
        
        // Assert
        Assert.Equal("/test", link.Href);
        Assert.Equal("test", link.Rel);
        Assert.Equal("GET", link.Method);
    }
}
