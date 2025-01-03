using DataEntities;
using Microsoft.EntityFrameworkCore;
using Products.Data;

namespace Products.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product");

        group.MapGet("/", async (ProductDataContext db) =>
        {
            return await db.Product.ToListAsync();
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async  (int id, ProductDataContext db) =>
        {
            return await db.Product.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Product model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // initial MapPut
        //group.MapPut("/{id}", async (int id, Product product, ProductDataContext db) =>
        //{
        //    var affected = await db.Product
        //        .Where(model => model.Id == id)
        //        .ExecuteUpdateAsync(setters => setters
        //          .SetProperty(m => m.Id, product.Id)
        //          .SetProperty(m => m.Name, product.Name)
        //          .SetProperty(m => m.Description, product.Description)
        //          .SetProperty(m => m.Price, product.Price)
        //          .SetProperty(m => m.ImageUrl, product.ImageUrl)
        //          .SetProperty(m => m.Stock, product.Stock)
        //        );

        //    return affected == 1 ? Results.Ok() : Results.NotFound();
        //})
        //.WithName("UpdateProduct")
        //.Produces(StatusCodes.Status404NotFound)
        //.Produces(StatusCodes.Status204NoContent);

        // MapPut with stock change and extended observability
        group.MapPut("/{id}", async (int id, Product product, ProductDataContext db, ProductsMetrics metrics) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, product.Id)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.Price, product.Price)
                  .SetProperty(m => m.ImageUrl, product.ImageUrl)
                  .SetProperty(m => m.Stock, product.Stock)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces<Product>(StatusCodes.Status201Created)
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (Product product, ProductDataContext db) =>
        {
            db.Product.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Product/{product.Id}",product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async  (int id, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        var stock = routes.MapGroup("/api/Stock");

        stock.MapGet("/{id}", async  (int id, ProductDataContext db) =>
        {
            return await db.Product.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Product model
                    ? Results.Ok(model.Stock)
                    : Results.NotFound();
        })
        .WithName("GetStockById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // initial MapPut
        //stock.MapPut("/{id}", async  (int id, int stockAmount, ProductDataContext db) =>
        //{
        //    var affected = await db.Product
        //        .Where(model => model.Id == id)
        //        .ExecuteUpdateAsync(setters => setters
        //          .SetProperty(m => m.Stock, stockAmount)
        //        );

        //    return affected == 1 ? Results.Ok() : Results.NotFound();
        //})
        //.WithName("UpdateStockById")
        //.Produces<Product>(StatusCodes.Status200OK)
        //.Produces(StatusCodes.Status404NotFound);

        // MapPut with stock change and extended observability
        stock.MapPut("/{id}", async (int id, int stockAmount, ProductDataContext db, ProductsMetrics metrics) =>
        {
            // Increment the stock change metric.
            metrics.StockChange(stockAmount);

            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Stock, stockAmount)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateStockById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
