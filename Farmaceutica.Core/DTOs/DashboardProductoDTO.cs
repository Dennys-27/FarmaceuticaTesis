using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class DashboardProductoDTO
    {
        public string? Imagen { get; set; }
        public decimal Precio { get; set; }
        public int StockTotal { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int VecesVendido { get; set; }
        public string FechaCreacion { get; set; } = string.Empty;
    }

    // DTO para operación VENTAS
    public class DashboardVentaDTO
    {
        public string? Imagen { get; set; }
        public string Producto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }

    // DTO para operación PEDIDOS
    public class DashboardPedidoDTO
    {
        public string? Imagen { get; set; }
        public string Producto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public int EstadoDelivery { get; set; }
    
        // Usar string pero convertir en BD
        public string? NUMERO { get; set; }
        public string? Telefono { get; set; }
    
        public string? Delivery { get; set; }
        public string? Direccion { get; set; }
    }
}
