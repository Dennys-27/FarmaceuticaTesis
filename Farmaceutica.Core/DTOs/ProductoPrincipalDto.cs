using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Farmaceutica.Core.DTOs
    {
        public class ProductoPrincipalDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Categoria { get; set; } = string.Empty;
            public string SubCategoria { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public string Imagen { get; set; } = string.Empty;
            public int StockTotal { get; set; }
            public decimal Precio { get; set; }
        }
    }
    // En Farmaceutica.Core.DTOs o donde tengas tus DTOs
    public class ProductoPrincipalResult
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string SubCategoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public int StockTotal { get; set; }
        public decimal Precio { get; set; }
    }
}
