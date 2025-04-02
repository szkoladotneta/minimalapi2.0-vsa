using System.Linq;
using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace MinimalApi20_vsa.Tests.ArchitectureTests;

[Trait("Category", TestCategories.Architecture)]
public class LayeringTests
{
    private readonly Assembly _assembly = typeof(MinimalApi20_vsa.Api.Endpoints.IEndpoint).Assembly;    
    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        // Arrange
        var domainNamespace = "MinimalApi20_vsa.Api.Domain";
        
        // Act
        var result = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace(domainNamespace)
            .Should()
            .NotHaveDependencyOn("MinimalApi20_vsa.Api.Features")
            .And()
            .NotHaveDependencyOn("MinimalApi20_vsa.Api.Endpoints")
            .GetResult();
        
        // Assert
Assert.True(result.IsSuccessful, 
    $"Domain layer should not depend on other layers. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
    
    [Fact]
    public void Features_ShouldNotDependOnEndpoints()
    {
        // Arrange
        var featuresNamespace = "MinimalApi20_vsa.Api.Features";
        
        // Act
        var result = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace(featuresNamespace)
            .Should()
            .NotHaveDependencyOn("MinimalApi20_vsa.Api.Endpoints.EndpointRegistrationExtensions")
            .GetResult();
        
        // Assert
        Assert.True(result.IsSuccessful, 
            $"Features should not depend on endpoint registration. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
    
    [Fact]
    public void Endpoints_ShouldFollowNamingConvention()
    {
        // Arrange & Act
        var result = Types.InAssembly(_assembly)
            .That()
            .ImplementInterface(typeof(MinimalApi20_vsa.Api.Endpoints.IEndpoint))
            .Should()
            .HaveNameEndingWith("Endpoint")
            .GetResult();
        
        // Assert
        Assert.True(result.IsSuccessful, 
            $"All IEndpoint implementations should end with 'Endpoint'. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
    
    [Fact]
    public void Handlers_ShouldBeStaticMethods()
    {
        // Get all types in the Features namespace
        var featureTypes = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("MinimalApi20_vsa.Api.Features")
            .GetTypes();
            
        // Find all Handler methods
        var handlerMethods = featureTypes
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name == "Handler")
            .ToList();
            
        // Assert
        Assert.NotEmpty(handlerMethods);
        Assert.All(handlerMethods, m => Assert.True(m.IsStatic, $"Handler method in {m.DeclaringType?.Name} should be static"));
    }
}
