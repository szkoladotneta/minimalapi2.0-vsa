using System.Reflection;
using Microsoft.AspNetCore.Routing;
using MinimalApi20_vsa.Api.Endpoints;
using Xunit;
using NetArchTest.Rules;

namespace MinimalApi20_vsa.Tests.ArchitectureTests;

[Trait("Category", TestCategories.Architecture)]
public class EndpointRegistrationTests
{
    private readonly Assembly _assembly = typeof(MinimalApi20_vsa.Api.Endpoints.IEndpoint).Assembly;

    [Fact]
    public void AllEndpointGroupsImplementIEndpointGroup()
    {
        // Arrange
        var groupPattern = Types.InAssembly(_assembly)
            .That()
            .HaveNameEndingWith("Group")
            .And()
            .DoNotHaveNameEndingWith("GroupAttribute")
            .GetTypes();

        // Assert
        Assert.All(groupPattern, t =>
            Assert.True(typeof(IEndpointGroup).IsAssignableFrom(t),
                $"Type {t.FullName} matches endpoint group naming pattern but doesn't implement IEndpointGroup"));
    }

    [Fact]
    public void EndpointClasses_ShouldBeNestedInFeatureClasses()
    {
        // Arrange & Act
        var endpointTypes = Types.InAssembly(_assembly)
            .That()
            .ImplementInterface(typeof(IEndpoint))
            .GetTypes();

        // Assert
        // Fix assertion to use a proper form that accepts a message
        Assert.All(endpointTypes, t =>
            Assert.True(t.DeclaringType != null,
                $"Endpoint type {t.FullName} should be nested inside a feature class"));
    }

    [Fact]
    public void EndpointGroups_ShouldHaveGroupNameProperty()
    {
        // Arrange
        var groupTypes = Types.InAssembly(_assembly)
            .That()
            .ImplementInterface(typeof(IEndpointGroup))
            .GetTypes();

        // Assert
        foreach (var groupType in groupTypes)
        {
            var instance = Activator.CreateInstance(groupType) as IEndpointGroup;
            Assert.NotNull(instance);
            Assert.NotEmpty(instance.GroupName);
        }
    }

    [Fact]
    public void EndpointGroup_ShouldHaveConfigureGroupMethod()
    {
        // Arrange
        var groupTypes = Types.InAssembly(_assembly)
            .That()
            .ImplementInterface(typeof(IEndpointGroup))
            .GetTypes();

        // Assert
        foreach (var groupType in groupTypes)
        {
            var configureGroupMethod = groupType.GetMethod("ConfigureGroup",
                BindingFlags.Public | BindingFlags.Instance);

            Assert.NotNull(configureGroupMethod);

            // Should have RouteGroupBuilder parameter
            var parameters = configureGroupMethod.GetParameters();
            Assert.Single(parameters);
            Assert.Equal(typeof(RouteGroupBuilder), parameters[0].ParameterType);
        }
    }
}