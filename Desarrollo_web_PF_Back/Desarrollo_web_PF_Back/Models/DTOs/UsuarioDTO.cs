﻿namespace Desarrollo_web_PF_Back.Models.DTOs
{
    public class UsuarioDTO
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public int? IdRol { get; set; }
        public int? interno { get; set; }
        }
}
