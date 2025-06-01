using Desarrollo_web_PF_Back.Models;
using Desarrollo_web_PF_Back.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desarrollo_web_PF_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        private readonly AppDbContext _dbPruebaContext;

        public AdministradorController(AppDbContext appDbContext)
        {
            _dbPruebaContext = appDbContext;
        }





        [HttpGet]
        [Route("ListaConteoTickets")]
        public async Task<IActionResult> ListaConteoTickets()
        {
            var lista = await (from Ticket in _dbPruebaContext.Tickets
                               join Estado in _dbPruebaContext.Estados on Ticket.IdEstado equals Estado.IdEstado
                               select new
                               {
                                   Id = Ticket.IdTickets,
                                   Estado = Estado.EstNombre,
                                   Titulo = Ticket.TickDescripcion,
                                   Fecha = Ticket.TickFechacreacion
                               }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Route("TicketsPendientesAsignacion")]
        public async Task<IActionResult> TicketsPendientesAsignacion()
        {
            var lista = await (from Ticket in _dbPruebaContext.Tickets
                               join prioridad in _dbPruebaContext.Prioridads on Ticket.IdPrioridad equals prioridad.IdPrioridad
                               join Estado in _dbPruebaContext.Estados on Ticket.IdEstado equals Estado.IdEstado
                               join Service in _dbPruebaContext.Servicios on Ticket.IdServicio equals Service.IdServicio
                               where Ticket.IdEstado == 1
                               select new
                               {
                                   Id = Ticket.IdTickets,
                                   Titulo = Ticket.TickDescripcion,
                                   Fecha = Ticket.TickFechacreacion,
                                   Prioridad = prioridad.PrioriNombre,
                                   categoria = Service.ServNombre
                               }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });

        }

        [HttpGet]
        [Route("TicketsAsignados")]
        public async Task<IActionResult> TicketsAsignados()
        {
            var lista = await (from ticketAsig in _dbPruebaContext.TicketxAsignacions
                               join tickets in _dbPruebaContext.Tickets on ticketAsig.IdTicket equals tickets.IdTickets
                               join Estado in _dbPruebaContext.Estados on tickets.IdEstado equals Estado.IdEstado
                               join usuario in _dbPruebaContext.Usuarios on ticketAsig.IdUsuario equals usuario.IdUsuario
                               select new
                               {
                                   Id = tickets.IdTickets,
                                   Titulo = tickets.TickDescripcion,
                                   Estado = Estado.EstNombre,
                                   Tecnico = usuario.UsuNombre + " " + usuario.UsuApellido,
                                   FechaAsignacion = ticketAsig.FechaAsignacion
                               }).ToListAsync();

            return StatusCode(StatusCodes.Status200OK, new { value = lista });

        }

        [HttpGet]
        [Route("ListaPrioridades")]
        public async Task<IActionResult> ListaPrioridades()
        {
            var lista = await (from prioridad in _dbPruebaContext.Prioridads
                               select new { id = prioridad.IdPrioridad, nombre = prioridad.PrioriNombre, descripcion = prioridad.PrioriDescripcion }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }


        [HttpGet]
        [Route("ListaEstados")]
        public async Task<IActionResult> ListaEstados()
        {
            var lista = await (from estados in _dbPruebaContext.Estados
                               select new { id = estados.IdEstado, nombre = estados.EstNombre, descripcion = estados.EstDescripcion }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Route("ListaServicios")]
        public async Task<IActionResult> ListaServicios()
        {
            var lista = await (from servicios in _dbPruebaContext.Servicios
                               select new { id = servicios.IdServicio, categoria = servicios.ServNombre, descripcion = servicios.SerDescripcion }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Route("ListaTecnicos")]
        public async Task<IActionResult> ListaTecnicos()
        {
            var lista = await (from usuarios in _dbPruebaContext.Usuarios
                               where usuarios.IdUsuario == 2
                               select new
                               {
                                   id = usuarios.IdUsuario,
                                   nombre = usuarios.UsuNombre + " " + usuarios.UsuApellido
                               }
                               ).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }


        [HttpPost]
        [Route("CrearTicketAsignacion")]
        public async Task<IActionResult> CrearTicketAsignacion([FromBody] TicketAsignacionDTO ticketAsignacionDTO)
        {
            try
            {
                if (ticketAsignacionDTO == null || ticketAsignacionDTO.IdTicket <= 0 || ticketAsignacionDTO.IdUsuairo <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos inválidos para la asignación del ticket.",
                        receivedData = ticketAsignacionDTO
                    });
                }

                var ticket = await _dbPruebaContext.Tickets.FindAsync(ticketAsignacionDTO.IdTicket);
                if (ticket == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Ticket con ID {ticketAsignacionDTO.IdTicket} no encontrado."
                    });
                }

                var usuario = await _dbPruebaContext.Usuarios.FindAsync(ticketAsignacionDTO.IdUsuairo);
                if (usuario == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Usuario con ID {ticketAsignacionDTO.IdUsuairo} no encontrado."
                    });
                }

                var nuevaAsignacion = new TicketxAsignacion
                {
                    IdTicket = ticketAsignacionDTO.IdTicket,
                    IdUsuario = ticketAsignacionDTO.IdUsuairo,
                    FechaAsignacion = DateTime.Now,
                    Descripcion = ticketAsignacionDTO.descripcion
                };

                _dbPruebaContext.TicketxAsignacions.Add(nuevaAsignacion);
                await _dbPruebaContext.SaveChangesAsync();

                var ticket2 = await _dbPruebaContext.Tickets.FindAsync(ticketAsignacionDTO.IdTicket);

                if (ticket2 != null)
                {
                    ticket2.IdEstado = 4; 

                    _dbPruebaContext.Tickets.Update(ticket2);
                    await _dbPruebaContext.SaveChangesAsync(); 
                }


                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                // Loggear el error aquí
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Error interno al asignar el ticket.",
                    errorDetails = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

    }
}

        /*
    [HttpGet]
    [Route("ObtenerInfTockes")]
    public async Task<IActionResult> ObtenerInfTockes()
    {
            var lista = await (from TicketTotal in _dbPruebaContext.TicketxAsignacions 
                               join Ticket in _dbPruebaContext.Tickets on TicketTotal.IdTicket equals Ticket.IdTickets
                               join Prioridad in _dbPruebaContext.Prioridads on Ticket.IdPrioridad equals Prioridad.IdPrioridad
                               join Estado in _dbPruebaContext.Estados on Ticket.IdEstado equals Estado.IdEstado
                               select new
                               {

                               }
                               )
        }
    }
        */
    
