using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class DashboardResumenDto
    {
        public int TotalProductos { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal PorcentajeCrecimiento { get; set; }
    }
}
