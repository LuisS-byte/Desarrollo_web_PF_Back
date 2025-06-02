using Desarrollo_web_PF_Back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Desarrollo_web_PF_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : Controller
    {
        private readonly AppDbContext _context;

        public CatalogoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("servicios")]
        public async Task<IActionResult> ObtenerServicios()
        {
            var servicios = await _context.Servicios
                .Select(s => new { s.IdServicio, s.ServNombre })
                .ToListAsync();
            return Ok(servicios);
        }

        [HttpGet("prioridades")]
        public async Task<IActionResult> ObtenerPrioridades()
        {
            var prioridades = await _context.Prioridads
                .Select(p => new { p.IdPrioridad, p.PrioriNombre })
                .ToListAsync();
            return Ok(prioridades);
        }
    }
}
