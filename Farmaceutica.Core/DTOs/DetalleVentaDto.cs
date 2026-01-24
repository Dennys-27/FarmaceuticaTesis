using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Farmaceutica.Core.DTOs
{
    [Keyless]
    public class DetalleVentaDto
    {
        public int VentId { get; set; }
        public int ProdId { get; set; }
        public decimal ProdPVenta { get; set; }
        public int DetvCant { get; set; }
    }


    [Keyless]
    public class DetalleCompraDto
    {
        public int ComprId { get; set; }
        public int ProdId { get; set; }
        public decimal ProdPCompra { get; set; }
        public int DetcCant { get; set; }
    }
}
