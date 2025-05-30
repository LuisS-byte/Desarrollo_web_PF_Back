using System;
using System.Collections.Generic;

namespace Desarrollo_web_PF_Back.Models;

public partial class ArchivoTicket
{
    public int IdArtick { get; set; }

    public int? IdTicket { get; set; }

    public string? ArNombre { get; set; }

    public string? ArRuta { get; set; }

    public DateOnly? TickFechacreacion { get; set; }

    public virtual Ticket? IdTicketNavigation { get; set; }
}
