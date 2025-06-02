namespace Desarrollo_web_PF_Back.Models.DTOs
{
    public class CrearTicketDTO
    {
        public int IdServicio { get; set; }
        public int IdPrioridad { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public IFormFile? Archivo { get; set; }

    }

}
