using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Mantenimentos
{
    public class CategoriaController : Controller
    {
        private readonly ILogger<CategoriaController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaController(
             ILogger<CategoriaController> logger,
            IConfiguration configuration,
            ICategoriaRepository categoriaRepository
            )
        {
            _categoriaRepository = categoriaRepository;
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
                var categorias = await _categoriaRepository.GetAllAsync();

                var data = categorias.Select(u => new object[]
                {
           
            u.Nombre,
            u.Descripcion,
            u.filter,
            u.IsActive == true
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
        public async Task<IActionResult> Guardar(Categoria categoria)
        {
            try
            {


                

                if (categoria.Id == 0)
                {
                    // Hash de la contraseña
                   
                    await _categoriaRepository.CreateAsync(categoria);
                }
                else
                {
                    // Solo hashear si se ingresó nueva contraseña
                    
                    await _categoriaRepository.UpdateAsync(categoria);
                }

                return Json(new { success = true, message = categoria.Id == 0 ? "Categoria registrado correctamente." : "categoria actualizado correctamente." });
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
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                    return Json(new { success = false, message = "categoria no encontrado" });

                return Json(new
                {
                    success = true,
                    categoria.Id,
                    categoria.Descripcion,
                    categoria.Nombre,
                    categoria.filter
                  
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
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                    return Json(new { success = false, message = "categoria no encontrado" });

                categoria.IsActive = false; // Marcamos como inactivo
                await _categoriaRepository.DeleteAsync(categoria.Id);

                return Json(new { success = true, message = "categoria eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoria");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ComboCategoria(int categoria)
        {
            var categorias = await _categoriaRepository.GetComboAsync();
            return Json(categorias);
        }

    }
}
