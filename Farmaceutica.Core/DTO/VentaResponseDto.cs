using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTO
{
    public class VentaResponseDto
    {
        public int Id { get; set; }
        public int NumeroFactura { get; set; }
        public string Encargado { get; set; }
        public string MetodoPago { get; set; }
        public string Moneda { get; set; }
        public string DocumentoTipo { get; set; }
        public string TipoVenta { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteRuc { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaVenta { get; set; }
        public List<DetalleVentaResponseDto> Detalles { get; set; } = new();
    }

    public class DetalleVentaResponseDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCodigo { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
    }

}
