using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Product>>> Get(
        [FromServices] DataContext context
    )
    {
        var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
        return products;
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> GetById(
        int id,
        [FromServices] DataContext context
    )
    {
        var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return product;
    }

    [HttpGet] //products/categories/Id da categoria
    [Route("categories/{id:int}")]
    public async Task<ActionResult<List<Product>>> GetByCategory( //Get utilizado para listar todos os produtos de acordo com o Id da categoria passado
        int id,
        [FromServices] DataContext context
    )
    {
        var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
        return products;
    }
}