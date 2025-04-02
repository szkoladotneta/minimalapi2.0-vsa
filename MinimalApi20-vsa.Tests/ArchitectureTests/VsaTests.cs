using System.Linq;
using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace MinimalApi20_vsa.Tests.ArchitectureTests;

[Trait("Category", TestCategories.Architecture)]
public class VsaTests
{
    private readonly Assembly _assembly = typeof(MinimalApi20_vsa.Api.Endpoints.IEndpoint).Assembly;

    [Fact]
    public void Feature_ClassesShouldContainRequestResponseAndHandler()
    {
        // Find feature types in the Features namespace that don't have "Group" in their name
        var featureTypes = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("MinimalApi20_vsa.Api.Features")
            .And()
            .DoNotHaveNameEndingWith("Group")
            .And()
            .ArePublic()
            .And()
            .AreNotNested()
            .GetTypes();

        foreach (var featureType in featureTypes)
        {
            // Check if type has a handler method
            var hasHandler = featureType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(m => m.Name == "Handler");

            Assert.True(hasHandler, $"{featureType.Name} should have a Handler method");

            // Check if the feature has request/response classes if it's a POST/PUT feature
            // For simplicity, we're just checking by name pattern here
            if (featureType.Name.StartsWith("Create") || featureType.Name.StartsWith("Update"))
            {
                var hasRequestType = featureType.GetNestedTypes()
                    .Any(t => t.Name == "Request");

                Assert.True(hasRequestType, $"{featureType.Name} should have a Request class");
            }

            // All features except Delete should have a Response class
            if (!featureType.Name.StartsWith("Delete"))
            {
                var hasResponseType = featureType.GetNestedTypes()
                    .Any(t => t.Name == "Response");

                Assert.True(hasResponseType, $"{featureType.Name} should have a Response class");
            }
        }
    }

    [Fact]
    public void Features_ShouldBeIndependent()
    {
        // Get all feature types
        var featureTypes = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("MinimalApi20_vsa.Api.Features")
            .GetTypes();

        // Group by main feature area (e.g., Products, Categories)
        var featureGroups = featureTypes
            .Where(t => t.Namespace != null)
            .GroupBy(t => t.Namespace);

        foreach (var group in featureGroups)
        {
            var groupNamespace = group.Key;

            // Skip endpoint group classes
            var nonGroupTypes = group.Where(t => !t.Name.EndsWith("Group")).ToList();

            foreach (var featureType in nonGroupTypes)
            {
                // The feature should only depend on its own group, common utilities, domain, and system libs
                // It should not depend on other feature groups
                foreach (var otherGroup in featureGroups)
                {
                    if (otherGroup.Key == groupNamespace) continue;

                    // We need to check dependency on the namespace, not just the key string
                    var otherNamespace = otherGroup.Key!;
                    var result = Types.InAssembly(_assembly)
                        .That()
                        .HaveNameStartingWith(featureType.Name)
                        .And()
                        .ResideInNamespace(featureType.Namespace ?? string.Empty)
                        .Should()
                        .NotHaveDependencyOn(otherNamespace)
                        .GetResult();

                    // Use explicit bool comparison to resolve ambiguity
                    Assert.True(result.IsSuccessful, 
                        $"{featureType.Name} should not depend on {otherNamespace}. Features should be isolated from each other.");
                }
            }
        }
    }

    [Fact]
    public void Handlers_ShouldFollowStandardNaming()
    {
        // Get all public static Handler methods from feature classes
        var handlerMethods = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace("MinimalApi20_vsa.Api.Features")
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name == "Handler")
            .ToList();

        // Handler methods should be present and named consistently
        Assert.NotEmpty(handlerMethods);
    }
}