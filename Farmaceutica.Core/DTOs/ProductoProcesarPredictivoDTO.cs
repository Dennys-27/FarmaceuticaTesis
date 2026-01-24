using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class ProductoProcesarPredictivoDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int StockTotal { get; set; }
        public int StockLocal { get; set; }
        public int StockDelivery { get; set; }
        public string? Categoria { get; set; }
        public string? SubCategoria { get; set; }

        public string Codigo { get; set; }
        public string EstadoProducto { get; set; }
    }
}
