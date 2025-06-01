using Desarrollo_web_PF_Back.Models;
using Desarrollo_web_PF_Back.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Desarrollo_web_PF_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TicketController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearTicket([FromForm] CrearTicketDTO dto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int usuarioId = int.Parse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");


            // 🔍 Validación de existencia de claves foráneas
            var servicio = await _context.Servicios.FindAsync(dto.IdServicio);
            var prioridad = await _context.Prioridads.FindAsync(dto.IdPrioridad);

            if (servicio == null || prioridad == null)
                return BadRequest("Servicio o prioridad inválida.");

            // 🎫 Crear nuevo ticket
            var ticket = new Ticket
            {
                IdUsuario = usuarioId,
                IdServicio = dto.IdServicio,
                IdPrioridad = dto.IdPrioridad,
                IdEstado = 1, // Estado inicial: por ejemplo “Pendiente”
                TickDescripcion = dto.Descripcion,
                TickFechacreacion = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // 📎 Guardar archivo si se envió
            if (dto.Archivo != null && dto.Archivo.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "tickets");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Archivo.CopyToAsync(stream);
                }

                var archivo = new ArchivoTicket
                {
                    IdTicket = ticket.IdTickets,
                    ArNombre = dto.Archivo.FileName,
                    ArRuta = filePath,
                    TickFechacreacion = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.ArchivoTickets.Add(archivo);
                await _context.SaveChangesAsync();
            }

            // 📧 Aquí después haremos el envío del correo

            return Ok(new { mensaje = "Ticket creado correctamente", ticketId = ticket.IdTickets });
        }




        [HttpGet("mis-tickets")]
        public async Task<IActionResult> ObtenerTicketsUsuario()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int usuarioId = int.Parse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var tickets = await _context.Tickets
                .Where(t => t.IdUsuario == usuarioId)
                .Include(t => t.IdEstadoNavigation)
                .Include(t => t.IdPrioridadNavigation)
                .OrderByDescending(t => t.TickFechacreacion)
                .Select(t => new
                {
                    Id = t.IdTickets,
                    Descripcion = t.TickDescripcion,
                    Fecha = t.TickFechacreacion,
                    Estado = t.IdEstadoNavigation.EstNombre,
                    Prioridad = t.IdPrioridadNavigation.PrioriNombre
                })
                .ToListAsync();

            return Ok(tickets);
        }


    }



}
