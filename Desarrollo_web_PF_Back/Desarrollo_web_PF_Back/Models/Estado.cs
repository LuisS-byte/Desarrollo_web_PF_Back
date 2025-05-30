using System;
using System.Collections.Generic;

namespace Desarrollo_web_PF_Back.Models;

public partial class Estado
{
    public int IdEstado { get; set; }

    public string? EstNombre { get; set; }

    public string? EstDescripcion { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketxCambioestado> TicketxCambioestadoEstadoAnteriorNavigations { get; set; } = new List<TicketxCambioestado>();

    public virtual ICollection<TicketxCambioestado> TicketxCambioestadoEstadoNuevoNavigations { get; set; } = new List<TicketxCambioestado>();
}
