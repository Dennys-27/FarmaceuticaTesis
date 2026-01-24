using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Mantenimentos
{
    public class ProductoController : Controller
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IProductoRepository _productoRepository;

        public ProductoController(ILogger<ProductoController> logger, IConfiguration configuration, IProductoRepository productoRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _productoRepository = productoRepository;
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

                var data = productos.Select(u => new object[]
                {
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "producto.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",
                    u.Nombre,
                    u.Descripcion,
                    u.Precio,
                    u.StockTotal,
                    u.StockLocal,
                    u.StockDelivery,
                    u.Codigo,
                    u.Categoria,
                    u.SubCategoria,
                    u.IsActive ? "<span class='badge badge-pill badge-soft-success font-size-12'>Activo</span>"
                                : "<span class='badge badge-pill badge-soft-danger font-size-12'>Inactivo</span>",
                    u.FechaCreacion.ToString("dd/MM/yyyy"),
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
                _logger.LogError(ex, "Error al listar productos");
                return Json(new { data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ListarDatosProcesar()
        {
            try
            {
                var productos = await _productoRepository.ConsultarProductosParaProcesar();

                var data = productos.Select(u => new object[]
                {
                
                    u.Nombre,
                    u.Descripcion,
                    u.Precio,
                    u.StockTotal,
                    u.StockLocal,
                    u.StockDelivery,
                    u.Codigo,
                    u.Categoria,
                    u.SubCategoria,
                    u.EstadoProducto
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
                _logger.LogError(ex, "Error al listar productos");
                return Json(new { data = new List<object>() });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Guardar(ProductoImagen producto, IFormFile? imagen)
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
                    producto.Imagen = fileName;
                }

                
                    // Hash de la contraseña
                await _productoRepository.UpdateAsync(producto);

              

                return Json(new { success = true, message = producto.Id == 0 ? "Producto registrado correctamente." : "Producto actualizado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar Producto");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Mostrar(int id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);
                if (producto == null)
                    return Json(new { success = false, message = "Producto no encontrado" });

                return Json(new
                {
                    success = true,
                    producto.Id,
                    producto.Descripcion,
                    producto.Nombre,
                    producto.Precio,
                    producto.StockTotal,
                    producto.StockLocal,
                    producto.StockDelivery,
                    producto.Codigo,
                    Imagen = string.IsNullOrEmpty(producto.Imagen) ? "producto.png" : producto.Imagen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MostrarVenta(int prod_id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(prod_id);
                if (producto == null)
                    return Json(new { success = false, message = "Producto no encontrado" });

                return Json(new
                {
                    success = true,
                    producto.Id,
                    producto.Descripcion,
                    producto.Nombre,
                    producto.Precio,
                    producto.StockTotal,
                    producto.StockLocal,
                    producto.StockDelivery,
                    producto.Codigo,
                    Imagen = string.IsNullOrEmpty(producto.Imagen) ? "producto.png" : producto.Imagen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MostrarCompra(int prod_id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(prod_id);
                if (producto == null)
                    return Json(new { success = false, message = "Producto no encontrado" });

                return Json(new
                {
                    success = true,
                    producto.Id,
                    producto.Descripcion,
                    producto.Nombre,
                    producto.Precio,
                    producto.StockTotal,
                    producto.StockLocal,
                    producto.StockDelivery,
                    producto.Codigo,
                    Imagen = string.IsNullOrEmpty(producto.Imagen) ? "producto.png" : producto.Imagen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);
                if (producto == null)
                    return Json(new { success = false, message = "Producto no encontrado" });

                producto.IsActive = false; // Marcamos como inactivo
                await _productoRepository.DeleteAsync(producto.Id);

                return Json(new { success = true, message = "Producto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Procesar()
        {
            try
            {
                var productos = await _productoRepository.Procesar();

                if (productos == null || !productos.Any())
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = "No se encontraron productos para procesar."
                    });
                }

                return Json(new
                {
                    ok = true,
                    data = productos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ProductoController.Procesar");
                return Json(new
                {
                    ok = false,
                    mensaje = "Error interno del servidor al procesar los productos."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ComboProductos(int cat_id)
        {
            var productos = await _productoRepository.GetComboAsync(cat_id);
            return Json(productos);
        }


    }


}
