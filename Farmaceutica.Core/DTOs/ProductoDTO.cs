using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int StockTotal { get; set; }
        public int StockLocal { get; set; }
        public int StockDelivery { get; set; }
        public int CategoriaId { get; set; }
        public string Categoria { get; set; }
        public int SubCategoriaId { get; set; }
        public string SubCategoria { get; set; }

        public string Codigo { get; set; }

        public string? Imagen { get; set; }
        public bool IsActive { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
