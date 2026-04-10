using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace InfnetCrud.Tests;

public class ProductEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly string _jsonFilePath;

    public ProductEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var relativePath = "C:\\PessoalDev\\InfnetCrud\\InfenetCrud\\Data\\products.json";
        _jsonFilePath = Path.GetFullPath(relativePath);
    }

    private void ResetJsonFile()
    {
        File.WriteAllText(_jsonFilePath, "[]");
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        // Arrange
        ResetJsonFile();

        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostProduct_ShouldCreateProduct()
    {
        // Arrange
        ResetJsonFile();

        var newProduct = new
        {
            name = "Teclado",
            price = 120.50m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/products", newProduct);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdProduct.Should().NotBeNull();
        createdProduct!.Id.Should().NotBe(Guid.Empty);
        createdProduct.Name.Should().Be("Teclado");
        createdProduct.Price.Should().Be(120.50m);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnCreatedProduct()
    {
        // Arrange
        ResetJsonFile();

        var newProduct = new
        {
            name = "Mouse",
            price = 80.00m
        };

        var postResponse = await _client.PostAsJsonAsync("/products", newProduct);
        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Act
        var getResponse = await _client.GetAsync($"/products/{createdProduct!.Id}");
        var product = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be("Mouse");
        product.Price.Should().Be(80.00m);
    }

    [Fact]
    public async Task PutProduct_ShouldUpdateProduct()
    {
        // Arrange
        ResetJsonFile();

        var newProduct = new
        {
            name = "Monitor",
            price = 900.00m
        };

        var postResponse = await _client.PostAsJsonAsync("/products", newProduct);
        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var updatedProduct = new
        {
            name = "Monitor Gamer",
            price = 1500.00m
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"/products/{createdProduct!.Id}", updatedProduct);
        var product = await putResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be("Monitor Gamer");
        product.Price.Should().Be(1500.00m);
    }

    [Fact]
    public async Task DeleteProduct_ShouldRemoveProduct()
    {
        // Arrange
        ResetJsonFile();

        var newProduct = new
        {
            name = "Cadeira",
            price = 500.00m
        };

        var postResponse = await _client.PostAsJsonAsync("/products", newProduct);
        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ProductResponse>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/products/{createdProduct!.Id}");
        var getResponse = await _client.GetAsync($"/products/{createdProduct.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}