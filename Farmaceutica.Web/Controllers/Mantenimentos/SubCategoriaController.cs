using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Migrations;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Mantenimentos
{
    public class SubCategoriaController : Controller
    {
        private readonly ILogger<SubCategoriaController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISubCategoriaRepository _subcategoriaRepository;
        public SubCategoriaController(
             ILogger<SubCategoriaController> logger,
            IConfiguration configuration,
            ISubCategoriaRepository SubCategoriaRepository
            )
        {
            _subcategoriaRepository = SubCategoriaRepository;
            _logger = logger;
            _configuration = configuration;
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
                var subcategorias = await _subcategoriaRepository.GetAllAsync();

                var data = subcategorias.Select(u => new object[]
                 {
                    u.Nombre,
                    u.Descripcion,
                    u.Filter,
                    u.CategoriaNombre ?? "Sin categoría",
                    u.IsActive
                        ? "<span class='badge bg-success'>Activo</span>"
                        : "<span class='badge bg-danger'>Inactivo</span>",
                    u.FechaCreacion.ToString("yyyy-MM-dd"),
                    $"<button type='button' class='btn btn-warning btn-icon' onclick='editar({u.Id})'><i class='ri-edit-2-line'></i></button>",
                    $"<button type='button' class='btn btn-danger btn-icon' onclick='eliminar({u.Id})'><i class='ri-delete-bin-5-line'></i></button>"
                 }).ToList();



                return Json(new
                {
                    draw = 1,                // requerido por DataTable
                    recordsTotal = data.Count,
                    recordsFiltered = data.Count,
                    data = data               // clave correcta para DataTable moderno
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar categorias");
                return Json(new { data = new List<object>() });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Guardar(SubCategoria subcategoria)
        {
            try
            {




                if (subcategoria.Id == 0)
                {
                    // Hash de la contraseña

                    await _subcategoriaRepository.CreateAsync(subcategoria);
                }
                else
                {
                    // Solo hashear si se ingresó nueva contraseña

                    await _subcategoriaRepository.UpdateAsync(subcategoria);
                }

                return Json(new { success = true, message = subcategoria.Id == 0 ? "Categoria registrado correctamente." : "categoria actualizado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar categoria");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Mostrar(int id)
        {
            try
            {
                var subcategoria = await _subcategoriaRepository.GetByIdAsync(id);
                if (subcategoria == null)
                    return Json(new { success = false, message = "categoria no encontrado" });

                return Json(new
                {
                    success = true,
                    subcategoria.Id,
                    subcategoria.Descripcion,
                    subcategoria.Nombre,
                    subcategoria.Filter,
                    subcategoria.CategoriaId,

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoria");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var subcategoria = await _subcategoriaRepository.GetByIdAsync(id);
                if (subcategoria == null)
                    return Json(new { success = false, message = "categoria no encontrado" });

                subcategoria.IsActive = false; // Marcamos como inactivo
                await _subcategoriaRepository.DeleteAsync(subcategoria.Id);

                return Json(new { success = true, message = "categoria eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoria");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ComboSubCategoria(int categoria)
        {
            var subcategorias = await _subcategoriaRepository.GetComboAsync(categoria);
            return Json(subcategorias);
        }

    }
}
