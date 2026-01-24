using Farmaceutica.Core.DTO;
using Farmaceutica.Core.Interfaces.InterfacesMovil;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Api.Controllers.Farmacia
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoMovilRepository _productoRepository;

        public ProductosController(IProductoMovilRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // ✅ GET: api/productos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var productos = await _productoRepository.GetAllAsync();

                // Mapear manualmente si no usas AutoMapper
                var productosDto = productos.Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    StockTotal = p.StockTotal,
                    StockLocal = p.StockLocal,
                    StockDelivery = p.StockDelivery,
                    Codigo = p.Codigo,
                    Imagen = p.Imagen,
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                    SubCategoriaId = p.SubCategoriaId,
                    SubCategoriaNombre = p.SubCategoria?.Nombre ?? "Sin subcategoría"
                });

                return Ok(new
                {
                    success = true,
                    count = productosDto.Count(),
                    data = productosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener productos",
                    error = ex.Message
                });
            }
        }

        // ✅ GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var producto = await _productoRepository.GetByIdAsync(id);

                if (producto == null)
                    return NotFound(new { success = false, message = "Producto no encontrado" });

                var productoDto = new ProductoDto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    StockTotal = producto.StockTotal,
                    StockLocal = producto.StockLocal,
                    StockDelivery = producto.StockDelivery,
                    Codigo = producto.Codigo,
                    Imagen = producto.Imagen,
                    CategoriaId = producto.CategoriaId,
                    CategoriaNombre = producto.Categoria?.Nombre ?? "Sin categoría",
                    SubCategoriaId = producto.SubCategoriaId,
                    SubCategoriaNombre = producto.SubCategoria?.Nombre ?? "Sin subcategoría"
                };

                return Ok(new { success = true, data = productoDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener el producto",
                    error = ex.Message
                });
            }
        }

        // ✅ GET: api/productos/categoria/1
        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetByCategoria(int categoriaId)
        {
            try
            {
                var productos = await _productoRepository.GetByCategoriaAsync(categoriaId);

                var productosDto = productos.Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    StockTotal = p.StockTotal,
                    StockLocal = p.StockLocal,
                    StockDelivery = p.StockDelivery,
                    Codigo = p.Codigo,
                    Imagen = p.Imagen,
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                    SubCategoriaId = p.SubCategoriaId,
                    SubCategoriaNombre = p.SubCategoria?.Nombre ?? "Sin subcategoría"
                });

                return Ok(new
                {
                    success = true,
                    count = productosDto.Count(),
                    data = productosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener productos por categoría",
                    error = ex.Message
                });
            }
        }

        // ✅ GET: api/productos/buscar/{nombre}
        [HttpGet("buscar/{nombre}")]
        public async Task<IActionResult> Search(string nombre)
        {
            try
            {
                var productos = await _productoRepository.GetByNombreAsync(nombre);

                var productosDto = productos.Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    StockTotal = p.StockTotal,
                    StockLocal = p.StockLocal,
                    StockDelivery = p.StockDelivery,
                    Codigo = p.Codigo,
                    Imagen = p.Imagen,
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                    SubCategoriaId = p.SubCategoriaId,
                    SubCategoriaNombre = p.SubCategoria?.Nombre ?? "Sin subcategoría"
                });

                return Ok(new
                {
                    success = true,
                    count = productosDto.Count(),
                    data = productosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al buscar productos",
                    error = ex.Message
                });
            }
        }

        // ✅ GET: api/productos/con-stock
        [HttpGet("con-stock")]
        public async Task<IActionResult> GetConStock()
        {
            try
            {
                var productos = await _productoRepository.GetProductosConStockAsync();

                var productosDto = productos.Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    StockTotal = p.StockTotal,
                    StockLocal = p.StockLocal,
                    StockDelivery = p.StockDelivery,
                    Codigo = p.Codigo,
                    Imagen = p.Imagen,
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                    SubCategoriaId = p.SubCategoriaId,
                    SubCategoriaNombre = p.SubCategoria?.Nombre ?? "Sin subcategoría"
                });

                return Ok(new
                {
                    success = true,
                    count = productosDto.Count(),
                    data = productosDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener productos con stock",
                    error = ex.Message
                });
            }
        }
    }
}
