using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Farmaceutica.Web.Controllers
{
    public class AnalisisEmpresaController : Controller
    {
        private readonly IDashboardRepository _analisisService;
        private readonly ILogger<AnalisisEmpresaController> _logger;

        public AnalisisEmpresaController(IDashboardRepository analisisService, ILogger<AnalisisEmpresaController> logger)
        {
            _analisisService = analisisService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string periodo = "1Y")
        {
            try
            {
                var data = await _analisisService.ObtenerDashboardDataAsync(periodo);

                // Para pasar datos a la vista, usa ViewBag o ViewData
                ViewBag.PeriodoActual = periodo;
                ViewBag.Periodos = new[] { "ALL", "1M", "6M", "1Y" };
                ViewBag.DatosMensualesJson = System.Text.Json.JsonSerializer.Serialize(data.DatosMensuales);

                // Retorna la vista con el modelo
                return View(data);  // Esto busca Index.cshtml en Views/Dashboard
            }
            catch (Exception ex)
            {
                // Manejo de errores para MVC
                TempData["Error"] = $"Error al cargar datos: {ex.Message}";
                return View(new DashboardData());  // Vista con modelo vacío
            }
        }

        public async Task<IActionResult> ObtenerDashboard(string periodo = "1Y")
        {
            try
            {
                var data = await _analisisService.ObtenerDashboardDataAsync(periodo);

                // Para AJAX, puedes retornar JSON o PartialView
                // Si quieres actualizar solo una parte de la página:
                return PartialView("_DashboardStats", data);

                // Si quieres retornar JSON para procesar en JavaScript:
                // return Json(new { 
                //     success = true, 
                //     data = data,
                //     datosGrafico = data.DatosMensuales 
                // });
            }
            catch (Exception ex)
            {
                return PartialView("_ErrorPartial", ex.Message);
                // O para JSON:
                // return Json(new { success = false, error = ex.Message });
            }
        }

        // Método para obtener solo datos del gráfico (para AJAX)
        public async Task<IActionResult> ObtenerDatosGrafico(string periodo = "1Y")
        {
            try
            {
                var data = await _analisisService.ObtenerDatosMensualesAsync(periodo);
                return Json(data);  // Retorna JSON para JavaScript
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Método para obtener métricas rápidas
        public async Task<IActionResult> ObtenerMetricasRapidas()
        {
            try
            {
                var data = await _analisisService.ObtenerMetricasRapidasAsync();
                return PartialView("_MetricasRapidas", data);  // O Json(data)
            }
            catch (Exception ex)
            {
                return PartialView("_ErrorPartial", ex.Message);
            }
        }
    }
}
