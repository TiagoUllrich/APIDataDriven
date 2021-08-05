using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody]User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {                
                model.Role = "employee"; //Força o usuário a ser sempre "funcionário"

                context.Users.Add(model);
                await context.SaveChangesAsync();
                
                model.Password = ""; //Esconde a senha
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new {message = "Não foi possível criar o usuário"});
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate( //ActionResult como dynamic porque as vezes retorna um usuário ou pode não ter retorno
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            var user = await context.Users.AsNoTracking().Where(x => x.Name == model.Name && x.Password == model.Password).FirstOrDefaultAsync();

            if(user == null)
                return NotFound(new {message = "Usuário ou senha inválidos"});

            var token = TokenService.GenerateToken(user); //Geração do token para o usuário
            user.Password = ""; //Esconde a senha
            return new //Retorna um novo usuário
            {
                user = user,
                token = token
            };
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            int id,
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(id != model.Id)
                return NotFound(new {message = "Usuário não encontrado"});

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {                
                return BadRequest(new {message = "Não foi possível editar o usuário"});;
            }
        }
    }
}