using MinimalApi20_vsa.Api.Endpoints;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Endpoints;

[Trait("Category", TestCategories.Unit)]
public class EndpointGroupAttributeTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange
        const string groupName = "TestGroup";
        
        // Act
        var attribute = new EndpointGroupAttribute(groupName);
        
        // Assert
        Assert.Equal(groupName, attribute.GroupName);
    }
}
