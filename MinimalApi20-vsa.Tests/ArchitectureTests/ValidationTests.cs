using System.Linq;
using System.Reflection;
using FluentValidation;
using NetArchTest.Rules;
using Xunit;

namespace MinimalApi20_vsa.Tests.ArchitectureTests;

[Trait("Category", TestCategories.Architecture)]
public class ValidationTests
{
    private readonly Assembly _assembly = typeof(MinimalApi20_vsa.Api.Endpoints.IEndpoint).Assembly;
    
    [Fact]
    public void AllRequestClasses_ShouldHaveValidator()
    {
        // Find all Request classes in Features
        var requestTypes = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("MinimalApi20_vsa.Api.Features")
            .And()
            .HaveNameEndingWith("Request")
            .GetTypes();
            
        foreach (var requestType in requestTypes)
        {
            // Find validator for this request type in the same declaring type
            var declaringType = requestType.DeclaringType;
            if (declaringType == null) continue;
            
            var validatorType = declaringType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(t => typeof(IValidator<>).MakeGenericType(requestType).IsAssignableFrom(t));
                
            Assert.NotNull(validatorType);
        }
    }
    
    [Fact]
    public void AllRequestValidators_ShouldInheritFromAbstractValidator()
    {
        // Find all validators
        var validatorInterfaceType = typeof(IValidator<>);
        
        var validatorTypes = Types.InAssembly(_assembly)
            .That()
            .ImplementInterface(validatorInterfaceType)
            .GetTypes();
            
        foreach (var validatorType in validatorTypes)
        {
            // Check if it inherits from AbstractValidator<>
            var baseType = validatorType.BaseType;
            Assert.NotNull(baseType);
            
            var isAbstractValidator = baseType.IsGenericType && 
                                     baseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>);
                                     
            Assert.True(isAbstractValidator, 
                $"Validator {validatorType.FullName} should inherit from AbstractValidator<>");
        }
    }
}
