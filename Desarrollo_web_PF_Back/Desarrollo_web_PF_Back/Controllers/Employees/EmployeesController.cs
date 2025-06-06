using Desarrollo_web_PF_Back.Models;
using Desarrollo_web_PF_Back.Models.DTOs.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Desarrollo_web_PF_Back.Controllers.Employees
{
    [ApiController]
    [Route("api/employees")]
    //[Authorize] // HABILITADO - Requiere autenticación JWT
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(AppDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }























        /// <summary>
        /// Obtiene los tickets del usuario autenticado con paginación y estadísticas
        /// </summary>
        /// <param name="currentPage">Página actual (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 10, máximo 100)</param>
        [HttpGet("my-tickets")]
        [Authorize(Roles = "Soporte Técnico")]
        public async Task<IActionResult> GetMyTickets(
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Obtener el ID del usuario autenticado desde el token JWT
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                {
                    _logger.LogWarning("No se pudo obtener el ID del usuario autenticado desde el token");
                    return Unauthorized(new { message = "Usuario no autenticado o token inválido" });
                }

                // Verificar que el usuario existe en la base de datos
                var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == userId);
                if (!usuarioExiste)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado en base de datos", userId);
                    return NotFound(new { message = $"Usuario con ID {userId} no encontrado" });
                }

                _logger.LogInformation("Obteniendo tickets para usuario autenticado {UserId} - Página: {CurrentPage}, Tamaño: {PageSize}", 
                    userId, currentPage, pageSize);

                // Validaciones
                if (currentPage < 1) currentPage = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // Query base para los tickets del usuario
                var baseQuery = _context.Tickets
                    .Where(t => t.IdUsuario == userId)
                    .Include(t => t.IdEstadoNavigation)
                    .Include(t => t.IdPrioridadNavigation);

                // Obtener estadísticas
                var allUserTickets = await baseQuery.ToListAsync();
                var statistics = new StatisticsDTO
                {
                    Open = allUserTickets.Count(t => t.IdEstadoNavigation?.EstNombre?.ToLower() == "abierto" || 
                                                    t.IdEstadoNavigation?.EstNombre?.ToLower() == "pendiente"),
                    InProgress = allUserTickets.Count(t => t.IdEstadoNavigation?.EstNombre?.ToLower().Contains("progreso") == true ||
                                                          t.IdEstadoNavigation?.EstNombre?.ToLower() == "asignado"),
                    Resolved = allUserTickets.Count(t => t.IdEstadoNavigation?.EstNombre?.ToLower() == "resuelto" ||
                                                        t.IdEstadoNavigation?.EstNombre?.ToLower() == "cerrado"),
                    Total = allUserTickets.Count
                };

                // Calcular metadata de paginación
                var totalTickets = allUserTickets.Count;
                var totalPages = (int)Math.Ceiling(totalTickets / (double)pageSize);

                // Obtener tickets paginados
                var tickets = await baseQuery
                    .OrderByDescending(t => t.TickFechacreacion)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new UserTicketDTO
                    {
                        Id = t.IdTickets.ToString().PadLeft(3, '0'), // Formato "001", "002", etc.
                        Title = t.TickDescripcion ?? "Sin título",
                        Status = t.IdEstadoNavigation!.EstNombre ?? "Sin estado",
                        Priority = t.IdPrioridadNavigation!.PrioriNombre ?? "Sin prioridad",
                        CreationDate = t.TickFechacreacion != null ? t.TickFechacreacion.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue
                    })
                    .ToListAsync();

                // Crear respuesta
                var response = new UserTicketsResponseDTO
                {
                    Metadata = new MetadataDTO
                    {
                        TotalTickets = totalTickets,
                        TotalPages = totalPages,
                        CurrentPage = currentPage,
                        PageSize = pageSize
                    },
                    Statistics = statistics,
                    Tickets = tickets
                };

                _logger.LogInformation("Tickets obtenidos exitosamente para usuario {UserId} - Total: {TotalTickets}, Página: {CurrentPage}/{TotalPages}", 
                    userId, totalTickets, currentPage, totalPages);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets del usuario");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los tickets ASIGNADOS al usuario autenticado (como técnico) con paginación y estadísticas
        /// </summary>
        /// <param name="currentPage">Página actual (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 10, máximo 100)</param>
        [HttpGet("assigned-tickets")]
        [Authorize(Roles = "Soporte Técnico")]
        public async Task<IActionResult> GetAssignedTickets(
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Obtener el ID del usuario autenticado desde el token JWT
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                {
                    _logger.LogWarning("No se pudo obtener el ID del usuario autenticado desde el token");
                    return Unauthorized(new { message = "Usuario no autenticado o token inválido" });
                }

                // Verificar que el usuario existe en la base de datos
                var usuarioExiste = await (from usuario in _context.Usuarios
                                           where usuario.IdUsuario == userId
                                           select new { usuario.UsuNombre, usuario.UsuApellido, usuario.UsuCorreo , usuario.IdRol})
                           .ToListAsync();
                if (usuarioExiste==null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado en base de datos", userId);
                    return NotFound(new { message = $"Usuario con ID {userId} no encontrado" });
                }

                _logger.LogInformation("Obteniendo tickets asignados para usuario autenticado {UserId} - Página: {CurrentPage}, Tamaño: {PageSize}", 
                    userId, currentPage, pageSize);

                // Validaciones
                if (currentPage < 1) currentPage = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // Query para obtener tickets asignados al usuario
                var baseQuery = _context.Tickets
                    .Where(t => t.TicketxAsignacions.Any(a => a.IdUsuario == userId))
                    .Include(t => t.IdEstadoNavigation)
                    .Include(t => t.IdPrioridadNavigation)
                    .Include(t => t.IdServicioNavigation)
                    .Include(t => t.IdUsuarioNavigation)
                    .Include(t => t.TicketxAsignacions.Where(a => a.IdUsuario == userId));

                // Obtener todos los tickets asignados para estadísticas
                var allAssignedTickets = await baseQuery.ToListAsync();
                
                var statistics = new StatisticsDTO
                {
                    Open = allAssignedTickets.Count(t => t.IdEstadoNavigation?.EstNombre?.ToLower() == "abierto" || 
                                                        t.IdEstadoNavigation?.EstNombre?.ToLower() == "pendiente"),
                    InProgress = allAssignedTickets.Count(t => t.IdEstadoNavigation?.EstNombre?.ToLower().Contains("progreso") == true ||
                                                              t.IdEstadoNavigation?.EstNombre?.ToLower() == "asignado"),
                    Resolved = allAssignedTickets.Count(t => t.IdEstadoNavigation?.EstNombre?.ToLower() == "resuelto" ||
                                                            t.IdEstadoNavigation?.EstNombre?.ToLower() == "cerrado"),
                    Total = allAssignedTickets.Count
                };

                // Calcular metadata de paginación
                var totalTickets = allAssignedTickets.Count;
                var totalPages = (int)Math.Ceiling(totalTickets / (double)pageSize);

                // Obtener tickets paginados con información de asignación
                var tickets = await baseQuery
                    .OrderByDescending(t => t.TicketxAsignacions
                        .Where(a => a.IdUsuario == userId)
                        .Max(a => a.FechaAsignacion))
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new AssignedTicketDTO
                    {
                        Id = t.IdTickets.ToString().PadLeft(3, '0'),
                        Title = t.TickDescripcion ?? "Sin título",
                        Status = t.IdEstadoNavigation!.EstNombre ?? "Sin estado",
                        Priority = t.IdPrioridadNavigation!.PrioriNombre ?? "Sin prioridad",
                        Service = t.IdServicioNavigation!.ServNombre ?? "Sin servicio",
                        CreationDate = t.TickFechacreacion != null ? t.TickFechacreacion.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                        AssignmentDate = t.TicketxAsignacions
                            .Where(a => a.IdUsuario == userId)
                            .OrderByDescending(a => a.FechaAsignacion)
                            .Select(a => a.FechaAsignacion)
                            .FirstOrDefault() ?? DateTime.MinValue,
                        CreatedBy = $"{t.IdUsuarioNavigation!.UsuNombre} {t.IdUsuarioNavigation.UsuApellido}",
                        AssignmentDescription = t.TicketxAsignacions
                            .Where(a => a.IdUsuario == userId)
                            .OrderByDescending(a => a.FechaAsignacion)
                            .Select(a => a.Descripcion)
                            .FirstOrDefault()
                    })
                    .ToListAsync();


                // Crear respuesta específica para tickets asignados
                var response = new AssignedTicketsResponseDTO
                {
                    Metadata = new MetadataDTO
                    {
                        TotalTickets = totalTickets,
                        TotalPages = totalPages,
                        CurrentPage = currentPage,
                        PageSize = pageSize
                    },
                    Statistics = statistics,
                    Tickets = tickets,
                    usuario = new UsuarioDTO
                    {
                        Id = userId,
                        Nombre = usuarioExiste.FirstOrDefault()?.UsuNombre,
                        Apellido = usuarioExiste.FirstOrDefault()?.UsuApellido,
                        Correo = usuarioExiste.FirstOrDefault()?.UsuCorreo ?? "No disponible"

                    }
                };

                _logger.LogInformation("Tickets asignados obtenidos exitosamente para usuario {UserId} - Total: {TotalTickets}, Página: {CurrentPage}/{TotalPages}", 
                    userId, totalTickets, currentPage, totalPages);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets asignados del usuario");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un ticket específico por ID (solo si pertenece al usuario autenticado)
        /// </summary>
        /// <param name="id">ID del ticket</param>
        [HttpGet("my-tickets/{id}")]
        [Authorize(Roles = "Soporte Técnico")]
        public async Task<IActionResult> GetMyTicketById(int id)
        {
            try
            {
                // Obtener el ID del usuario autenticado desde el token JWT
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                {
                    _logger.LogWarning("No se pudo obtener el ID del usuario autenticado desde el token");
                    return Unauthorized(new { message = "Usuario no autenticado o token inválido" });
                }

                // Verificar que el usuario existe en la base de datos
                var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == userId);
                if (!usuarioExiste)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado en base de datos", userId);
                    return NotFound(new { message = $"Usuario con ID {userId} no encontrado" });
                }

                _logger.LogInformation("Obteniendo ticket {TicketId} para usuario autenticado {UserId}", id, userId);

                var ticket = await _context.Tickets
                    .Where(t => t.IdTickets == id && t.IdUsuario == userId) // Solo tickets del usuario
                    .Include(t => t.IdEstadoNavigation)
                    .Include(t => t.IdPrioridadNavigation)
                    .Include(t => t.IdServicioNavigation)
                    .Include(t => t.IdUsuarioNavigation)
                    .Include(t => t.TicketxAsignacions)
                        .ThenInclude(a => a.IdUsuarioNavigation)
                    .Include(t => t.ArchivoTickets)
                    .Include(t => t.ComentarioxTickets)
                        .ThenInclude(c => c.IdUsuarioNavigation)
                    .Include(t => t.TicketxCambioestados)
                        .ThenInclude(c => c.IdUsuarioNavigation)
                    .Include(t => t.TicketxCambioestados)
                        .ThenInclude(c => c.EstadoAnteriorNavigation)
                    .Include(t => t.TicketxCambioestados)
                        .ThenInclude(c => c.EstadoNuevoNavigation)
                    .FirstOrDefaultAsync();

                if (ticket == null)
                {
                    _logger.LogWarning("Ticket {TicketId} no encontrado para usuario {UserId}", id, userId);
                    return NotFound(new { message = $"Ticket con ID {id} no encontrado o no pertenece al usuario {userId}" });
                }

                var tecnicoAsignado = ticket.TicketxAsignacions
                    .OrderByDescending(a => a.FechaAsignacion)
                    .FirstOrDefault()?.IdUsuarioNavigation;

                var result = new TicketDetailDTO
                {
                    Id = ticket.IdTickets,
                    Descripcion = ticket.TickDescripcion,
                    FechaCreacion = ticket.TickFechacreacion,
                    FechaCierre = ticket.TickFechacierre,
                    Estado = new EstadoDTO
                    {
                        Id = ticket.IdEstadoNavigation!.IdEstado,
                        Nombre = ticket.IdEstadoNavigation.EstNombre,
                        Descripcion = ticket.IdEstadoNavigation.EstDescripcion
                    },
                    Prioridad = new PrioridadDTO
                    {
                        Id = ticket.IdPrioridadNavigation!.IdPrioridad,
                        Nombre = ticket.IdPrioridadNavigation.PrioriNombre,
                        Descripcion = ticket.IdPrioridadNavigation.PrioriDescripcion
                    },
                    Servicio = new ServicioDTO
                    {
                        Id = ticket.IdServicioNavigation!.IdServicio,
                        Nombre = ticket.IdServicioNavigation.ServNombre,
                        Descripcion = ticket.IdServicioNavigation.SerDescripcion
                    },
                    UsuarioCreador = new UsuarioDTO
                    {
                        Id = ticket.IdUsuarioNavigation!.IdUsuario,
                        Nombre = ticket.IdUsuarioNavigation.UsuNombre,
                        Apellido = ticket.IdUsuarioNavigation.UsuApellido,
                        Correo = ticket.IdUsuarioNavigation.UsuCorreo
                    },
                    TecnicoAsignado = tecnicoAsignado != null ? new UsuarioDTO
                    {
                        Id = tecnicoAsignado.IdUsuario,
                        Nombre = tecnicoAsignado.UsuNombre,
                        Apellido = tecnicoAsignado.UsuApellido,
                        Correo = tecnicoAsignado.UsuCorreo
                    } : null,
                    Archivos = ticket.ArchivoTickets.Select(a => new ArchivoDTO
                    {
                        Id = a.IdArtick,
                        Nombre = a.ArNombre,
                        Ruta = a.ArRuta,
                        FechaCreacion = a.TickFechacreacion
                    }).ToList(),
                    Comentarios = ticket.ComentarioxTickets.Select(c => new ComentarioDTO
                    {
                        Id = c.IdComentario,
                        Descripcion = c.ComenDescripcion,
                        Usuario = new UsuarioDTO
                        {
                            Id = c.IdUsuarioNavigation!.IdUsuario,
                            Nombre = c.IdUsuarioNavigation.UsuNombre,
                            Apellido = c.IdUsuarioNavigation.UsuApellido,
                            Correo = c.IdUsuarioNavigation.UsuCorreo
                        }
                    }).ToList(),
                    Asignaciones = ticket.TicketxAsignacions.Select(a => new AsignacionDTO
                    {
                        Id = a.IdAsignacion,
                        Descripcion = a.Descripcion,
                        FechaAsignacion = a.FechaAsignacion,
                        Usuario = new UsuarioDTO
                        {
                            Id = a.IdUsuarioNavigation!.IdUsuario,
                            Nombre = a.IdUsuarioNavigation.UsuNombre,
                            Apellido = a.IdUsuarioNavigation.UsuApellido,
                            Correo = a.IdUsuarioNavigation.UsuCorreo
                        }
                    }).OrderByDescending(a => a.FechaAsignacion).ToList(),
                    CambiosEstado = ticket.TicketxCambioestados.Select(c => new CambioEstadoDTO
                    {
                        Id = c.IdComentario,
                        FechaCambio = c.FechaCambio,
                        EstadoAnterior = new EstadoDTO
                        {
                            Id = c.EstadoAnteriorNavigation!.IdEstado,
                            Nombre = c.EstadoAnteriorNavigation.EstNombre,
                            Descripcion = c.EstadoAnteriorNavigation.EstDescripcion
                        },
                        EstadoNuevo = new EstadoDTO
                        {
                            Id = c.EstadoNuevoNavigation!.IdEstado,
                            Nombre = c.EstadoNuevoNavigation.EstNombre,
                            Descripcion = c.EstadoNuevoNavigation.EstDescripcion
                        },
                        Usuario = new UsuarioDTO
                        {
                            Id = c.IdUsuarioNavigation!.IdUsuario,
                            Nombre = c.IdUsuarioNavigation.UsuNombre,
                            Apellido = c.IdUsuarioNavigation.UsuApellido,
                            Correo = c.IdUsuarioNavigation.UsuCorreo
                        }
                    }).OrderByDescending(c => c.FechaCambio).ToList()
                };

                _logger.LogInformation("Ticket {TicketId} obtenido exitosamente para usuario {UserId}", id, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo ticket para el usuario autenticado
        /// </summary>
        /// <param name="dto">Datos del ticket a crear</param>
        [HttpPost("create-ticket")]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50MB
        [Authorize(Roles = "Soporte Técnico")]
        [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
        public async Task<IActionResult> CreateTicket([FromForm] CreateEmployeeTicketDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Correo))
                {
                    _logger.LogWarning("Correo electrónico no proporcionado");
                    return BadRequest(new { message = "El correo electrónico es requerido" });
                }

                _logger.LogInformation("Buscando usuario por correo: {Correo}", dto.Correo);

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuCorreo == dto.Correo);

                if (usuario == null)
                {
                    _logger.LogInformation("Usuario no encontrado. Registrando usuario externo con correo: {Correo}", dto.Correo);

                    usuario = new Usuario
                    {
                        UsuNombre = dto.Nombre ?? "Usuario Externo",
                        UsuCorreo = dto.Correo,
                        IdRol = 3,
                        UsuInterno = false
                    };

                    await _context.Usuarios.AddAsync(usuario);
                    await _context.SaveChangesAsync();

                    if (usuario.IdUsuario == 0)
                    {
                        _logger.LogError("No se pudo registrar el usuario externo con correo: {Correo}", dto.Correo);
                        return StatusCode(500, new { message = "Error al registrar usuario externo" });
                    }

                    _logger.LogInformation("Usuario externo registrado exitosamente con ID: {IdUsuario}", usuario.IdUsuario);
                }

                int userId = usuario.IdUsuario;

                // Validar servicio y prioridad
                var servicio = await _context.Servicios.FindAsync(dto.IdServicio);
                var prioridad = await _context.Prioridads.FindAsync(dto.IdPrioridad);

                if (servicio == null)
                {
                    _logger.LogWarning("Servicio con ID {IdServicio} no encontrado", dto.IdServicio);
                    return BadRequest(new { message = "Servicio no válido" });
                }

                if (prioridad == null)
                {
                    _logger.LogWarning("Prioridad con ID {IdPrioridad} no encontrada", dto.IdPrioridad);
                    return BadRequest(new { message = "Prioridad no válida" });
                }

                if (string.IsNullOrWhiteSpace(dto.Descripcion))
                {
                    return BadRequest(new { message = "La descripción del ticket es requerida" });
                }

                _logger.LogInformation("Creando ticket en base de datos para usuario {UserId}", userId);

                var ticket = new Ticket
                {
                    IdUsuario = userId,
                    IdServicio = dto.IdServicio,
                    IdPrioridad = dto.IdPrioridad,
                    IdEstado = 1,
                    TickDescripcion = dto.Descripcion,
                    TickFechacreacion = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Ticket creado con ID: {TicketId}", ticket.IdTickets);

                // Procesar archivo si existe
                if (dto.Archivo != null && dto.Archivo.Length > 0)
                {
                    _logger.LogInformation("Procesando archivo adjunto: {FileName}, Tamaño: {FileSize} bytes",
                        dto.Archivo.FileName, dto.Archivo.Length);

                    try
                    {
                        string uploadsFolder = Path.Combine("wwwroot", "uploads", "tickets");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
                        string relativePath = Path.Combine("uploads", "tickets", uniqueFileName);
                        string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await dto.Archivo.CopyToAsync(stream);
                        }

                        var archivo = new ArchivoTicket
                        {
                            IdTicket = ticket.IdTickets,
                            ArNombre = dto.Archivo.FileName,
                            ArRuta = relativePath,
                            TickFechacreacion = DateOnly.FromDateTime(DateTime.Now)
                        };

                        _context.ArchivoTickets.Add(archivo);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Archivo guardado exitosamente: {FilePath}", relativePath);
                    }
                    catch (Exception exFile)
                    {
                        _logger.LogError(exFile, "Error al procesar archivo adjunto para ticket {TicketId}", ticket.IdTickets);
                    }
                }

                var response = new CreateTicketResponseDTO
                {
                    Success = true,
                    Message = "Ticket creado exitosamente",
                    TicketId = ticket.IdTickets,
                    TicketNumber = ticket.IdTickets.ToString().PadLeft(3, '0')
                };

                _logger.LogInformation("Ticket creado exitosamente para usuario {UserId} con ID {TicketId}",
                    userId, ticket.IdTickets);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear ticket");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al crear el ticket",
                    error = ex.Message
                });
            }
        }

    }
}