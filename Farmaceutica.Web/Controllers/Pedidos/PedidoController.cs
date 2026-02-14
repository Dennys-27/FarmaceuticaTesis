using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Farmaceutica.Web.Controllers.Mantenimentos;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Web.Controllers.Pedidos
{
    public class PedidoController : Controller
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPedidoRepository _PedidoRepository;
        

        public PedidoController(ILogger<PedidoController> logger, IConfiguration configuration, IPedidoRepository pedidoRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _PedidoRepository = pedidoRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListarVentasParaAsignarUsuarioCorreo(
            [FromForm] DateTime? fechaInicio,
            [FromForm] DateTime? fechaFin,
            [FromForm] string correo,
            [FromForm] int draw = 1)  // DataTables envía 'draw'
        {
            try
            {
                var delivery = await _PedidoRepository.GetPedidoDeliveryPorCorreo(fechaInicio, fechaFin, correo);

                var data = delivery.Select(u => new object[]
                {
                    // 0 - IMAGEN
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "usuario.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",

                    // 1 - NOMBRE CLIENTE
                    u.Nombre ?? "",

                    // 2 - APELLIDO CLIENTE
                    u.Apellido ?? "",

                    // 3 - METODO PAGO
                    u.MetodoPago ?? "",

                    // 4 - CLI CORREO
                    u.CliCorreo ?? "",

                    // 5 - CLI DIRECCION
                    u.CliDireccion ?? "",

                    // 6 - CLI RUC
                    u.CliRuc ?? "",

                    // 7 - TELEFONO
                    u.Telefono ?? "",

                    // 8 - IGV
                    u.Igv,

                    // 9 - SUBTOTAL
                    u.Subtotal,

                    // 10 - TOTAL
                    u.Total,

                    // 11 - NOMBRE DELIVERY
                    u.PrimerNombre ?? "",

                    // 12 - APELLIDO DELIVERY
                    u.SegundoNombre ?? "",

                    // 13 - FECHA COMPRA
                    u.FechaCompra.ToString("yyyy-MM-dd"),

                    // 14 - FECHA ASIGNACION
                    u.FechaAsignacion?.ToString("yyyy-MM-dd") ?? "",

                    // 15 - FECHA ENTREGA
                    u.FechaEntrega?.ToString("yyyy-MM-dd") ?? "",

                    // 16 - ESTADO
                    u.EstadoDelivery switch
                    {
                        "Pendiente" => "<span class='badge bg-warning'>Pendiente</span>",
                        "Asignado" => "<span class='badge bg-info'>Asignado</span>",
                        "EnCamino" => "<span class='badge bg-primary'>En Camino</span>",
                        "Entregado" => "<span class='badge bg-success'>Entregado</span>",
                        "Rechazado" => "<span class='badge bg-danger'>Rechazado</span>",
                        _ => "<span class='badge bg-secondary'>Desconocido</span>"
                    },

                    // 17 - ACCIONES
                    $"<div class='btn-group btn-group-sm'>" +
                        $"<button class='btn btn-info btn-icon me-1' onclick=\"verDetalle({u.Id})\" title='Ver detalle'>" +
                            $"<i class='ri-eye-line'></i>" +
                        $"</button>" +
                        (u.EstadoDelivery == "Entregado"
                            ? $"<button class='btn btn-warning btn-icon' disabled title='Ya entregado - No se puede modificar'>" +
                                $"<i class='ri-truck-line'></i>" +
                              $"</button>"
                            : $"<button class='btn btn-warning btn-icon' onclick=\"procesar({u.Id})\" title='Asignar delivery'>" +
                                $"<i class='ri-truck-line'></i>" +
                              $"</button>") +
                    $"</div>"
                }).ToList();

                return Json(new
                {
                    draw = draw,  // Devolver el mismo draw recibido
                    recordsTotal = data.Count,
                    recordsFiltered = data.Count,
                    data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar ventas delivery");
                return Json(new
                {
                    draw = draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object[]>()
                });
            }
        }

        public IActionResult Pedidos()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ListarVentasParaAsignarUsuarioPorId(DateTime? fechaInicio, DateTime? fechaFin, int id)
        {
            try
            {
                var delivery = await _PedidoRepository.GetVentaPedidoPorId(fechaInicio, fechaFin, id);

                var data = delivery.Select(u => new object[]
                {
            // 0 - # (índice automático, DataTables lo maneja)
            "", // DataTables pondrá automáticamente el número de fila aquí

            // 1 - CLIENTE (Nombre completo)
            $"<div class='d-flex align-items-center'>" +
                $"<div class='flex-shrink-0 me-2'>" +
                    $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "usuario.png" : u.Imagen)}' " +
                    $"class='avatar-xs rounded-circle' alt='{u.Nombre}'>" +
                $"</div>" +
                $"<div class='flex-grow-1'>" +
                    $"<h6 class='mb-0'>{u.Nombre} {u.Apellido}</h6>" +
                $"</div>" +
            $"</div>",

            // 2 - TIPO VENTA (Método de Pago)
            u.MetodoPago ?? "",

            // 3 - CORREO
            u.CliCorreo ?? "",

            // 4 - DIRECCIÓN
            u.CliDireccion ?? "",

            // 5 - RUC
            u.CliRuc ?? "",

            // 6 - TELÉFONO
            u.Telefono ?? "",

            // 7 - IGV (alineado a la derecha)
            $"<div class='text-end'>{u.Igv:N2}</div>",

            // 8 - SUBTOTAL (alineado a la derecha)
            $"<div class='text-end'>{u.Subtotal:N2}</div>",

            // 9 - TOTAL (alineado a la derecha)
            $"<div class='text-end'>{u.Total:N2}</div>",

            // 10 - REPARTIDOR (Nombre completo)
            $"<div>{u.PrimerNombre} {u.SegundoNombre}</div>",
            
            // 10 - REPARTIDOR (Nombre completo)
            $"<div>{u.TelefonoDelivery}</div>",


            // 11 - FECHA COMPRA
            u.FechaCompra.ToString("yyyy-MM-dd"),

            // 12 - ESTADO
            u.EstadoDelivery switch
            {
                "Pendiente" => "<span class='badge bg-warning'>Pendiente</span>",
                "Asignado" => "<span class='badge bg-info'>Asignado</span>",
                "EnCamino" => "<span class='badge bg-primary'>En Camino</span>",
                "Entregado" => "<span class='badge bg-success'>Entregado</span>",
                "Rechazado" => "<span class='badge bg-danger'>Rechazado</span>",
                _ => "<span class='badge bg-secondary'>Desconocido</span>"
            },

            // 13 - ACCIONES (la última columna)
            $"<div class='text-center'>" +
                $"<button class='btn btn-info btn-icon me-1' onclick=\"verDetalle({u.Id})\" title='Ver detalle'>" +
                    $"<i class='ri-eye-line'></i>" +
                $"</button>" +
                (u.EstadoDelivery == "Entregado" || u.EstadoDelivery == "Rechazado"
                    ? $"<button class='btn btn-warning btn-icon' disabled title='Ya entregado o rechazado - No se puede modificar'>" +
                        $"<i class='ri-truck-line'></i>" +
                      $"</button>"
                    : $"<button class='btn btn-warning btn-icon' onclick=\"procesar({u.Id})\" title='Asignar delivery'>" +
                        $"<i class='ri-truck-line'></i>" +
                      $"</button>") +
            $"</div>"
                }).ToList();

                return Json(new
                {
                    draw = Convert.ToInt32(Request.Form["draw"]), // IMPORTANTE: usar el draw del request
                    recordsTotal = data.Count,
                    recordsFiltered = data.Count,
                    data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar ventas delivery");
                return Json(new
                {
                    draw = Convert.ToInt32(Request.Form["draw"]),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>()
                });
            }
        }


    }
}
