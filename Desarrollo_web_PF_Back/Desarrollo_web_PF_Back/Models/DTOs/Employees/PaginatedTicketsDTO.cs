namespace Desarrollo_web_PF_Back.Models.DTOs.Employees
{
    public class PaginatedTicketsDTO
    {
        public List<TicketEmployeeDTO> Tickets { get; set; } = new List<TicketEmployeeDTO>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }

    public class TicketEmployeeDTO
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public DateOnly? FechaCierre { get; set; }
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }
        public string? Servicio { get; set; }
        public string? UsuarioCreador { get; set; }
        public string? TecnicoAsignado { get; set; }
        public bool TieneArchivos { get; set; }
        public int CantidadComentarios { get; set; }
    }
}