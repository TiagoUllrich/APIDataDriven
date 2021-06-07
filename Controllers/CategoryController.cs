using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

//https://localhost:5001/categories
[Route("categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")] //Validação para aceitar apenas numeros int na rota
    public async Task<ActionResult<List<Category>>> Get() //Utilizado lista para quando chamar a rota sem parâmetro de id
    {                                                    //receba o cadastro de todos os itens
        return new List<Category>();
    }

    [HttpGet]
    [Route("{id:int}")] //Validação para aceitar apenas numeros int na rota
    public async Task<ActionResult<Category>> GetById(int id)
    {
        return new Category();
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody]Category model, //FromBody capturará o corpo do Json
        [FromServices]DataContext context //
    )
    {
        if(!ModelState.IsValid) 
        return BadRequest(ModelState); //ModelState utiliza a validação do Data annotation da classe Category
        
        return Ok(model);
    }
    
    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Category>>> Put(int id, [FromBody]Category model)
    {
        //Verifica se o Id informado é o mesmo do modelo
        if(id != model.Id) return NotFound(new {message = "Categoria não encontrada"});

        if(!ModelState.IsValid) return BadRequest(ModelState);

        return Ok(model);
    }
        
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Category>>> Delete()
    {
        return Ok();
    }
}