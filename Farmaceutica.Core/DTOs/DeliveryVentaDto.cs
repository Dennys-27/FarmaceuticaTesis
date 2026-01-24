using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class DeliveryVentaDto
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public string TipoVenta { get; set; } = string.Empty;
        public string? CliCorreo { get; set; }
        public string? CliDireccion { get; set; }
        public string? CliRuc { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaCompra { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string EstadoDelivery { get; set; } = string.Empty; // Nota: En tu SQL dice "AS TipoVenta" pero parece ser EstadoDelivery
        public decimal? Igv { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Total { get; set; }
        public string Encargado { get; set; } = string.Empty;
        public string? Delivery { get; set; } // Nombre del repartidor

        public string? Imagen { get; set; }
    }

    public class DeliveryVentaDeatelleDto
    {
        public string? Imagen { get; set; }
        public string Producto { get; set; } = string.Empty;

        public int Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public decimal? TotalVenta { get; set; }
    }

    public class DeliveryVentaFacturaDto
    {
        public int VentaId { get; set; }
        public int NumeroFactura { get; set; }
        public DateTime FechaVenta { get; set; }

        public string Cliente { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public string Ruc { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }

        public string MetodoPago { get; set; }
        public string Moneda { get; set; }
        public string Unidad { get; set; }

        public string Encargado { get; set; }
    }

}
