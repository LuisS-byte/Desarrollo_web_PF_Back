using System;
using System.Collections.Generic;

namespace Desarrollo_web_PF_Back.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdRol { get; set; }

    public string? UsuApellido { get; set; }

    public string? UsuNombre { get; set; }

    public string? UsuCorreo { get; set; }

    public string? UsuContraseña { get; set; }

    public bool? UsuInterno { get; set; }

    public DateOnly? UsuFecharegistro { get; set; }

    public virtual ICollection<ComentarioxTicket> ComentarioxTickets { get; set; } = new List<ComentarioxTicket>();

    public virtual Rol? Rol { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketxAsignacion> TicketxAsignacions { get; set; } = new List<TicketxAsignacion>();

    public virtual ICollection<TicketxCambioestado> TicketxCambioestados { get; set; } = new List<TicketxCambioestado>();
}
