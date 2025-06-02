namespace Desarrollo_web_PF_Back.Models.DTOs.Employees
{
    public class UserTicketsResponseDTO
    {
        public MetadataDTO Metadata { get; set; } = new MetadataDTO();
        public StatisticsDTO Statistics { get; set; } = new StatisticsDTO();
        public List<UserTicketDTO> Tickets { get; set; } = new List<UserTicketDTO>();
    }

    public class MetadataDTO
    {
        public int TotalTickets { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class StatisticsDTO
    {
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int Total { get; set; }
    }

    public class UserTicketDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
}