using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Entities
{
    public class Compra
    {
        public int Id { get; set; }
        // Solo obligatorio
        public int EncargadoId { get; set; }
        public Usuario? Encargado { get; set; }

        // Todo lo demás opcional
        public int? ProveedorId { get; set; }
        public Usuario? Proveedor { get; set; }

        public string? ProvRuc { get; set; }
        public string? ProvDireccion { get; set; }
        public string? ProvCorreo { get; set; }

        public MetodoPago? MetodoPago { get; set; } // enum nullable
        public Moneda? Moneda { get; set; } // enum nullable
        public Unidad? Unidad { get; set; } // enum nullable
        public DocumentoTipo? DocumentoTipo { get; set; } // enum nullable

        public decimal? Subtotal { get; set; }
        public decimal? Igv { get; set; }
        public decimal? Total { get; set; }

        public string? Comentario { get; set; }

        public int IsActive { get; set; }

        public DateTime FechaCreacion { get; set; }

        public ICollection<DetalleCompra>? DetalleCompras { get; set; }
    }
}
