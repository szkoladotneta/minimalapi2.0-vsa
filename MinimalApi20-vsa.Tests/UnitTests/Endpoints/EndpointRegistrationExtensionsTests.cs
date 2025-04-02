using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi20_vsa.Api.Endpoints;
using Moq;
using Xunit;

namespace MinimalApi20_vsa.Tests.UnitTests.Endpoints;

[Trait("Category", TestCategories.Unit)]
public class EndpointRegistrationExtensionsTests
{
    public class TestEndpoint : IEndpoint
    {
        public bool MapEndpointCalled { get; private set; }
        
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            MapEndpointCalled = true;
        }
    }
    
    [EndpointGroup("Test")]
    public class TestGroupedEndpoint : IEndpoint
    {
        public bool MapEndpointCalled { get; private set; }
        
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            MapEndpointCalled = true;
        }
    }
    
    public class TestEndpointGroup : IEndpointGroup
    {
        public string GroupName => "Test";
        public bool ConfigureGroupCalled { get; private set; }
        
        public void ConfigureGroup(RouteGroupBuilder group)
        {
            ConfigureGroupCalled = true;
        }
    }
    
    [Fact]
    public void AddEndpoints_RegistersEndpointsFromAssembly()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();
        
        // Act
        services.AddEndpoints(assembly);
        
        // Assert
        var provider = services.BuildServiceProvider();
        var endpoints = provider.GetServices<IEndpoint>();
        var groups = provider.GetServices<IEndpointGroup>();
        
        Assert.Contains(endpoints, e => e.GetType() == typeof(TestEndpoint));
        Assert.Contains(endpoints, e => e.GetType() == typeof(TestGroupedEndpoint));
        Assert.Contains(groups, g => g.GetType() == typeof(TestEndpointGroup));
    }
}
