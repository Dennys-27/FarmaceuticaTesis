using Farmaceutica.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        // Rol del usuario (usa el enum)
        public RolUsuario Rol { get; set; }  

        // 🔹 Campos opcionales para relaciones futuras
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ruc { get; set; }
        public string? web { get; set; }
        public string? Imagen { get; set; }

        public ICollection<Venta> Ventas { get; set; }
        public ICollection<TokenRecuperacion>? TokensRecuperacion { get; set; }
    }
}
