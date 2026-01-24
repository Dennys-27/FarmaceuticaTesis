using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Temporales
{
    public class ProductoTemp
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }           // nullable
        public string? Descripcion { get; set; }      // nullable
        public decimal? Precio { get; set; }          // nullable
        public int? StockLocal { get; set; }          // nullable
        public int? StockDelivery { get; set; }       // nullable
        public string? Codigo { get; set; }           // nullable
        public int? CategoriaId { get; set; }         // nullable
        public string? Categoria { get; set; }        // nullable
        public int? SubCategoriaId { get; set; }      // nullable
        public string? SubCategoria { get; set; }     // nullable
        public bool? IsActive { get; set; }           // nullable si SQL permite null
        public DateTime? FechaCreacion { get; set; }  // nullable
    }

}
