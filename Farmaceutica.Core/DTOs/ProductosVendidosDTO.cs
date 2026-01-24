using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    [Keyless]
    public class ProductosVendidosDTO
    {
        [Column("CATEGORIA")]
        public string Categoria { get; set; } = null!;
        [Column("SUBCATEGORIA")]
        public string Subcategoria { get; set; } = null!;
        [Column("PRODUCTO")]
        public string Producto { get; set; } = null!;
        [Column("STOCK LOCAL")]
        public int? StockLocal { get; set; }
        [Column("CANTIDAD VENDIDA")]
        public int? CantidadVendida { get; set; }
        [Column("PORCENTAJE VENDIDO")]
        public decimal? PorcentajeVendido { get; set; }
    }


    [Keyless]
    public class ProductosCompradosDTO
    {
        [Column("CATEGORIA")]
        public string Categoria { get; set; } = null!;
        [Column("SUBCATEGORIA")]
        public string Subcategoria { get; set; } = null!;
        [Column("PRODUCTO")]
        public string Producto { get; set; } = null!;
        [Column("STOCK LOCAL")]
        public int? StockLocal { get; set; }
        [Column("CANTIDAD COMPRADO")]
        public int? CantidadComprado { get; set; }
        [Column("PORCENTAJE COMPRADO")]
        public decimal? PorcentajeComprado { get; set; }
    }
}
