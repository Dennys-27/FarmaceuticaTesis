using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public enum UnidadEnum
    {
        Caja = 1,
        Litro = 2,
        Unidad = 3
    }

    public class DetalleVentaListarDto
    {
        public int Id { get; set; }
        public string Categoria { get; set; }
        public string Producto { get; set; }
        public UnidadEnum? Unidad { get; set; } // enum nullable
        public decimal? Precio { get; set; }    // decimal nullable
        public int? Cantidad { get; set; }      // int nullable
        public decimal? TotalVenta { get; set; } // decimal nullable
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
    }

    public class DetalleCompraListarDto
    {
        public int Id { get; set; }
        public string Categoria { get; set; }
        public string Producto { get; set; }
        public UnidadEnum? Unidad { get; set; } // enum nullable
        public decimal? Precio { get; set; }    // decimal nullable
        public int? Cantidad { get; set; }      // int nullable
        public decimal? TotalCompra { get; set; } // decimal nullable
        public int CompraId { get; set; }
        public int ProductoId { get; set; }
    }


}

