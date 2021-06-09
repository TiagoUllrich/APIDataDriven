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
        return Ok(products);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> GetById(
        int id,
        [FromServices] DataContext context
    )
    {
        var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return Ok(product);
    }

    [HttpGet] //products/categories/Id da categoria
    [Route("categories/{id:int}")]
    public async Task<ActionResult<Product>> GetByCategory( //Get utilizado para listar todos os produtos de acordo com o Id da categoria passado
        int id,
        [FromServices] DataContext context
    )
    {
        var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Product>> Post(
        [FromBody]Product model,
        [FromServices]DataContext context
    )
    {
        if (!ModelState.IsValid)
        return BadRequest(ModelState);

        try
        {
            context.Products.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (Exception)
        {
            return BadRequest(new {message = "Não foi possivel criar o produto"});
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> Put(
        int id,
        [FromBody]Product model,
        [FromServices]DataContext context
    )
    {
        if(id != model.Id)
        return NotFound(new {message = "Produto não encontrado"});

        if(!ModelState.IsValid)
        return BadRequest(ModelState);

        try
        {
            context.Entry<Product>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {            
            return BadRequest (new {message = "Este registro já foi atualizado"});
        }
        catch (Exception)
        {            
            return BadRequest(new {message = "Não foi possível atualizar o produto"});
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> Delete(
        int id,
        [FromServices]DataContext context
    )
    {
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if(product == null)
        return NotFound(new {message = "Produto não encontrado"});

        try
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return Ok(new {message = "Produto removido"});
        }
        catch (Exception)
        {            
            return BadRequest(new {message = "Não foi possível remover o produto"});
        }
    }
}