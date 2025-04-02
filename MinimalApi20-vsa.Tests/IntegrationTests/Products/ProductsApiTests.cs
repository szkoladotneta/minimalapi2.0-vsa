using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Features.Products;
using MinimalApi20_vsa.Tests.IntegrationTests.TestFixtures;
using Xunit;

namespace MinimalApi20_vsa.Tests.IntegrationTests.Products;

[Trait("Category", TestCategories.Integration)]
public class ProductsApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ProductsApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Act
        var response = await _client.GetAsync("/api/products/1");
        var content = await response.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<GetProduct.Response>>(content, _jsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(hateoasResponse);
        Assert.NotNull(hateoasResponse.Data);
        Assert.Equal(1, hateoasResponse.Data.Id);
        Assert.Equal("Product 1", hateoasResponse.Data.Name);
        Assert.Equal(10.0m, hateoasResponse.Data.Price);
        
        // Verify HATEOAS links
        Assert.Contains(hateoasResponse.Links, l => l.Rel == "self");
        Assert.Contains(hateoasResponse.Links, l => l.Rel == "update");
        Assert.Contains(hateoasResponse.Links, l => l.Rel == "delete");
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new CreateProduct.Request("New Test Product", 99.99m, 1);
        var content = new StringContent(
            JsonSerializer.Serialize(newProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<CreateProduct.Response>>(responseContent, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(hateoasResponse);
        Assert.NotNull(hateoasResponse.Data);
        Assert.True(hateoasResponse.Data.Id > 0);
        
        // Verify location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/products/{hateoasResponse.Data.Id}", response.Headers.Location!.ToString());
        
        // Verify the product was really created by making a GET request
        var getResponse = await _client.GetAsync(response.Headers.Location);
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidProduct = new CreateProduct.Request("", -10m, null);
        var content = new StringContent(
            JsonSerializer.Serialize(invalidProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var updatedProduct = new UpdateProduct.Request("Updated Product 1", 15.99m, 2);
        var content = new StringContent(
            JsonSerializer.Serialize(updatedProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync("/api/products/1", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the product was updated
        var getResponse = await _client.GetAsync("/api/products/1");
        getResponse.EnsureSuccessStatusCode();
        
        var responseContent = await getResponse.Content.ReadAsStringAsync();
        var hateoasResponse = JsonSerializer.Deserialize<HateoasResponse<GetProduct.Response>>(responseContent, _jsonOptions);
        
        Assert.NotNull(hateoasResponse);
        Assert.Equal("Updated Product 1", hateoasResponse.Data.Name);
        Assert.Equal(15.99m, hateoasResponse.Data.Price);
        Assert.Equal(2, hateoasResponse.Data.Category?.Id);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/api/products/3");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the product was deleted
        var getResponse = await _client.GetAsync("/api/products/3");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
