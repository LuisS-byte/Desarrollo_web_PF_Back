using Desarrollo_web_PF_Back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desarrollo_web_PF_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebaJwtController : ControllerBase
    {
        private readonly AppDbContext _dbPruebaContext;
        public PruebaJwtController(AppDbContext appDbContext)
        {
            _dbPruebaContext = appDbContext;
        }
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [Route("ListaAdmin")]
        public async Task<IActionResult> ListaAdmin()
        {
            var lista = await _dbPruebaContext.Prioridads.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }


        [HttpGet]
        [Authorize(Roles = "Soporte Técnico")]
        [Route("ListaSoporteTecnico")]
        public async Task<IActionResult> ListaUsuario()
        {
            var lista = await _dbPruebaContext.Prioridads.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }


        [HttpGet]
        [Authorize(Roles = "Usuario Final")]
        [Route("ListaUsuarioFinal")]
        public async Task<IActionResult> ListaInvitado()
        {
            var lista = await _dbPruebaContext.Prioridads.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }
    }
}
