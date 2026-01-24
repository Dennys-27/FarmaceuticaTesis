using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTO
{
    public class VentaCrearDto
    {
        [Required]
        public int UsuarioId { get; set; } // ID del vendedor

        [Required]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        public Moneda Moneda { get; set; } = Moneda.Dolar;

        [Required]
        public Unidad Unidad { get; set; } = Unidad.Unidad;

        [Required]
        public DocumentoTipo DocumentoTipo { get; set; } = DocumentoTipo.Boleta;

        [Required]
        public TipoVenta TipoVenta { get; set; } = TipoVenta.Farmacia;

        // Campos opcionales del cliente
        public string? ClienteNombre { get; set; }
        public string? ClienteRuc { get; set; }
        public string? ClienteCorreo { get; set; }
        public string? ClienteDireccion { get; set; }

        // Para delivery
        public string? DireccionEntrega { get; set; }
        public string? Comentario { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }

    public class DetalleVentaDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }
    }

}
