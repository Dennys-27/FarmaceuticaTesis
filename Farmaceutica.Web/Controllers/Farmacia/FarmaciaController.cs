using Farmaceutica.Core.Interfaces;
using Farmaceutica.Web.Controllers.Mantenimentos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Farmacia
{
    public class FarmaciaController : Controller
    {
        private readonly ILogger<FarmaciaController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IFarmaciaRepository _productoRepository;

        public FarmaciaController(ILogger<FarmaciaController> logger, IConfiguration configuration, IFarmaciaRepository farmaciaRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _productoRepository = farmaciaRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var productos = await _productoRepository.GetAllAsync();

                var data = productos.Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Descripcion,
                    u.Precio,
                    u.StockTotal,
                    Imagen = string.IsNullOrEmpty(u.Imagen) ? "producto.png" : u.Imagen,
                    u.Categoria,
                    u.SubCategoria
                }).ToList();

                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos");
                return Json(new List<object>());
            }
        }

    }
}
