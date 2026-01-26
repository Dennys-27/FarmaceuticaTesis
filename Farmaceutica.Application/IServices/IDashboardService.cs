using Farmaceutica.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.IServices
{
    public interface IDashboardService
    {
        Task<DashboardMetricDto> GetProductosMetricAsync();
        Task<DashboardMetricDto> GetVentasMetricAsync();
        Task<DashboardMetricDto> GetComprasMetricAsync();
        Task<DashboardMetricDto> GetPedidosMetricAsync();
        Task<DashboardFullDto> GetFullDashboardAsync();
    }
}
