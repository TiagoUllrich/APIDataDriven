using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

//https://localhost:5001/categories
[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")] 
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)] //Duração do cache de 30 minutos
    public async Task<ActionResult<List<Category>>> Get(    //Utilizado lista para quando chamar a rota sem parâmetro de id receba o cadastro de todos os itens
        [FromServices]DataContext context                   //Informar ao controller que o DataContext virá pelo Serviço sendo gerenciado pela injeção de dependencia do ASP .NET Core
    ) 
    {                                                    
        var categories = await context.Categories.AsNoTracking().ToListAsync(); //AsNoTracking realiza uma rápida leitura do banco. Utilizar ele quando realizar apenas leitura no método.
        return Ok(categories);                                                  //Quando utilizar alguma consulta que retornará dados do banco, ex: order by, where, etc; utilizar o ToList sempre no final.
    }

    [HttpGet]
    [Route("{id:int}")] //Validação para aceitar apenas numeros int na rota
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(
        int id,
        [FromServices]DataContext context
    )
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return Ok(category);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody]Category model, //FromBody capturará o corpo do Json
        [FromServices]DataContext context 
    )
    {
        if(!ModelState.IsValid) 
        return BadRequest(ModelState); //ModelState utiliza a validação do Data annotation da classe Category

        try
        {
            context.Categories.Add(model); //Categories é o DbSet utilizado em DataContext.cs
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (Exception)
        {            
            return BadRequest(new {message = "Não foi possível criar a categoria"});
        }
        
        
    }
    
    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Put(
        int id, 
        [FromBody]Category model,
        [FromServices]DataContext context
        )
    {
        
        if(id != model.Id) 
        return NotFound(new {message = "Categoria não encontrada"}); //Verifica se o Id informado é o mesmo do modelo

        if(!ModelState.IsValid) 
        return BadRequest(ModelState); //Verifica se os dados são válidos

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified; //Categories é o DbSet utilizado em DataContext.cs Entry é o metodo utilizado para alterar
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest (new {message = "Este registro já foi atualizado"});
        }
        catch (Exception)
        {            
            return BadRequest(new {message = "Não foi possível atualizar a categoria"});
        }
    }
        
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Delete(
        int id,
        [FromServices]DataContext context
    )
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id); //FirstOrDefaultAsync busca uma categoria através da expressão lambda. 
        if (category == null)                                                         //Caso encontre mais do que uma categoria, ele pegará a primeira. Caso não encontrar nada, retornará nulo.
            return NotFound(new {message = "Categoria não encontrada"});

        try
        {
            context.Categories.Remove(category); 
            await context.SaveChangesAsync();
            return Ok(new {message = "Categoria removida"});
        }
        catch (Exception)
        {            
            return BadRequest(new {message = "Não foi possível remover a categoria"});
        }   
    }
}