using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class VentaDto
    {
        [Column("NUMERO DE FACTURA")]
        public int? NumeroFactura { get; set; }

        [Column("ENCARGADO")]
        public string? Encargado { get; set; }

        [Column("METODO PAGO")]
        public string? MetodoPago { get; set; }

        [Column("MONEDA")]
        public string? Moneda { get; set; }

        [Column("UNIDAD")]
        public string? Unidad { get; set; }

        [Column("CLIENTE")]
        public string? Cliente { get; set; }

        [Column("RUC")]
        public string? Ruc { get; set; }

        [Column("CORREO")]
        public string? Correo { get; set; }

        [Column("DIRECCION")]
        public string? Direccion { get; set; }

        [Column("IGV")]
        public decimal? Igv { get; set; }

        [Column("SUBTOTAL")]
        public decimal? Subtotal { get; set; }

        [Column("TOTAL")]
        public decimal? Total { get; set; }

        [Column("ESTADO")]
        public int? Estado { get; set; }

        [Column("COMENTARIO")]
        public string? Comentario { get; set; }

        [Column("FECHA VENTA")]
        public DateTime? FechaVenta { get; set; }
    }

    public class CompraDto
    {
        [Column("NUMERO DE FACTURA")]
        public int? NumeroFactura { get; set; }

        [Column("ENCARGADO")]
        public string? Encargado { get; set; }

        [Column("METODO PAGO")]
        public string? MetodoPago { get; set; }

        [Column("MONEDA")]
        public string? Moneda { get; set; }

        [Column("UNIDAD")]
        public string? Unidad { get; set; }

        [Column("PROVEEDOR")]
        public string? Proveedor { get; set; }

        [Column("RUC")]
        public string? Ruc { get; set; }

        [Column("CORREO")]
        public string? Correo { get; set; }

        [Column("DIRECCION")]
        public string? Direccion { get; set; }

        [Column("IGV")]
        public decimal? Igv { get; set; }

        [Column("SUBTOTAL")]
        public decimal? Subtotal { get; set; }

        [Column("TOTAL")]
        public decimal? Total { get; set; }

        [Column("ESTADO")]
        public int? Estado { get; set; }

        [Column("COMENTARIO")]
        public string? Comentario { get; set; }

        [Column("FECHA COMPRA")]
        public DateTime? FechaCompra { get; set; }
    }

}
