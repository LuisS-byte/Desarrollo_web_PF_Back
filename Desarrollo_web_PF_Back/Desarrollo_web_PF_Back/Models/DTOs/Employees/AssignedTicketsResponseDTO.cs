namespace Desarrollo_web_PF_Back.Models.DTOs.Employees
{
    public class AssignedTicketsResponseDTO
    {
        public MetadataDTO Metadata { get; set; } = new MetadataDTO();
        public StatisticsDTO Statistics { get; set; } = new StatisticsDTO();
        public List<AssignedTicketDTO> Tickets { get; set; } = new List<AssignedTicketDTO>();
        public UsuarioDTO usuario { get; set; }
    }

    public class AssignedTicketDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? AssignmentDescription { get; set; }
    }
}