namespace Desarrollo_web_PF_Back.Models.DTOs
{
    public class ComentarioDTO
    {
        public int? IdTicket { get; set; }

        public int? IdUsuario { get; set; }
        public string Comentario { get; set; }
    }
}
