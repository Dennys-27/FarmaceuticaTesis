using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Farmaceutica.Web.Controllers.Mantenimentos
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        public UsuariosController(
            ILogger<UsuariosController> logger, 
            IConfiguration configuration,
            IUsuarioRepository usuarioRepository
            )
        {
            _logger = logger;
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = new PasswordHasher<Usuario>();
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
                var usuarios = await _usuarioRepository.GetAllAsync();

                var data = usuarios.Select(u => new object[]
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
        public async Task<IActionResult> Guardar(Usuario usuario, IFormFile? imagen)
        {
            try
            {
                

                // Guardar imagen
                if (imagen != null && imagen.Length > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                    if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }
                    usuario.Imagen = fileName;
                }

                if (usuario.Id == 0)
                {
                    // Hash de la contraseña
                    usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);
                    await _usuarioRepository.CreateAsync(usuario);
                }
                else
                {
                    // Solo hashear si se ingresó nueva contraseña
                    if (!string.IsNullOrEmpty(usuario.Password))
                    {
                        usuario.Password = _passwordHasher.HashPassword(usuario, usuario.Password);
                    }
                    await _usuarioRepository.UpdateAsync(usuario);
                }

                return Json(new { success = true, message = usuario.Id == 0 ? "Usuario registrado correctamente." : "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar usuario");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Mostrar(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                    return Json(new { success = false, message = "Usuario no encontrado" });

                return Json(new
                {
                    success = true,
                    usuario.Id,
                    usuario.Email,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.UsuarioNombre,
                    usuario.Telefono,
                    Password = usuario.Password,
                    Rol = (int)usuario.Rol,
                    Imagen = string.IsNullOrEmpty(usuario.Imagen) ? "usuario.png" : usuario.Imagen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                    return Json(new { success = false, message = "Usuario no encontrado" });

                usuario.IsActive = false; // Marcamos como inactivo
                await _usuarioRepository.DeleteAsync(usuario.Id);

                return Json(new { success = true, message = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                return Json(new { success = false, message = "Error interno" });
            }
        }


        
    }
}
