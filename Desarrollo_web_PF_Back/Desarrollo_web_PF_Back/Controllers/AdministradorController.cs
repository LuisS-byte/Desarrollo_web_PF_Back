using Desarrollo_web_PF_Back.Custom;
using Desarrollo_web_PF_Back.Models;
using Desarrollo_web_PF_Back.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Desarrollo_web_PF_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        private readonly AppDbContext _dbPruebaContext;

        private readonly Utilidades _utilidades;

        public AdministradorController(AppDbContext appDbContext, IConfiguration configuration, Utilidades utilidades)
        {
            _dbPruebaContext = appDbContext;
            _utilidades = utilidades;
        }

        [HttpGet]
        [Route("InformacionUsuarios")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> InformacionUsuarios()
        {
            var lista = await (from usuario in _dbPruebaContext.Usuarios
                               join rol in _dbPruebaContext.Rols on usuario.IdRol equals rol.IdRol
                               select new
                               {
                                   Id = usuario.IdUsuario,
                                   nombre = usuario.UsuNombre,
                                   apellido = usuario.UsuApellido,
                                   Correo = usuario.UsuCorreo,
                                   Rol = rol.RolNombre,
                                   Interno = usuario.UsuInterno.HasValue && usuario.UsuInterno.Value ? "Sí" : "No" 
                               }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }


        [HttpPost]
        [Authorize(Roles = "Administrador, Soporte Técnico, Usuario Final")]
        [Route("EnviarCorreo")]
        public IActionResult EnviarCorreo([FromBody] CorreoDTO correo)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("DigitalTickets@gmail.com", "Sistema de Tickets");
                mail.To.Add(correo.Destinatario);
                mail.Subject = correo.Asunto;
                mail.Body = correo.Cuerpo;
                mail.IsBodyHtml = true; 

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("luis2004eduardocristales@gmail.com", "sayb sjhi blza fzml") 
                };

                smtp.Send(mail);
                return Ok(new { mensaje = "Correo enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al enviar el correo: " + ex.Message });
            }
        }




        [HttpPost]
        [Route("RegistrarUsuario")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {

            var modeloUsuario = new Usuario
            {
                UsuNombre = objeto.Nombre,
                UsuApellido = objeto.Apellido,
                UsuCorreo = objeto.Correo,
                IdRol = objeto.IdRol,
                UsuInterno = objeto.interno ==1 ? true:false,
                UsuContraseña = _utilidades.encriptarSHA256(objeto.Clave)
            };

            await _dbPruebaContext.Usuarios.AddAsync(modeloUsuario);
            await _dbPruebaContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSucces = true });

        }

        [HttpPatch]
        [Route("ActualizarUsuario")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarUsuario(UsuarioDTO objeto)
        {
            var modeloUsuario = await _dbPruebaContext.Usuarios.FindAsync(objeto.Id);
            if (modeloUsuario == null)
            {
                return NotFound(new { isSuccess = false, message = "Usuario no encontrado" });
            }
            if (objeto.Clave != null)
            {
                               modeloUsuario.UsuContraseña = _utilidades.encriptarSHA256(objeto.Clave);
            }
            modeloUsuario.UsuNombre = objeto.Nombre;
            modeloUsuario.UsuApellido = objeto.Apellido;
            modeloUsuario.UsuCorreo = objeto.Correo;
            modeloUsuario.IdRol = objeto.IdRol;
            modeloUsuario.UsuInterno = objeto.interno == 1 ? true : false;
            _dbPruebaContext.Usuarios.Update(modeloUsuario);
            await _dbPruebaContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        [HttpGet]
        [Route("ListaRoles")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await (from rol in _dbPruebaContext.Rols
                               select new
                               {
                                   id= rol.IdRol,
                                   NombreRol = rol.RolNombre
                               }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }


        [HttpGet]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador, Soporte Técnico")]
        [Route("ListaPrioridades")]
        public async Task<IActionResult> ListaPrioridades()
        {
            var lista = await (from prioridad in _dbPruebaContext.Prioridads
                               select new { id = prioridad.IdPrioridad, nombre = prioridad.PrioriNombre, descripcion = prioridad.PrioriDescripcion }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [Route("ListaEstados")]
        public async Task<IActionResult> ListaEstados()
        {
            var lista = await (from estados in _dbPruebaContext.Estados
                               select new { id = estados.IdEstado, nombre = estados.EstNombre, descripcion = estados.EstDescripcion }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Soporte Técnico")]
        [Route("ListaServicios")]
        public async Task<IActionResult> ListaServicios()
        {
            var lista = await (from servicios in _dbPruebaContext.Servicios
                               select new { id = servicios.IdServicio, categoria = servicios.ServNombre, descripcion = servicios.SerDescripcion }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [Route("ListaTecnicos")]
        public async Task<IActionResult> ListaTecnicos()
        {
            var lista = await (from usuarios in _dbPruebaContext.Usuarios
                               where usuarios.IdRol == 2
                               select new
                               {
                                   id = usuarios.IdUsuario,
                                   nombre = usuarios.UsuNombre + " " + usuarios.UsuApellido,
                                   correo = usuarios.UsuCorreo
                               }
                               ).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        [Route("GenerarReportePorCategoria")]
        public async Task<IActionResult> GenerarReportePorCategoria(int id)
        {
            var lista = await (from Ticket in _dbPruebaContext.Tickets
                               join estado in _dbPruebaContext.Estados on Ticket.IdEstado equals estado.IdEstado
                               join Prioridad in _dbPruebaContext.Prioridads on Ticket.IdPrioridad equals Prioridad.IdPrioridad
                               join servicio in _dbPruebaContext.Servicios on Ticket.IdServicio equals servicio.IdServicio
                               join usuario in _dbPruebaContext.Usuarios on Ticket.IdUsuario equals usuario.IdUsuario
                               where servicio.IdServicio == id
                               select new
                               {
                                   id = Ticket.IdTickets,
                                   categoria = servicio.ServNombre,
                                   titulo = Ticket.TickDescripcion,
                                   fechaCreacion = Ticket.TickFechacreacion,
                                   Encargado = usuario.UsuNombre + " " + usuario.UsuApellido,
                                   Prioridad = Prioridad.PrioriNombre,
                                   estado = estado.EstNombre,
                                   comentarios = (from comentarios in _dbPruebaContext.ComentarioxTickets where comentarios.IdTicket == Ticket.IdTickets select new { comentarios.ComenDescripcion }).ToList(),
                               }).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpGet]
        [Route("AllInfoTicket")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ListAllInfoTicketaPrioridades(int id)
        {
            var lista = await (from Ticket in _dbPruebaContext.Tickets
                               join estado in _dbPruebaContext.Estados on Ticket.IdEstado equals estado.IdEstado
                               join Prioridad in _dbPruebaContext.Prioridads on Ticket.IdPrioridad equals Prioridad.IdPrioridad
                               join servicio in _dbPruebaContext.Servicios on Ticket.IdPrioridad equals servicio.IdServicio
                               join usuario in _dbPruebaContext.Usuarios on Ticket.IdUsuario equals usuario.IdUsuario
                               join archivo in _dbPruebaContext.ArchivoTickets on Ticket.IdTickets equals archivo.IdTicket
                               where Ticket.IdTickets == id
                               select new
                               {

                                   id = Ticket.IdTickets,
                                   categoria = servicio.ServNombre,
                                   nombreCompletoUsuairo = usuario.UsuNombre+" "+usuario.UsuApellido,
                                   correoUsuario = usuario.UsuCorreo,
                                   prioridad = Prioridad.PrioriNombre,
                                   fechaCreacion = Ticket.TickFechacreacion,
                                   comentarios = (from comentarios in _dbPruebaContext.ComentarioxTickets where comentarios.IdTicket == id select new {comentarios.ComenDescripcion}).ToList(),
                                   rutaArchivo = archivo.ArRuta,
                                   descripcion = Ticket.TickDescripcion,
                                   idUsaurio = usuario.IdUsuario
                               }
                               ).ToListAsync();

            return StatusCode(StatusCodes.Status200OK, new { value = lista });
        }

        [HttpPost]
        [Route("agregarComentario")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> agregarComentario(ComentarioDTO dto)
        {
            var nuevaAsignacion = new ComentarioxTicket
            {
                IdTicket = dto.IdTicket,
                IdUsuario = dto.IdUsuario,
                ComenDescripcion = dto.Comentario
            };

            _dbPruebaContext.ComentarioxTickets.Add(nuevaAsignacion);
            await _dbPruebaContext.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
