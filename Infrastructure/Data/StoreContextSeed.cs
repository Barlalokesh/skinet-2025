using System;
using System.Reflection;
using System.Text.Json;
using core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
       var basePath = Directory.GetCurrentDirectory();

        if (!context.Products.Any())
        {
         var productsPath = Path.Combine(basePath, "..", "Infrastructure", "Data", "SeedData", "products.json");
           var productsData = await File.ReadAllTextAsync(productsPath);

         var products = JsonSerializer.Deserialize<List<Product>>(productsData);
           
            if(products==null) return;
           
           context.Products.AddRange(products);

           await context.SaveChangesAsync();
        } 

        if (!context.DeliveryMethods.Any())
        {
        var dmPath = Path.Combine(basePath, "..", "Infrastructure", "Data", "SeedData", "delivery.json");
                 
            var dmData = await File.ReadAllTextAsync(dmPath);

            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);
           
            if(methods==null) return;
           
           context.DeliveryMethods.AddRange(methods);
           
           await context.SaveChangesAsync();
        } 
    }

}
