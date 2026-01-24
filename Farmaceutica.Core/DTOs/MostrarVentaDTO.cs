using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class MostrarVentaDTO
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

        [Column("PRODUCTO")]
        public string? Producto { get; set; }

        [Column("CODIGO PRODUCTO")]
        public string? CodigoProducto { get; set; }

        [Column("CANTIDAD")]
        public int? Cantidad { get; set; }

        [Column("PRECIO UNITARIA")]
        public decimal? PrecioUnitaria { get; set; }

        [Column("TOTAL")]
        public decimal? Total { get; set; }

        [Column("FECHA VENTA")]
        public DateTime? FechaVenta { get; set; }
    }


    public class MostrarCompraDTO
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

        [Column("PRODUCTO")]
        public string? Producto { get; set; }

        [Column("CODIGO PRODUCTO")]
        public string? CodigoProducto { get; set; }

        [Column("CANTIDAD")]
        public int? Cantidad { get; set; }

        [Column("PRECIO UNITARIA")]
        public decimal? PrecioUnitaria { get; set; }

        [Column("TOTAL")]
        public decimal? Total { get; set; }

        [Column("FECHA COMPRA")]
        public DateTime? FechaCompra { get; set; }
    }
}
