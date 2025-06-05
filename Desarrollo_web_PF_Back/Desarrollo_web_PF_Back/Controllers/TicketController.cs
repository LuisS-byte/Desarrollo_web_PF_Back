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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public TicketController(AppDbContext context, IWebHostEnvironment env, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _context = context;
            _env = env;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost("crear")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 50MB
        [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
        public async Task<IActionResult> CrearTicket([FromForm] CrearTicketDTO dto)
        {
            try
            {
                Console.WriteLine("=== INICIO CREAR TICKET ===");

                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int usuarioId = int.Parse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                Console.WriteLine($"Usuario ID: {usuarioId}");

                var servicio = await _context.Servicios.FindAsync(dto.IdServicio);
                var prioridad = await _context.Prioridads.FindAsync(dto.IdPrioridad);
                Console.WriteLine($"Servicio: {servicio?.IdServicio}, Prioridad: {prioridad?.IdPrioridad}");

                if (servicio == null || prioridad == null)
                    return BadRequest("Servicio o prioridad inválida.");

                Console.WriteLine("=== CREANDO TICKET EN BD ===");
                var ticket = new Ticket
                {
                    IdUsuario = usuarioId,
                    IdServicio = dto.IdServicio,
                    IdPrioridad = dto.IdPrioridad,
                    IdEstado = 1,
                    TickDescripcion = dto.Descripcion,
                    TickFechacreacion = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Ticket creado con ID: {ticket.IdTickets}");

                Console.WriteLine("=== VERIFICANDO ARCHIVO ===");
                if (dto.Archivo != null && dto.Archivo.Length > 0)
                {
                    Console.WriteLine($"Archivo detectado: {dto.Archivo.FileName}");
                    Console.WriteLine($"Tamaño: {dto.Archivo.Length} bytes");
                    Console.WriteLine($"Tipo: {dto.Archivo.ContentType}");

                    try
                    {
                        Console.WriteLine("=== PREPARANDO RUTAS ===");
                        string uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "tickets");
                        Console.WriteLine($"Ruta uploads: {uploadsFolder}");

                        Console.WriteLine("=== CREANDO DIRECTORIO ===");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
                        Console.WriteLine($"Nombre único: {uniqueFileName}");

                        string relativePath = Path.Combine("uploads", "tickets", uniqueFileName);
                        string fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                        Console.WriteLine($"Ruta completa: {fullPath}");

                        Console.WriteLine("=== GUARDANDO ARCHIVO ===");
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await dto.Archivo.CopyToAsync(stream);
                        }
                        Console.WriteLine("Archivo guardado exitosamente");

                        Console.WriteLine("=== GUARDANDO EN BD ===");
                        var archivo = new ArchivoTicket
                        {
                            IdTicket = ticket.IdTickets,
                            ArNombre = dto.Archivo.FileName,
                            ArRuta = relativePath,
                            TickFechacreacion = DateOnly.FromDateTime(DateTime.Now)
                        };

                        _context.ArchivoTickets.Add(archivo);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Registro de archivo guardado en BD");
                    }
                    catch (Exception exFile)
                    {
                        Console.WriteLine($"[ERROR ARCHIVO] {exFile.Message}");
                        Console.WriteLine($"[ERROR ARCHIVO STACK] {exFile.StackTrace}");
                        return StatusCode(500, $"Error al guardar archivo: {exFile.Message}");
                    }
                }
                Console.WriteLine("BUSCANDO AL USUARIO");
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                Console.WriteLine("¿Usuario encontrado? " + (usuario != null));
                if (usuario != null)
                    Console.WriteLine("Correo del usuario: " + usuario.UsuCorreo);

                if (usuario != null && !string.IsNullOrEmpty(usuario.UsuCorreo))
                {

                    Console.WriteLine("CREANDO CORREO");
                    var httpClient = _httpClientFactory.CreateClient();

                   
                    var token = HttpContext.Request.Headers["Authorization"].ToString();


                    if (!token.StartsWith("Bearer "))
                    {
                        token = "Bearer " + token;
                    }

                    var correoPayload = new
                    {
                        destinatario = usuario.UsuCorreo,
                        asunto = $"✅ Ticket #{ticket.IdTickets} creado correctamente",
                        cuerpo = $"""
                <h3>Hola {usuario.UsuNombre},</h3>
            <p>Tu ticket ha sido registrado con éxito.</p>
            <ul>
                <li><strong>ID:</strong> #{ticket.IdTickets}</li>
                <li><strong>Descripción:</strong> {ticket.TickDescripcion}</li>
                <li><strong>Fecha:</strong> {ticket.TickFechacreacion}</li>
            </ul>
            <p>Te mantendremos informado sobre su progreso.</p>
        """
                    };

                    var request = new HttpRequestMessage(HttpMethod.Post, _config["Api:BaseUrl"] + "/api/Administrador/EnviarCorreo")
                    {
                        Content = JsonContent.Create(correoPayload)
                    };

                    
                    request.Headers.Add("Authorization", token);

                    var response = await httpClient.SendAsync(request);
                    Console.WriteLine("SE CREEO EL TICKETTTT");
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("[WARN] El correo no pudo enviarse.");
                    }
                }

                Console.WriteLine("=== TICKET CREADO EXITOSAMENTE ===");
                return Ok(new { mensaje = "Ticket creado correctamente", ticketId = ticket.IdTickets });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL ERROR] {ex.Message}");
                Console.WriteLine($"[FATAL STACK] {ex.StackTrace}");
                return StatusCode(500, $"Error fatal: {ex.Message}");
            }
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
