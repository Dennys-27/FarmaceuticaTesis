using Farmaceutica.Application.IServices;
using Farmaceutica.Application.Services;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ObtenerDashboardResumenUseCase _useCase;
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IDashboardRepository _repository;
        // ✅ INYECCIÓN CORRECTA
        public DashboardController(ObtenerDashboardResumenUseCase useCase, IDashboardService dashboardService, ILogger<DashboardController> logger, IDashboardRepository dashboardRepository)
        {
            _useCase = useCase;
            _dashboardService = dashboardService;
            _logger = logger;
            _repository = dashboardRepository;
        }

        // ✅ ACCIÓN PRINCIPAL MVC - PASA DashboardFullDto
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = await _dashboardService.GetFullDashboardAsync();

                // NUEVO: Obtener datos de análisis de empresa
                try
                {
                    // Obtener el servicio de análisis
                    var analisisService = HttpContext.RequestServices.GetService<IDashboardRepository>();
                    if (analisisService != null)
                    {
                        var analisisData = await analisisService.ObtenerDashboardDataAsync("1Y");

                        // Pasar al ViewBag para usar en la vista
                        ViewBag.AnalisisEmpresa = analisisData;
                        ViewBag.PeriodoActual = "1Y";
                        ViewBag.Periodos = new[] { "ALL", "1M", "6M", "1Y" };

                        // Convertir datos mensuales a JSON para JavaScript
                        if (analisisData.DatosMensuales != null)
                        {
                            ViewBag.DatosMensualesJson = System.Text.Json.JsonSerializer.Serialize(
                                analisisData.DatosMensuales,
                                new System.Text.Json.JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                                }
                            );
                        }
                    }
                }
                catch (Exception analisisEx)
                {
                    _logger.LogWarning(analisisEx, "No se pudieron cargar datos de análisis de empresa");
                    // No fallar el dashboard principal por esto
                }

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard");

                // Dashboard por defecto en caso de error
                return View(new DashboardFullDto
                {
                    Productos = new DashboardMetricDto
                    {
                        Titulo = "Total Productos",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = "/Productos"
                    },
                    Ventas = new DashboardMetricDto
                    {
                        Titulo = "Ventas",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = "/Ventas"
                    },
                    Compras = new DashboardMetricDto
                    {
                        Titulo = "Compras",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = "/Compras"
                    },
                    Pedidos = new DashboardMetricDto
                    {
                        Titulo = "Pedidos Pendientes",
                        Valor = 0,
                        PorcentajeCambio = 0,
                        EsPositivo = false,
                        UrlDetalle = "/Pedidos"
                    }
                });
            }
        }

        // ✅ ENDPOINT PARA ACTUALIZACIÓN AJAX
        [HttpGet]
        public async Task<JsonResult> GetMetricasActualizadas()
        {
            try
            {
                var data = await _dashboardService.GetFullDashboardAsync();
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetMetricasActualizadas");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ListarProducto()
        {
            var datos = await _repository.ObtenerProductosMasVendidos();

            var data = datos.Select(u => new object[]
                {
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "producto.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",
                    u.NombreProducto,
                    u.Precio,
                    u.StockTotal,
                    u.VecesVendido,
                    u.FechaCreacion
                }).ToList();

            return Json(new
            {
                draw = 1,                // requerido por DataTable
                recordsTotal = data.Count,
                recordsFiltered = data.Count,
                data = data               // clave correcta para DataTable moderno
            });
        }

        [HttpPost]
        public async Task<IActionResult> ListarVentas()
        {
            var datos = await _repository.ObtenerVentasRecientes();

            var data = datos.Select(u => new object[]
                {
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "producto.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",
                    u.Producto,
                    u.Categoria,
                    u.Cliente,
                    u.Total,
                    u.Precio,
                    u.Cantidad
                }).ToList();

            return Json(new
            {
                draw = 1,                // requerido por DataTable
                recordsTotal = data.Count,
                recordsFiltered = data.Count,
                data = data               // clave correcta para DataTable moderno
            });
        }

        [HttpPost]
        public async Task<IActionResult> ListarPedidos()
        {
            var datos = await _repository.ObtenerPedidosPendientes();

            var data = datos.Select(u => new object[]
            {
        // Imagen
        $@"
        <div class='d-flex align-items-center'>
            <div class='flex-shrink-0 me-2'>
                <img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "producto.png" : u.Imagen)}'
                     class='avatar-xs rounded-circle' />
            </div>
        </div>",

        // Datos del pedido
        u.Producto,
        u.Categoria,
        u.Cliente,
        u.Direccion,
        u.NUMERO,
        u.Total,
        u.Precio,
        u.Cantidad,
        u.EstadoDelivery,
        u.Telefono,
        u.Delivery
            }).ToList();

            return Json(new
            {
                draw = 1,                    // requerido por DataTables
                recordsTotal = data.Count,
                recordsFiltered = data.Count,
                data
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAnalisisPartial(string periodo = "1Y")
        {
            try
            {
                if (_repository == null)
                {
                    return Content("<div class='alert alert-warning'>Servicio no disponible</div>");
                }

                // USA EL MÉTODO CORRECTO
                var data = await _repository.ObtenerDashboardDataAsync(periodo);
                ViewBag.PeriodoActual = periodo;

                return PartialView("_AnalisisEmpresaPartial", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerAnalisisPartial");
                return Content($"<div class='alert alert-danger'>Error: {ex.Message}</div>");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosGrafico(string periodo = "1Y")
        {
            try
            {
                if (_repository == null)
                {
                    return Json(new { error = "Servicio no disponible" });
                }

                // USA EL MÉTODO CORRECTO
                var data = await _repository.ObtenerDatosMensualesAsync(periodo);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ObtenerDatosGrafico");
                return Json(new { error = ex.Message });
            }
        }
    }
}
