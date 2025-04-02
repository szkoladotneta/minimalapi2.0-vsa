using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Common.Exceptions;
using MinimalApi20_vsa.Api.Endpoints.Filters;
using Moq;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Endpoints.Filters;

[Trait("Category", TestCategories.Unit)]
public class ValidationFilterTests
{
    public class TestRequest
    {
        public string Name { get; set; } = string.Empty;
    }
    
    [Fact]
    public async Task InvokeAsync_ThrowsValidationException_WhenRequestIsNull()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        var filter = new ValidationFilter<TestRequest>(validatorMock.Object);
        
        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(c => c.Arguments).Returns(Array.Empty<object>());
        
        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>("Result");
        
        // Act & Assert
var exception = await Assert.ThrowsAsync<MinimalApi20_vsa.Api.Common.Exceptions.ValidationException>(
    async () => await filter.InvokeAsync(contextMock.Object, next));
        
        Assert.Contains("Invalid request format", exception.Errors);
    }
    
    [Fact]
    public async Task InvokeAsync_ThrowsValidationException_WhenValidationFails()
    {
        // Arrange
        var request = new TestRequest();
        var validationResult = new ValidationResult(
            new[] { 
                new ValidationFailure("Name", "Name is required")
            });
        
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        var filter = new ValidationFilter<TestRequest>(validatorMock.Object);
        
        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(c => c.Arguments).Returns(new object[] { request });
        
        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>("Result");
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<MinimalApi20_vsa.Api.Common.Exceptions.ValidationException>(
            async () => await filter.InvokeAsync(contextMock.Object, next));
        
        Assert.Contains("Name: Name is required", exception.Errors);
    }
    
    [Fact]
    public async Task InvokeAsync_CallsNext_WhenValidationPasses()
    {
        // Arrange
        var request = new TestRequest { Name = "Valid Name" };
        var validationResult = new ValidationResult();
        
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        var filter = new ValidationFilter<TestRequest>(validatorMock.Object);
        
        var contextMock = new Mock<EndpointFilterInvocationContext>();
        contextMock.Setup(c => c.Arguments).Returns(new object[] { request });
        
        var nextCalled = false;
        EndpointFilterDelegate next = _ => 
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("Result");
        };
        
        // Act
        var result = await filter.InvokeAsync(contextMock.Object, next);
        
        // Assert
        Assert.True(nextCalled);
        Assert.Equal("Result", result);
    }
}
