using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class RepartidorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
    }
    public class ResultadoProcesarDelivery
    {
        public string Resultado { get; set; } = string.Empty;
        public int VentaId { get; set; }
    }
}
