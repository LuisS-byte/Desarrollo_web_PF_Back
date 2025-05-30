namespace Desarrollo_web_PF_Back.Models.DTOs
{
    public class LoginRolDTO
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string NombreRol { get; set; } = "SinRol";
    }
}
