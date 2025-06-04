namespace Desarrollo_web_PF_Back.Models.DTOs.Employees
{
    public class TicketDetailDTO
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public DateOnly? FechaCierre { get; set; }
        public EstadoDTO Estado { get; set; } = new EstadoDTO();
        public PrioridadDTO Prioridad { get; set; } = new PrioridadDTO();
        public ServicioDTO Servicio { get; set; } = new ServicioDTO();
        public UsuarioDTO UsuarioCreador { get; set; } = new UsuarioDTO();
        public UsuarioDTO? TecnicoAsignado { get; set; }
        public List<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public List<ComentarioDTO> Comentarios { get; set; } = new List<ComentarioDTO>();
        public List<AsignacionDTO> Asignaciones { get; set; } = new List<AsignacionDTO>();
        public List<CambioEstadoDTO> CambiosEstado { get; set; } = new List<CambioEstadoDTO>();
    }

    public class EstadoDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class PrioridadDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ServicioDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? NombreCompleto => $"{Nombre} {Apellido}";
    }

    public class ArchivoDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Ruta { get; set; }
        public DateOnly? FechaCreacion { get; set; }
    }

    public class ComentarioDTO
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public UsuarioDTO Usuario { get; set; } = new UsuarioDTO();
    }

    public class AsignacionDTO
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public UsuarioDTO Usuario { get; set; } = new UsuarioDTO();
    }

    public class CambioEstadoDTO
    {
        public int Id { get; set; }
        public DateOnly? FechaCambio { get; set; }
        public EstadoDTO EstadoAnterior { get; set; } = new EstadoDTO();
        public EstadoDTO EstadoNuevo { get; set; } = new EstadoDTO();
        public UsuarioDTO Usuario { get; set; } = new UsuarioDTO();
    }
}