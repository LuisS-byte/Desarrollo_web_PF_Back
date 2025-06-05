using Desarrollo_web_PF_Back.Custom;
using Desarrollo_web_PF_Back.Models;
using Desarrollo_web_PF_Back.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desarrollo_web_PF_Back.Controllers
{
    [Route("api/[controller]")]
    //esta api podemos acceder a ella sin necesidad de un token o que este autorizado
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly AppDbContext _dbPruebaContext;
        private readonly Utilidades _utilidades;
        public AccesoController(AppDbContext dbPruebaContext, Utilidades utilidades)
        {
            _dbPruebaContext = dbPruebaContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _dbPruebaContext.Usuarios
                .Include(u => u.Rol) // Asegurarse de incluir el rol en la consulta
                .Where(u =>
                    u.UsuCorreo == objeto.Correo &&
                    u.UsuContraseña == _utilidades.encriptarSHA256(objeto.Clave)
                )
                .FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });

            // Generar el token con el nombre del rol
            var token = _utilidades.GenerarToken(usuarioEncontrado);

            // Enviar respuesta con el token y el nombre del rol
            return StatusCode(StatusCodes.Status200OK, new
            {
                isSuccess = true,
                token = token,
                rol = usuarioEncontrado.Rol?.RolNombre
            });
        }


    }
}
