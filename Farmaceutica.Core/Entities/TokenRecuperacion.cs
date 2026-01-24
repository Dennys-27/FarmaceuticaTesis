using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Entities
{
    public class TokenRecuperacion
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
