using Farmaceutica.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IDashboardRepository
    {
        DashboardResumenDto ObtenerResumen();

        Task<List<DashboardProductoDTO>> ObtenerProductosMasVendidos();

        Task<List<DashboardVentaDTO>> ObtenerVentasRecientes();

        Task<List<DashboardPedidoDTO>> ObtenerPedidosPendientes();

        Task<DashboardData> ObtenerDashboardDataAsync(string periodo);
        Task<List<GraficoMensual>> ObtenerDatosMensualesAsync(string periodo);
        Task<Dictionary<string, decimal>> ObtenerMetricasRapidasAsync();
    }
}
