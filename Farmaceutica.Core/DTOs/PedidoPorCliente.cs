using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class PedidoPorCliente
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } // Cambia a nullable
        public string? Apellido { get; set; } // Cambia a nullable
        public string? MetodoPago { get; set; } // Cambia a nullable
        public string? CliCorreo { get; set; }
        public string? CliDireccion { get; set; }
        public string? CliRuc { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaCompra { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? EstadoDelivery { get; set; } // Cambia a nullable
        public decimal? Igv { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Total { get; set; }
        public string? PrimerNombre { get; set; } // Cambia a nullable
        public string? SegundoNombre { get; set; }
        public string? Imagen { get; set; }
    }
}