using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Ticket
{
    public int IdTickets { get; set; }

    public int? IdServicio { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdPrioridad { get; set; }

    public int? IdEstado { get; set; }

    public string? TickDescripcion { get; set; }

    public DateOnly? TickFechacreacion { get; set; }

    public DateOnly? TickFechacierre { get; set; }

    public virtual ICollection<ArchivoTicket> ArchivoTickets { get; set; } = new List<ArchivoTicket>();

    public virtual ICollection<ComentarioxTicket> ComentarioxTickets { get; set; } = new List<ComentarioxTicket>();

    public virtual Estado? IdEstadoNavigation { get; set; }

    public virtual Prioridad? IdPrioridadNavigation { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<TicketxAsignacion> TicketxAsignacions { get; set; } = new List<TicketxAsignacion>();

    public virtual ICollection<TicketxCambioestado> TicketxCambioestados { get; set; } = new List<TicketxCambioestado>();
}
