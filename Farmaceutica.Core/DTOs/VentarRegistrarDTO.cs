using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class VentarRegistrarDTO
    {
        public int Id { get; set; }
        public int EncargadoId { get; set; }
        public bool IsActive { get; set; }
    }
}
