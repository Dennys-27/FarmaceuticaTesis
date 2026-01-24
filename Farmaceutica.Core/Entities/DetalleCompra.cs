using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Entities
{
    public class DetalleCompra : BaseEntity
    {
        public int CompraId { get; set; }
        public Compra Compra { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal TotalCompra { get; set; }
    }
}
