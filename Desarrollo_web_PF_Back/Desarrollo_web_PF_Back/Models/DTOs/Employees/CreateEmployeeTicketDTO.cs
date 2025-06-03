namespace Desarrollo_web_PF_Back.Models.DTOs.Employees
{
    public class CreateEmployeeTicketDTO
    {
        public int UserId { get; set; }
        public int IdServicio { get; set; }
        public int IdPrioridad { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public IFormFile? Archivo { get; set; }
    }

    public class CreateTicketResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
    }
}