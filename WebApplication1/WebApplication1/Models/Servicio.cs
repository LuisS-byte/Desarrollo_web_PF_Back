using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public string? ServNombre { get; set; }

    public string? SerDescripcion { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
