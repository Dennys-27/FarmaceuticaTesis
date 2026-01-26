using Farmaceutica.Application.IServices;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IProductoRepository productoRepository, ILogger<DashboardService> logger = null)
        {
            _productoRepository = productoRepository;
            _logger = logger;
        }

        public async Task<DashboardMetricDto> GetProductosMetricAsync()
        {
            try
            {
                // 1. Obtener total productos activos (tu query SQL)
                var totalProductos = await _productoRepository.CountActiveAsync();

                // 2. Obtener total stock (tu query SQL)
                var totalStock = await _productoRepository.SumStockActiveAsync();

                // 3. Calcular porcentaje de cambio
                var fechaActual = DateTime.Now;
                var inicioMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                var inicioMesPasado = inicioMesActual.AddMonths(-1);

                var productosMesActual = totalProductos;
                var productosMesPasado = await _productoRepository.CountCreatedInPeriodAsync(inicioMesPasado, inicioMesActual);

                // Calcular porcentaje
                var porcentajeCambio = productosMesPasado > 0
                    ? Math.Round(((productosMesActual - productosMesPasado) / (decimal)productosMesPasado) * 100, 2)
                    : productosMesActual > 0 ? 100 : 0;

                return new DashboardMetricDto
                {
                    Titulo = "Total Productos",
                    Valor = totalStock / 1000m, // Convertir a miles (k)
                    PorcentajeCambio = porcentajeCambio,
                    EsPositivo = porcentajeCambio >= 0,
                    UrlDetalle = null
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error en GetProductosMetricAsync");
                throw new ApplicationException("Error al obtener métricas de productos", ex);
            }
        }

        public async Task<DashboardMetricDto> GetVentasMetricAsync()
        {
            try
            {
                var fechaActual = DateTime.Now;
                var inicioMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                var inicioMesPasado = inicioMesActual.AddMonths(-1);
                var finMesActual = inicioMesActual.AddMonths(1);

                // Ventas del mes actual
                var ventasActual = await _productoRepository.GetTotalVentasAsync(inicioMesActual, finMesActual);

                // Ventas del mes pasado
                var ventasPasado = await _productoRepository.GetTotalVentasAsync(inicioMesPasado, inicioMesActual);

                // Calcular porcentaje
                var porcentajeCambio = ventasPasado > 0
                    ? Math.Round(((ventasActual - ventasPasado) / ventasPasado) * 100, 2)
                    : ventasActual > 0 ? 100 : 0;

                return new DashboardMetricDto
                {
                    Titulo = "Ventas",
                    Valor = ventasActual / 1000m, // En miles
                    PorcentajeCambio = porcentajeCambio,
                    EsPositivo = porcentajeCambio >= 0,
                    UrlDetalle = null,
                    Icono = "bx bx-dollar-circle",
                    ColorIcono = "success"
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error en GetVentasMetricAsync");
                throw;
            }
        }

        public async Task<DashboardMetricDto> GetComprasMetricAsync()
        {
            try
            {
                var fechaActual = DateTime.Now;
                var inicioMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                var inicioMesPasado = inicioMesActual.AddMonths(-1);
                var finMesActual = inicioMesActual.AddMonths(1);

                var comprasActual = await _productoRepository.GetTotalComprasAsync(inicioMesActual, finMesActual);
                var comprasPasado = await _productoRepository.GetTotalComprasAsync(inicioMesPasado, inicioMesActual);

                var porcentajeCambio = comprasPasado > 0
                    ? Math.Round(((comprasActual - comprasPasado) / comprasPasado) * 100, 2)
                    : comprasActual > 0 ? 100 : 0;

                return new DashboardMetricDto
                {
                    Titulo = "Compras",
                    Valor = comprasActual,
                    PorcentajeCambio = porcentajeCambio,
                    EsPositivo = porcentajeCambio >= 0,
                    UrlDetalle = null,
                    Icono = "bx bx-shopping-bag",
                    ColorIcono = "info"
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error en GetComprasMetricAsync");
                throw;
            }
        }

        public async Task<DashboardMetricDto> GetPedidosMetricAsync()
        {
            try
            {
                var pedidosPendientes = await _productoRepository.CountPedidosPendientesAsync();
                var totalPedidos = await _productoRepository.GetTotalPedidosPendientesAsync();

                // Calcular cambio con mes anterior
                var fechaActual = DateTime.Now;
                var inicioMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                var inicioMesPasado = inicioMesActual.AddMonths(-1);

                // Aquí necesitarías un método para contar pedidos por período
                // Por ahora usaremos un valor fijo para el ejemplo
                var porcentajeCambio = 0m; // Implementar lógica real

                return new DashboardMetricDto
                {
                    Titulo = "Pedidos Pendientes",
                    Valor = totalPedidos / 1000m, // En miles
                    PorcentajeCambio = porcentajeCambio,
                    EsPositivo = porcentajeCambio >= 0,
                    UrlDetalle = null,
                    Icono = "bx bx-cart",
                    ColorIcono = "warning"
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error en GetPedidosMetricAsync");
                throw;
            }
        }

        public async Task<DashboardFullDto> GetFullDashboardAsync()
        {
            try
            {
                // ✅ EJECUCIÓN SECUENCIAL - UNA POR UNA
                var productos = await GetProductosMetricAsync();
                var ventas = await GetVentasMetricAsync();
                var compras = await GetComprasMetricAsync();
                var pedidos = await GetPedidosMetricAsync();

                return new DashboardFullDto
                {
                    Productos = productos,
                    Ventas = ventas,
                    Compras = compras,
                    Pedidos = pedidos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetFullDashboardAsync");

                // En caso de error, retorna datos por defecto
                return new DashboardFullDto
                {
                    Productos = new DashboardMetricDto
                    {
                        Titulo = "Total Productos",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = null
                    },
                    Ventas = new DashboardMetricDto
                    {
                        Titulo = "Ventas",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = null
                    },
                    Compras = new DashboardMetricDto
                    {
                        Titulo = "Compras",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = null
                    },
                    Pedidos = new DashboardMetricDto
                    {
                        Titulo = "Pedidos",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = null
                    }
                };
            }
        }
    }
}
