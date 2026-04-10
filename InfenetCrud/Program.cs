using InfenetCrud.Models;
using InfenetCrud.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/products", async (ProductRepository repository) =>
{
    var products = await repository.GetAllAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id:guid}", async (Guid id, ProductRepository repository) =>
{
    var product = await repository.GetByIdAsync(id);

    return product is null
        ? Results.NotFound(new { message = "Produto não encontrado." })
        : Results.Ok(product);
});

app.MapPost("/products", async (Product product, ProductRepository repository) =>
{
    if (string.IsNullOrWhiteSpace(product.Name))
        return Results.BadRequest(new { message = "O nome é obrigatório." });

    if (product.Price <= 0)
        return Results.BadRequest(new { message = "O preço deve ser maior que zero." });

    var createdProduct = await repository.AddAsync(product);

    return Results.Created($"/products/{createdProduct.Id}", createdProduct);
});

app.MapPut("/products/{id:guid}", async (Guid id, Product product, ProductRepository repository) =>
{
    if (string.IsNullOrWhiteSpace(product.Name))
        return Results.BadRequest(new { message = "O nome é obrigatório." });

    if (product.Price <= 0)
        return Results.BadRequest(new { message = "O preço deve ser maior que zero." });

    var updatedProduct = await repository.UpdateAsync(id, product);

    return updatedProduct is null
        ? Results.NotFound(new { message = "Produto não encontrado." })
        : Results.Ok(updatedProduct);
});

app.MapDelete("/products/{id:guid}", async (Guid id, ProductRepository repository) =>
{
    var deleted = await repository.DeleteAsync(id);

    return deleted
        ? Results.NoContent()
        : Results.NotFound(new { message = "Produto não encontrado." });
});

app.Run();

public partial class Program { }