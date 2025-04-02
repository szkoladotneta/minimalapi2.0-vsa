using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi20_vsa.Api.Common.Helpers;
using MinimalApi20_vsa.Api.Common.Models;
using System;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Common.Helpers;

[Trait("Category", TestCategories.Unit)]
public class HateoasResultsTests
{
    [Fact]
    public void CreatedWithLinks_ReturnsCorrectResponse()
    {
        // Arrange
        var testData = new { Id = 1, Name = "Test" };
        var link1 = new Link("/test/1", "self", "GET");
        var link2 = new Link("/test", "collection", "GET");
        
        // Act
        var result = HateoasResults.CreatedWithLinks("/test/1", testData, link1, link2);
        
        // Assert
        Assert.NotNull(result);
        
        // Check that the result is of type Created with HateoasResponse
        var resultType = result.GetType();
        Assert.True(resultType.Name.StartsWith("Created"), $"Expected Created type but got {resultType.Name}");
        
        // Verify the location
        var locationProperty = resultType.GetProperty("Location");
        Assert.NotNull(locationProperty);
        var location = locationProperty.GetValue(result) as string;
        Assert.Equal("/test/1", location);
        
        // Get the Value property
        var valueProperty = resultType.GetProperty("Value");
        Assert.NotNull(valueProperty);
        var hateoasResponse = valueProperty.GetValue(result);
        Assert.NotNull(hateoasResponse);
        
        // Get the HateoasResponse.Data
        var hateoasResponseType = hateoasResponse.GetType();
        var dataProperty = hateoasResponseType.GetProperty("Data");
        Assert.NotNull(dataProperty);
        var data = dataProperty.GetValue(hateoasResponse);
        Assert.NotNull(data);
        
        // Verify data properties
        var idProperty = data.GetType().GetProperty("Id");
        var nameProperty = data.GetType().GetProperty("Name");
        Assert.Equal(1, idProperty.GetValue(data));
        Assert.Equal("Test", nameProperty.GetValue(data));
        
        // Get the HateoasResponse.Links
        var linksProperty = hateoasResponseType.GetProperty("Links");
        Assert.NotNull(linksProperty);
        var links = linksProperty.GetValue(hateoasResponse) as System.Collections.Generic.ICollection<Link>;
        Assert.Equal(2, links.Count);
        Assert.Contains(links, l => l.Href == "/test/1" && l.Rel == "self" && l.Method == "GET");
        Assert.Contains(links, l => l.Href == "/test" && l.Rel == "collection" && l.Method == "GET");
    }
    
    [Fact]
    public void OkWithLinks_ReturnsCorrectResponse()
    {
        // Arrange
        var testData = new { Id = 1, Name = "Test" };
        var link1 = new Link("/test/1", "self", "GET");
        var link2 = new Link("/test", "collection", "GET");
        
        // Act
        var result = HateoasResults.OkWithLinks(testData, link1, link2);
        
        // Assert
        Assert.NotNull(result);
        
        // Check that the result is of type Ok with HateoasResponse
        var resultType = result.GetType();
        Assert.True(resultType.Name.StartsWith("Ok"), $"Expected Ok type but got {resultType.Name}");
        
        // Get the Value property
        var valueProperty = resultType.GetProperty("Value");
        Assert.NotNull(valueProperty);
        var hateoasResponse = valueProperty.GetValue(result);
        Assert.NotNull(hateoasResponse);
        
        // Get the HateoasResponse.Data
        var hateoasResponseType = hateoasResponse.GetType();
        var dataProperty = hateoasResponseType.GetProperty("Data");
        Assert.NotNull(dataProperty);
        var data = dataProperty.GetValue(hateoasResponse);
        Assert.NotNull(data);
        
        // Verify data properties
        var idProperty = data.GetType().GetProperty("Id");
        var nameProperty = data.GetType().GetProperty("Name");
        Assert.Equal(1, idProperty.GetValue(data));
        Assert.Equal("Test", nameProperty.GetValue(data));
        
        // Get the HateoasResponse.Links
        var linksProperty = hateoasResponseType.GetProperty("Links");
        Assert.NotNull(linksProperty);
        var links = linksProperty.GetValue(hateoasResponse) as System.Collections.Generic.ICollection<Link>;
        Assert.Equal(2, links.Count);
        Assert.Contains(links, l => l.Href == "/test/1" && l.Rel == "self" && l.Method == "GET");
        Assert.Contains(links, l => l.Href == "/test" && l.Rel == "collection" && l.Method == "GET");
    }
}
