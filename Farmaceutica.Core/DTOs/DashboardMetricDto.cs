using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class DashboardMetricDto
    {
        public string Titulo { get; set; }
        public decimal Valor { get; set; }
        public decimal PorcentajeCambio { get; set; }
        public bool EsPositivo { get; set; }
        public string ColorPorcentaje => EsPositivo ? "success" : "danger";
        public string IconoPorcentaje => EsPositivo ? "ri-arrow-right-up-line" : "ri-arrow-right-down-line";
        public string UrlDetalle { get; set; }
        public string Icono { get; set; } = "bx bx-package";
        public string ColorIcono { get; set; } = "success";
    }

    public class DashboardFullDto
    {
        public DashboardMetricDto Productos { get; set; }
        public DashboardMetricDto Ventas { get; set; }
        public DashboardMetricDto Compras { get; set; }
        public DashboardMetricDto Pedidos { get; set; }
    }
}
