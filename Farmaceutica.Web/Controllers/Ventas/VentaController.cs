using Farmaceutica.Application.Services;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Farmaceutica.Web.Controllers.Mantenimentos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Ventas
{
    public class VentaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VentaController> _logger; private readonly VentaService _ventaService;
        private readonly IventaRepository _ventaRepository;
        public VentaController(ILogger<VentaController> logger, IConfiguration configuration,VentaService ventaService, IventaRepository ventaRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _ventaService = ventaService;
            _ventaRepository = ventaRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int usu_id)
        {
            try
            {
                var idGenerado = await _ventaService.GuardarDetalleAsync(usu_id);
                return Json(new { ok = true, message = "Venta registrada correctamente", id = idGenerado });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDetalle([FromForm] DetalleVentaDto dto)
        {
            try
            {
                var detalleGuardado = await _ventaService.GuardarDetalleVentaAsync(dto);
                return Json(new
                {
                    ok = true,
                    message = "Detalle guardado correctamente",
                    data = new
                    {
                        detalleGuardado.VentId,
                        detalleGuardado.ProdId,
                        detalleGuardado.ProdPVenta,
                        detalleGuardado.DetvCant
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Calculo(int vent_id)
        {
            try
            {
                var totales = _ventaService.CalcularTotales(vent_id);
                return Json(totales);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDetalle(int detv_id)
        {
            await _ventaService.EliminarDetalleVentaAsync(detv_id);
            return Json(new { ok = true, message = "Detalle eliminado correctamente" });
        }


        [HttpPost]
        public IActionResult ListarDetalle(int vent_id)
        {
            var datos = _ventaService.ObtenerDetalleVenta(vent_id);

            var data = datos.Select(row => new object[]
            {
            row.Categoria,
            row.Producto,
            row.Unidad.ToString(), // Enum como string para mostrar
            row.Precio,
            row.Cantidad,
            row.TotalVenta,
            $"<button type='button' onClick='eliminar({row.Id},{row.VentaId})' " +
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
        public async Task<IActionResult> GuardarVenta([FromForm] VentaGuardarDto dto)
        {
            try
            {
                await _ventaService.GuardarVentaAsync(dto);

                return Json(new
                {
                    ok = true,
                    message = $"Venta registrada correctamente con Nro: V-{dto.VentId}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }


        public IActionResult Ver(int v)
        {
            var model = _ventaService.ObtenerVentaParaVista(v);
            if (model == null) return NotFound("Venta no encontrada");
            return View(model);
        }

        public IActionResult Ventas()
        {
           
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Listar(
    string numeroFactura = null,
    string encargado = null,
    string cliente = null,
    string metodoPago = null,
    DateTime? fechaInicio = null,
    DateTime? fechaFin = null
)
        {
            try
            {
                var ventas = await _ventaRepository.GetAllAsync();

                // Aplicar filtros en memoria (puedes hacer esto en el repositorio para optimizar)
                if (!string.IsNullOrEmpty(numeroFactura))
                    ventas = ventas.Where(v => v.NumeroFactura.ToString().Contains(numeroFactura)).ToList();

                if (!string.IsNullOrEmpty(encargado))
                    ventas = ventas.Where(v => v.Encargado != null && v.Encargado.Contains(encargado, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(cliente))
                    ventas = ventas.Where(v => v.Cliente != null && v.Cliente.Contains(cliente, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(metodoPago))
                    ventas = ventas.Where(v => v.MetodoPago != null && v.MetodoPago.Contains(metodoPago, StringComparison.OrdinalIgnoreCase)).ToList();

                if (fechaInicio.HasValue)
                    ventas = ventas.Where(v => v.FechaVenta.HasValue && v.FechaVenta.Value.Date >= fechaInicio.Value.Date).ToList();

                if (fechaFin.HasValue)
                    ventas = ventas.Where(v => v.FechaVenta.HasValue && v.FechaVenta.Value.Date <= fechaFin.Value.Date).ToList();

                var data = ventas.Select(u => new object[]
                 {
            u.NumeroFactura,
            u.Encargado,
            u.MetodoPago,
            u.Moneda,
            u.Unidad,
            u.Cliente,
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
            u.FechaVenta?.ToString("yyyy-MM-dd") ?? "",
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
                _logger.LogError(ex, "Error al listar ventas");
                return Json(new { data = new List<object>() });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Mostrar(int numeroFactura)
        {
            try
            {
                var listaVenta = await _ventaRepository.GetByIdAsync(numeroFactura);

                if (listaVenta == null || !listaVenta.Any())
                    return Json(new { success = false, message = "Venta no encontrada" });

                // Tomamos los datos generales del primer registro (ya que son iguales en toda la factura)
                var ventaGeneral = listaVenta.First();

                return Json(new
                {
                    success = true,
                    encabezado = new
                    {
                        ventaGeneral.NumeroFactura,
                        ventaGeneral.FechaVenta,
                        ventaGeneral.Encargado,
                        ventaGeneral.MetodoPago,
                        ventaGeneral.Moneda,
                        ventaGeneral.Unidad,
                        ventaGeneral.Cliente,
                        ventaGeneral.Ruc,
                        ventaGeneral.Correo,
                        ventaGeneral.Direccion
                    },
                    detalle = listaVenta.Select(v => new
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
                _logger.LogError(ex, "Error al obtener detalles de la venta");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        public IActionResult Reporte()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReporteProductosVendidos(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var data = await _ventaRepository.GetProductosVendidosAsync(fechaInicio, fechaFin);

                var jsonData = data.Select(d => new object[]
                {
            d.Categoria,
            d.Subcategoria,
            d.Producto,
            d.StockLocal,
            d.CantidadVendida,
            d.PorcentajeVendido
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



        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] Venta venta)
        {
            try
            {
                await _ventaService.RegistrarVentaAsync(venta);
                return Ok(new { message = "Venta registrada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}
