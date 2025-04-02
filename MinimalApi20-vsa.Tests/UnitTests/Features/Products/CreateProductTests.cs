using FluentValidation.TestHelper;
using MinimalApi20_vsa.Api.Features.Products;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Features.Products;

[Trait("Category", TestCategories.Unit)]
public class CreateProductTests
{
    [Fact]
    public void Validator_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new CreateProduct.Validator();
        var request = new CreateProduct.Request("", 10.0m, null);
        
        // Act
        var result = validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }
    
    [Fact]
    public void Validator_ShouldHaveError_WhenPriceIsZeroOrNegative()
    {
        // Arrange
        var validator = new CreateProduct.Validator();
        
        // Act & Assert
        validator.TestValidate(new CreateProduct.Request("Test", 0m, null))
            .ShouldHaveValidationErrorFor(r => r.Price);
            
        validator.TestValidate(new CreateProduct.Request("Test", -1m, null))
            .ShouldHaveValidationErrorFor(r => r.Price);
    }
    
    [Fact]
    public void Validator_ShouldHaveError_WhenCategoryIdIsInvalid()
    {
        // Arrange
        var validator = new CreateProduct.Validator();
        var request = new CreateProduct.Request("Test", 10.0m, 0);
        
        // Act
        var result = validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(r => r.CategoryId.Value);
    }
    
    [Fact]
    public void Validator_ShouldNotHaveError_WhenRequestIsValid()
    {
        // Arrange
        var validator = new CreateProduct.Validator();
        var request = new CreateProduct.Request("Test", 10.0m, 1);
        
        // Act
        var result = validator.TestValidate(request);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
