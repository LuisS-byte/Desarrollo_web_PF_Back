using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class TicketxCambioestado
{
    public int IdComentario { get; set; }

    public int? IdTicket { get; set; }

    public int? IdUsuario { get; set; }

    public int? EstadoAnterior { get; set; }

    public int? EstadoNuevo { get; set; }

    public DateOnly? FechaCambio { get; set; }

    public virtual Estado? EstadoAnteriorNavigation { get; set; }

    public virtual Estado? EstadoNuevoNavigation { get; set; }

    public virtual Ticket? IdTicketNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
