using System;
using System.Collections.Generic;

namespace Farmaceutica.Core.Entities
{
    public class Producto : BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int StockTotal { get; set; }
        public int StockLocal { get; set; }
        public int StockDelivery { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public int SubCategoriaId { get; set; }
        public SubCategoria SubCategoria { get; set; }

        public string Codigo { get; set; }
        public string? Imagen { get; set; }

        public ICollection<DetalleVenta> DetalleVentas { get; set; }
        public ICollection<DetalleCompra> DetalleCompras { get; set; }

        public Producto()
        {
            DetalleVentas = new HashSet<DetalleVenta>();
            DetalleCompras = new HashSet<DetalleCompra>();
        }
    }
}
