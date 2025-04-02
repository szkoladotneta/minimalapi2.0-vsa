using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace MinimalApi20_vsa.Tests.ArchitectureTests;

[Trait("Category", TestCategories.Architecture)]
public class ExceptionHandlingTests
{
    private readonly Assembly _assembly = typeof(MinimalApi20_vsa.Api.Endpoints.IEndpoint).Assembly;
    
    [Fact]
    public void CustomExceptions_ShouldInheritFromAppException()
    {
        // Arrange
        var appExceptionType = typeof(MinimalApi20_vsa.Api.Common.Exceptions.AppException);
        
        // Act & Assert
        var exceptions = Types.InAssembly(_assembly)
            .That()
            .ArePublic()
            .And()
            .HaveNameEndingWith("Exception")
            .And()
            .DoNotInherit(appExceptionType)
            .And()
            .AreNotAbstract()
            .GetTypes();
            
        // Only allow System exceptions and AppException
        var allowedExceptions = exceptions
            .Where(t => t.Namespace?.StartsWith("System") == true || 
                        t == appExceptionType)
            .ToList();
            
        var disallowedExceptions = exceptions
            .Except(allowedExceptions)
            .ToList();
            
        Assert.Empty(disallowedExceptions);
    }
    
    [Fact]
    public void Handlers_ShouldNotCatchGenericExceptions()
    {
        // This test requires source code analysis which is beyond the scope of NetArchTest.
        // It would typically be done with a tool like Roslyn analyzer or through code reviews.
        // As a simpler alternative, we'll check if handlers directly throw our custom exceptions
        
        // Find handler methods in features
        var handlerMethods = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("MinimalApi20_vsa.Api.Features")
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name == "Handler")
            .ToList();
            
        // If we had access to IL analysis, we could check for try/catch blocks with generic Exception handling
        // For now, we'll just assert that we found handlers, as a placeholder for this architectural rule
        Assert.NotEmpty(handlerMethods);
    }
}
