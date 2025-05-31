using Desarrollo_web_PF_Back.Models;
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
    
