using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Endpoints.Filters;
using Moq;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Endpoints.Filters;

[Trait("Category", TestCategories.Unit)]
public class EndpointFilterBaseTests
{
    public class TestFilter : EndpointFilterBase
    {
        public bool BeforeExecutionCalled { get; private set; }
        public bool AfterExecutionCalled { get; private set; }
        public object? PreResult { get; set; }
        public object? PostResult { get; set; }
        
        protected override ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
        {
            BeforeExecutionCalled = true;
            return ValueTask.FromResult(PreResult);
        }
        
        protected override ValueTask<object?> OnAfterExecutionAsync(EndpointFilterInvocationContext context, object? response)
        {
            AfterExecutionCalled = true;
            return ValueTask.FromResult(PostResult ?? response);
        }
    }
    
    [Fact]
    public async Task InvokeAsync_CallsOnBeforeAndOnAfterExecution()
    {
        // Arrange
        var filter = new TestFilter();
        var contextMock = new Mock<EndpointFilterInvocationContext>();
        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>("Result");
        
        // Act
        var result = await filter.InvokeAsync(contextMock.Object, next);
        
        // Assert
        Assert.True(filter.BeforeExecutionCalled);
        Assert.True(filter.AfterExecutionCalled);
        Assert.Equal("Result", result);
    }
    
    [Fact]
    public async Task InvokeAsync_ReturnsPreResultIfNotNull()
    {
        // Arrange
        var filter = new TestFilter { PreResult = "PreResult" };
        var contextMock = new Mock<EndpointFilterInvocationContext>();
        EndpointFilterDelegate next = _ => throw new InvalidOperationException("Should not be called");
        
        // Act
        var result = await filter.InvokeAsync(contextMock.Object, next);
        
        // Assert
        Assert.True(filter.BeforeExecutionCalled);
        Assert.False(filter.AfterExecutionCalled); // Next is not called, so AfterExecution is not called
        Assert.Equal("PreResult", result);
    }
    
    [Fact]
    public async Task InvokeAsync_ReturnsPostResultIfNotNull()
    {
        // Arrange
        var filter = new TestFilter { PostResult = "PostResult" };
        var contextMock = new Mock<EndpointFilterInvocationContext>();
        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>("Result");
        
        // Act
        var result = await filter.InvokeAsync(contextMock.Object, next);
        
        // Assert
        Assert.True(filter.BeforeExecutionCalled);
        Assert.True(filter.AfterExecutionCalled);
        Assert.Equal("PostResult", result);
    }
}
