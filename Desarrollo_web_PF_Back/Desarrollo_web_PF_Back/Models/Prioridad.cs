using System;
using System.Collections.Generic;

namespace Desarrollo_web_PF_Back.Models;

public partial class Prioridad
{
    public int IdPrioridad { get; set; }

    public string? PrioriNombre { get; set; }

    public string? PrioriDescripcion { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
