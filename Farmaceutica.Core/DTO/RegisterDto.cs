using Farmaceutica.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTO
{
    public class RegisterDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;


        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        public string Ruc { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; } = RolUsuario.Cliente; // Valor por defecto
    }
}
