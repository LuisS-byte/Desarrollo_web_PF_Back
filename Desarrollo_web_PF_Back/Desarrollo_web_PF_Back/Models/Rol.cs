using System;
using System.Collections.Generic;

namespace Desarrollo_web_PF_Back.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string? RolNombre { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
