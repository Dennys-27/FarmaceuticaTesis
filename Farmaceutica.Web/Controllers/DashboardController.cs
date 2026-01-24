using Farmaceutica.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ObtenerDashboardResumenUseCase _useCase;

        // ✅ INYECCIÓN CORRECTA
        public DashboardController(ObtenerDashboardResumenUseCase useCase)
        {
            _useCase = useCase;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ✅ RUTA MVC NORMAL
        [HttpPost]
        public JsonResult Resumen()
        {
            var data = _useCase.Ejecutar();
            return Json(data);
        }
    }
}
