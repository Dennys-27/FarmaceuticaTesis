using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class DashboardData
    {
        public string Periodo { get; set; } = "1Y";
        public int TotalOrdenes { get; set; } // Total de ventas web
        public int TotalVentasLocales { get; set; } // Ventas en farmacia
        public int TotalDeliveries { get; set; } // Ventas por delivery
        public decimal IngresosTotales { get; set; } // Total de todas las ventas
        public decimal EgresosTotales { get; set; } // Total de compras
        public decimal GananciaNeta { get; set; } // Ingresos - Egresos
        public List<GraficoMensual> DatosMensuales { get; set; }
    }

    // Models/Dashboard/GraficoMensual.cs
    public class GraficoMensual
    {
        public string Mes { get; set; } // Formato: "Ene-2024"
        public int VentasWeb { get; set; }
        public int VentasFarmacia { get; set; }
        public int VentasDelivery { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Egresos { get; set; }
        public decimal Ganancia { get; set; }
    }

}
