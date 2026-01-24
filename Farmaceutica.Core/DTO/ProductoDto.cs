using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTO
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int StockTotal { get; set; }
        public int StockLocal { get; set; }
        public int StockDelivery { get; set; }
        public string Codigo { get; set; }
        public string? Imagen { get; set; }

        // Información de categoría
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }

        public int SubCategoriaId { get; set; }
        public string SubCategoriaNombre { get; set; }
    }
}
