using System;
using System.Collections.Generic;

namespace Desarrollo_web_PF_Back.Models;

public partial class TicketxAsignacion
{
    public int IdAsignacion { get; set; }

    public int? IdTicket { get; set; }

    public int? IdUsuario { get; set; }

    public string? Descripcion { get; set; }

    public virtual Ticket? IdTicketNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
