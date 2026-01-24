using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Mantenimentos
{
    public class EncargadoController : Controller
    {
        private readonly ILogger<EncargadoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEncargadoRepository _encargadoRepository;
        
        public EncargadoController(
            ILogger<EncargadoController> logger, 
            IConfiguration configuration,
            IEncargadoRepository encargadoRepository
            )
        {
            _logger = logger;
            _configuration = configuration;
            _encargadoRepository = encargadoRepository;

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
                var clientes = await _encargadoRepository.GetAllAsync();

                var data = clientes.Select(u => new object[]
                {
            $"<div class='d-flex align-items-center'>" +
                $"<div class='flex-shrink-0 me-2'>" +
                    $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "usuario.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                $"</div>" +
            $"</div>",
            u.Email,
            u.Nombre,
            u.Apellido,
            u.UsuarioNombre,
            u.Telefono,
            u.Rol.ToString(),
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
                _logger.LogError(ex, "Error al listar usuarios");
                return Json(new { data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Mostrar(int id)
        {
            try
            {
                var cliente = await _encargadoRepository.GetByIdAsync(id);
                if (cliente == null)
                    return Json(new { success = false, message = "Cliente no encontrado" });

                return Json(new
                {
                    success = true,
                    cliente.Id,
                    cliente.Email,
                    cliente.Nombre,
                    cliente.Apellido,
                    cliente.UsuarioNombre,
                    cliente.Telefono,
                    Password = cliente.Password,
                    Rol = (int)cliente.Rol,
                    Imagen = string.IsNullOrEmpty(cliente.Imagen) ? "usuario.png" : cliente.Imagen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var cliente = await _encargadoRepository.GetByIdAsync(id);
                if (cliente == null)
                    return Json(new { success = false, message = "Proveedor no encontrado" });

                cliente.IsActive = false; // Marcamos como inactivo
                await _encargadoRepository.DeleteAsync(cliente.Id);

                return Json(new { success = true, message = "Proveedor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor");
                return Json(new { success = false, message = "Error interno" });
            }
        }
    }
}
