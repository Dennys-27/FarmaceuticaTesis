using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;
        private readonly AppFarmaceuticaContex _context;
        public DashboardRepository(IConfiguration config, AppFarmaceuticaContex context)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _context = context;
        }

       

        public DashboardResumenDto ObtenerResumen()
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_dashboard_resumen", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (!dr.Read()) return new DashboardResumenDto();

            return new DashboardResumenDto
            {
                TotalProductos = dr.GetInt32(0),
                TotalVentas = dr.IsDBNull(1) ? 0 : dr.GetDecimal(1),
                PorcentajeCrecimiento = dr.IsDBNull(2) ? 0 : dr.GetDecimal(2)
            };
        }

        public async Task<List<DashboardVentaDTO>> ObtenerVentasRecientes()
        {
            var paramOperacion = new SqlParameter("@Operacion", "VENTAS");
            return await _context.Database
                .SqlQueryRaw<DashboardVentaDTO>(
                    "[fersoftw_Farmaceutica].[SP_DASHBOARD] @Operacion = @Operacion",
                    paramOperacion)
                .ToListAsync();
        }

        public async Task<List<DashboardPedidoDTO>> ObtenerPedidosPendientes()
        {
            var paramOperacion = new SqlParameter("@Operacion", "PEDIDOS");
            return await _context.Database
                .SqlQueryRaw<DashboardPedidoDTO>(
                    "EXEC [fersoftw_Farmaceutica].[SP_DASHBOARD] @Operacion = @Operacion",
                    paramOperacion)
                .ToListAsync();
        }

        public async Task<List<DashboardProductoDTO>> ObtenerProductosMasVendidos()
        {
            var paramOperacion = new SqlParameter("@Operacion", "PRODUCTOS");
            return await _context.Database
                .SqlQueryRaw<DashboardProductoDTO>(
                    "EXEC [fersoftw_Farmaceutica].[SP_DASHBOARD] @Operacion = @Operacion",
                    paramOperacion)
                .ToListAsync();
        }
        public async Task<DashboardData> ObtenerDashboardDataAsync(string periodo)
        {
            var fechaFiltro = CalcularFechaFiltro(periodo);

            // Obtener ventas activas
            var ventasQuery = _context.Ventas.Where(v => v.IsActive == 1);

            if (fechaFiltro.HasValue)
            {
                ventasQuery = ventasQuery.Where(v => v.FechaCreacion >= fechaFiltro.Value);
            }

            var ventas = await ventasQuery.ToListAsync();

            // Obtener compras activas
            var comprasQuery = _context.Compras.Where(c => c.IsActive == 1);

            if (fechaFiltro.HasValue)
            {
                comprasQuery = comprasQuery.Where(c => c.FechaCreacion >= fechaFiltro.Value);
            }

            var compras = await comprasQuery.ToListAsync();

            // Calcular métricas
            // Usando conversión explícita
            var totalVentasWeb = ventas.Count(v => (int?)v.TipoVenta == (int)TipoVenta.Web);

            // O usando HasValue
            var totalVentasFarmacia = ventas.Count(v =>
                (int?)v.TipoVenta  == (int)TipoVenta.Farmacia);

            var totalDeliveries = ventas.Count(v =>
                (int?)v.TipoVenta == (int)TipoVenta.Delivery);

            var ingresosTotales = ventas.Sum(v => v.Total);
            var egresosTotales = compras.Sum(c => c.Total);
            var gananciaNeta = ingresosTotales - egresosTotales;

            // Obtener datos mensuales para el gráfico
            var datosMensuales = await ObtenerDatosMensualesAsync(periodo);

            return new DashboardData
            {
                Periodo = periodo,
                TotalOrdenes = totalVentasWeb,
                TotalVentasLocales = totalVentasFarmacia,
                TotalDeliveries = totalDeliveries,
                IngresosTotales = (decimal)ingresosTotales,
                EgresosTotales = (decimal)egresosTotales,
                GananciaNeta = (decimal)gananciaNeta,
                DatosMensuales = datosMensuales
            };
        }

        public async Task<List<GraficoMensual>> ObtenerDatosMensualesAsync(string periodo)
        {
            var fechaFiltro = CalcularFechaFiltro(periodo);
            var resultado = new List<GraficoMensual>();

            // Obtener ventas agrupadas por mes
            var ventasQuery = _context.Ventas
                .Where(v => v.IsActive == 1)
                .AsQueryable();

            if (fechaFiltro.HasValue)
            {
                ventasQuery = ventasQuery.Where(v => v.FechaCreacion >= fechaFiltro.Value);
            }

            var ventasPorMes = await ventasQuery
                .GroupBy(v => new { v.FechaCreacion.Year, v.FechaCreacion.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    // Reemplaza las comparaciones de 'TipoVenta?' con 'int' por comparaciones seguras usando .HasValue y el valor del enum

                    // Dentro del método ObtenerDatosMensualesAsync, reemplaza las siguientes líneas:

                    VentasWeb = g.Count(v => v.TipoVenta.HasValue && v.TipoVenta.Value == TipoVenta.Web),
                    VentasFarmacia = g.Count(v => (v.TipoVenta.HasValue && v.TipoVenta.Value == TipoVenta.Farmacia) || !v.TipoVenta.HasValue),
                    VentasDelivery = g.Count(v => v.TipoVenta.HasValue && v.TipoVenta.Value == TipoVenta.Delivery),
                  
                    Ingresos = g.Sum(v => v.Total)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // Obtener compras agrupadas por mes
            var comprasQuery = _context.Compras
                .Where(c => c.IsActive == 1)
                .AsQueryable();

            if (fechaFiltro.HasValue)
            {
                comprasQuery = comprasQuery.Where(c => c.FechaCreacion >= fechaFiltro.Value);
            }

            var comprasPorMes = await comprasQuery
                .GroupBy(c => new { c.FechaCreacion.Year, c.FechaCreacion.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Egresos = g.Sum(c => c.Total)
                })
                .ToListAsync();

            // Combinar datos
            foreach (var ventaMes in ventasPorMes)
            {
                var compraMes = comprasPorMes
                    .FirstOrDefault(c => c.Year == ventaMes.Year && c.Month == ventaMes.Month);

                var mes = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(ventaMes.Month);

                resultado.Add(new GraficoMensual
                {
                    Mes = $"{mes}-{ventaMes.Year}",
                    VentasWeb = ventaMes.VentasWeb,
                    VentasFarmacia = ventaMes.VentasFarmacia,
                    VentasDelivery = ventaMes.VentasDelivery,
                    Ingresos = (decimal)ventaMes.Ingresos,
                    Egresos = compraMes?.Egresos ?? 0,
                    Ganancia = (decimal)(ventaMes.Ingresos - (compraMes?.Egresos ?? 0))
                });
            }

            return resultado;
        }

        public async Task<Dictionary<string, decimal>> ObtenerMetricasRapidasAsync()
        {
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);

            // Ventas del mes actual
            var ventasMes = await _context.Ventas
                .Where(v => v.IsActive == 1 && v.FechaCreacion >= primerDiaMes)
                .ToListAsync();

            var comprasMes = await _context.Compras
                .Where(c => c.IsActive == 1 && c.FechaCreacion >= primerDiaMes)
                .ToListAsync();

            return new Dictionary<string, decimal>
            {
                { "VentasHoy", ventasMes.Where(v => v.FechaCreacion.Date == hoy).Sum(v => v.Total) ?? 0 },
                { "VentasMes", ventasMes.Sum(v => v.Total) ?? 0 },
                { "ComprasMes", comprasMes.Sum(c => c.Total) ?? 0 },
                { "GananciaMes", (ventasMes.Sum(v => v.Total) ?? 0) - (comprasMes.Sum(c => c.Total) ?? 0) }
            };
        }

        private DateTime? CalcularFechaFiltro(string periodo)
        {
            var hoy = DateTime.Today;

            return periodo switch
            {
                "1M" => hoy.AddMonths(-1),
                "6M" => hoy.AddMonths(-6),
                "1Y" => hoy.AddYears(-1),
                _ => null // ALL
            };
        }
    }
}
