using Farmaceutica.Application.Services;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Inventarios
{
    public class CompraController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CompraController> _logger; 
        private readonly ICompraRepository _compraRepository;
        private readonly CompraService _compraService;
        public CompraController(ILogger<CompraController> logger, IConfiguration configuration, ICompraRepository compraRepository, CompraService compraService)
        {
            _configuration = configuration;
            _logger = logger;
            _compraRepository = compraRepository;
            _compraService = compraService;
        }
        // GET: CompraController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int usu_id)
        {
            try
            {
                var idGenerado = await _compraRepository.RegistrarCompraAsync(usu_id);
                return Json(new { ok = true, message = "Venta registrada correctamente", id = idGenerado });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDetalle([FromForm] DetalleCompraDto dto)
        {
            try
            {
                var detalleGuardado = await _compraService.GuardarDetalleCompraAsync(dto);
                return Json(new
                {
                    ok = true,
                    message = "Detalle guardado correctamente",
                    data = new
                    {
                        detalleGuardado.ComprId,
                        detalleGuardado.ProdId,
                        detalleGuardado.ProdPCompra,
                        detalleGuardado.DetcCant
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Calculo(int compr_id)
        {
            try
            {
                var totales = _compraService.CalcularTotales(compr_id);
                return Json(totales);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ListarDetalle(int compr_id)
        {
            var datos = _compraService.ObtenerDetalleCompra(compr_id);

            var data = datos.Select(row => new object[]
            {
            row.Categoria,
            row.Producto,
            row.Unidad.ToString(), // Enum como string para mostrar
            row.Precio,
            row.Cantidad,
            row.TotalCompra,
            $"<button type='button' onClick='eliminar({row.Id},{row.CompraId})' " +
            $"id='{row.Id}' class='btn btn-danger btn-icon waves-effect waves-light'>" +
            "<i class='ri-delete-bin-5-line'></i></button>"
            }).ToArray();

            var results = new
            {
                sEcho = 1,
                iTotalRecords = data.Length,
                iTotalDisplayRecords = data.Length,
                aaData = data
            };

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDetalle(int detc_id)
        {
            await _compraService.EliminarDetalleCompraAsync(detc_id);
            return Json(new { ok = true, message = "Detalle eliminado correctamente" });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCompra([FromForm] CompraGuardarDto dto)
        {
            try
            {
                await _compraService.GuardaCompraAsync(dto);

                return Json(new
                {
                    ok = true,
                    message = $"Compra registrada correctamente con Nro: V-{dto.ComprId}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }

        public IActionResult Ver(int v)
        {
            var model = _compraService.ObtenerCompraParaVista(v);
            if (model == null) return NotFound("Compra no encontrada");
            return View(model);
        }


        public IActionResult Compras()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Listar(
    string numeroFactura = null,
    string encargado = null,
    string proveedor = null,
    string metodoPago = null,
    DateTime? fechaInicio = null,
    DateTime? fechaFin = null
)
        {
            try
            {
                var compras = await _compraRepository.GetAllAsync();

                // Aplicar filtros en memoria (puedes hacer esto en el repositorio para optimizar)
                if (!string.IsNullOrEmpty(numeroFactura))
                    compras = compras.Where(v => v.NumeroFactura.ToString().Contains(numeroFactura)).ToList();

                if (!string.IsNullOrEmpty(encargado))
                    compras = compras.Where(v => v.Encargado != null && v.Encargado.Contains(encargado, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(proveedor))
                    compras = compras.Where(v => v.Proveedor != null && v.Proveedor.Contains(proveedor, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(metodoPago))
                    compras = compras.Where(v => v.MetodoPago != null && v.MetodoPago.Contains(metodoPago, StringComparison.OrdinalIgnoreCase)).ToList();

                if (fechaInicio.HasValue)
                    compras = compras.Where(v => v.FechaCompra.HasValue && v.FechaCompra.Value.Date >= fechaInicio.Value.Date).ToList();

                if (fechaFin.HasValue)
                    compras = compras.Where(v => v.FechaCompra.HasValue && v.FechaCompra.Value.Date <= fechaFin.Value.Date).ToList();

                var data = compras.Select(u => new object[]
                 {
            u.NumeroFactura,
            u.Encargado,
            u.MetodoPago,
            u.Moneda,
            u.Unidad,
            u.Proveedor,
            u.Ruc,
            u.Correo,
            u.Direccion,
            u.Ruc,
            u.Igv,
            u.Subtotal,
            u.Total,
            u.Comentario,
            u.Estado == 1
                ? "<span class='badge bg-success'>Activo</span>"
                : "<span class='badge bg-danger'>Inactivo</span>",
            u.FechaCompra?.ToString("yyyy-MM-dd") ?? "",
            $"<button type='button' class='btn btn-warning btn-icon' onclick='editar({u.NumeroFactura})'><i class='ri-edit-2-line'></i></button>",
            $"<button type='button' class='btn btn-danger btn-icon' onclick='eliminar({u.NumeroFactura})'><i class='ri-delete-bin-5-line'></i></button>"
                 }).ToList();

                return Json(new
                {
                    draw = 1,
                    recordsTotal = data.Count,
                    recordsFiltered = data.Count,
                    data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar compras");
                return Json(new { data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Mostrar(int numeroFactura)
        {
            try
            {
                var listaCompra = await _compraRepository.GetByIdAsync(numeroFactura);

                if (listaCompra == null || !listaCompra.Any())
                    return Json(new { success = false, message = "Compra no encontrada" });

                // Tomamos los datos generales del primer registro (ya que son iguales en toda la factura)
                var compraGeneral = listaCompra.First();

                return Json(new
                {
                    success = true,
                    encabezado = new
                    {
                        compraGeneral.NumeroFactura,
                        compraGeneral.FechaCompra,
                        compraGeneral.Encargado,
                        compraGeneral.MetodoPago,
                        compraGeneral.Moneda,
                        compraGeneral.Unidad,
                        compraGeneral.Proveedor,
                        compraGeneral.Ruc,
                        compraGeneral.Correo,
                        compraGeneral.Direccion
                    },
                    detalle = listaCompra.Select(v => new
                    {
                        v.Producto,
                        v.CodigoProducto,
                        v.Cantidad,
                        v.PrecioUnitaria,
                        v.Total
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de la compra");
                return Json(new { success = false, message = "Error interno" });
            }
        }


        public IActionResult Reporte()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReporteProductosComprados(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var data = await _compraRepository.GetProductosCompradosAsync(fechaInicio, fechaFin);

                var jsonData = data.Select(d => new object[]
                {
            d.Categoria,
            d.Subcategoria,
            d.Producto,
            d.StockLocal,
            d.CantidadComprado,
            d.PorcentajeComprado
                }).ToList();

                return Json(new
                {
                    draw = 1,
                    recordsTotal = jsonData.Count,
                    recordsFiltered = jsonData.Count,
                    data = jsonData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de productos vendidos");
                return Json(new { data = new List<object>() });
            }
        }

    }
}
