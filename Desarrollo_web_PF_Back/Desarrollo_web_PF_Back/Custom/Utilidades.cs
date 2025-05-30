using Desarrollo_web_PF_Back.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Desarrollo_web_PF_Back.Custom
{
    public class Utilidades
    {
        private readonly IConfiguration _configuration;
        public Utilidades(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string encriptarSHA256(string clave)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(clave));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GenerarToken(Usuario usuario)
        {
            // Crear información del usuario para el token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.UsuCorreo!),
                new Claim(ClaimTypes.Role, usuario.Rol.RolNombre!)
            };

            // Obtener la clave secreta desde la configuración
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            // Especificar las credenciales de firma
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            // Configuración de los detalles del token, incluyendo Issuer y Audience
            var jwtConfig = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],  // Definido en appsettings.json
                audience: _configuration["Jwt:Audience"],  // Definido en appsettings.json
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            // Retornar el token como un string
            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
