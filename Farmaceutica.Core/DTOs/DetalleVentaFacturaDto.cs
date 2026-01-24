using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class VentaVerViewModel
    {
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; }

        public string CliNom { get; set; }
        public string CliRuc { get; set; }
        public string CliDirecc { get; set; }
        public string CliCorreo { get; set; }

        public string UsuNom { get; set; }

        public string MonNom { get; set; }
        public string PagNom { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }

        public string Comentario { get; set; }

        public List<DetalleVentaFacturaDto> Detalles { get; set; }
    }
    public class DetalleVentaFacturaDto
    {
        public string Categoria { get; set; }
        public string ProductoNombre { get; set; }
        public string Unidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalVenta { get; set; }
    }


    public class CompraVerViewModel
    {
        public int CompraId { get; set; }
        public DateTime Fecha { get; set; }

        public string ProvNom { get; set; }
        public string ProvRuc { get; set; }
        public string ProvDirecc { get; set; }
        public string ProvCorreo { get; set; }

        public string UsuNom { get; set; }

        public string MonNom { get; set; }
        public string PagNom { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }

        public string Comentario { get; set; }

        public List<DetalleCompraFacturaDto> Detalles { get; set; }
    }
    public class DetalleCompraFacturaDto
    {
        public string Categoria { get; set; }
        public string ProductoNombre { get; set; }
        public string Unidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalCompra { get; set; }
    }
}
