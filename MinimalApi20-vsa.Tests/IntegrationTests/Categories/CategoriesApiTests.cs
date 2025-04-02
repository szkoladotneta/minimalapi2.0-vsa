using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Features.Categories;
using MinimalApi20_vsa.Tests.IntegrationTests.TestFixtures;
using Xunit;

namespace MinimalApi20_vsa.Tests.IntegrationTests.Categories;

[Trait("Category", TestCategories.Integration)]
public class CategoriesApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CategoriesApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCategories_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/categories");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetCategory_WithValidId_ReturnsCategory()
    {
        // Act
        var response = await _client.GetAsync("/api/categories/1");
        var content = await response.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<GetCategory.Response>>(content, _jsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(hateoasResponse);
        Assert.NotNull(hateoasResponse.Data);
        Assert.Equal(1, hateoasResponse.Data.Id);
        Assert.Equal("Category 1", hateoasResponse.Data.Name);
        Assert.Equal("Test Category 1", hateoasResponse.Data.Description);
        
        // Verify HATEOAS links
        Assert.Contains(hateoasResponse.Links, l => l.Rel == "self");
        Assert.Contains(hateoasResponse.Links, l => l.Rel == "products");
    }

    [Fact]
    public async Task GetCategoryProducts_ReturnsProductsInCategory()
    {
        // Act
        var response = await _client.GetAsync("/api/categories/1/products");
        var content = await response.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<GetCategoryProducts.Response>>(content, _jsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(hateoasResponse);
        Assert.NotNull(hateoasResponse.Data);
        Assert.Equal(2, hateoasResponse.Data.Products.Count());
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newCategory = new CreateCategory.Request("New Test Category", "New Test Description");
        var content = new StringContent(
            JsonSerializer.Serialize(newCategory),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/categories", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<CreateCategory.Response>>(responseContent, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(hateoasResponse);
        Assert.NotNull(hateoasResponse.Data);
        Assert.True(hateoasResponse.Data.Id > 0);
        
        // Verify location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/categories/{hateoasResponse.Data.Id}", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task UpdateCategory_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updatedCategory = new UpdateCategory.Request("Updated Category 2", "Updated Description 2");
        var content = new StringContent(
            JsonSerializer.Serialize(updatedCategory),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync("/api/categories/2", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the category was updated
        var getResponse = await _client.GetAsync("/api/categories/2");
        getResponse.EnsureSuccessStatusCode();
        
        var responseContent = await getResponse.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<GetCategory.Response>>(responseContent, _jsonOptions);
        
        Assert.NotNull(hateoasResponse);
        Assert.Equal("Updated Category 2", hateoasResponse.Data.Name);
        Assert.Equal("Updated Description 2", hateoasResponse.Data.Description);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsNoContent()
    {
        // Create a category to delete (to avoid affecting other tests)
        var newCategory = new CreateCategory.Request("Category to Delete", "This will be deleted");
        var createContent = new StringContent(
            JsonSerializer.Serialize(newCategory),
            Encoding.UTF8,
            "application/json");
        var createResponse = await _client.PostAsync("/api/categories", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<CreateCategory.Response>>(createResponseContent, _jsonOptions);
        
        // Act
        var response = await _client.DeleteAsync($"/api/categories/{hateoasResponse.Data.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the category was deleted
        var getResponse = await _client.GetAsync($"/api/categories/{hateoasResponse.Data.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
