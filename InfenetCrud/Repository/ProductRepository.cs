
using System.Text.Json;
using InfenetCrud.Models;

namespace InfenetCrud.Repository;

public class ProductRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public ProductRepository(IWebHostEnvironment env)
    {
        var dataFolder = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataFolder);

        _filePath = Path.Combine(dataFolder, "products.json");

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var products = await GetAllAsync();
        return products.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        var products = await GetAllAsync();

        product.Id = Guid.NewGuid();
        products.Add(product);

        await SaveAllAsync(products);
        return product;
    }

    public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
    {
        var products = await GetAllAsync();

        var product = products.FirstOrDefault(p => p.Id == id);
        if (product is null)
            return null;

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;

        await SaveAllAsync(products);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var products = await GetAllAsync();

        var product = products.FirstOrDefault(p => p.Id == id);
        if (product is null)
            return false;

        products.Remove(product);
        await SaveAllAsync(products);

        return true;
    }

    private async Task SaveAllAsync(List<Product> products)
    {
        var json = JsonSerializer.Serialize(products, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }
}