using core.Entities;
using core.Interface;
using core.Specifications;
using Microsoft.AspNetCore.Mvc;
using API.RequestHelpers;

namespace API.Controllers;

public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
        [FromQuery]ProductSpecParams specParams)
    {
       var spec = new ProductSpecification(specParams);

       return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
   }
    
    [HttpGet("{id:int}")] // api/products/3
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null) return NotFound();

        return product;
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.Add(product);

        if(await repo.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        return BadRequest("Failed to create product");
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id  || !ProductExists(id)) return BadRequest("cannot update this product");

        repo.Update(product);

        if(await repo.SaveChangesAsync())
        {
             return NoContent();
        }
        return BadRequest("Failed to update product");
       
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null) return NotFound();

       repo.Remove(product);

        if(await repo.SaveChangesAsync())
        {
             return NoContent();
        }
        return BadRequest("Product deletion failed");
    }
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    { 
        var spec = new BrandListSpecification();

            return Ok( await repo.ListAsync(spec));
        
    }
     [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
         var spec = new TypeListSpecification();
            return Ok( await repo.ListAsync(spec));
        
    }
    private bool ProductExists(int id)
    {
        return repo.Exists(id);
    }

    
}

