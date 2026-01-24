using Farmaceutica.Application.Services;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Farmaceutica.Web.Controllers.Mantenimentos
{
    public class DeliveryController : Controller
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDeliveryRepository _DeliveryRepository;

        public DeliveryController(
            ILogger<DeliveryController> logger,
            IConfiguration configuration,
            IDeliveryRepository DeliveryRepository
            )
        {
            _logger = logger;
            _configuration = configuration;
            _DeliveryRepository = DeliveryRepository;

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
                var clientes = await _DeliveryRepository.GetAllAsync();

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
                var cliente = await _DeliveryRepository.GetByIdAsync(id);
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
                var cliente = await _DeliveryRepository.GetByIdAsync(id);
                if (cliente == null)
                    return Json(new { success = false, message = "Proveedor no encontrado" });

                cliente.IsActive = false; // Marcamos como inactivo
                await _DeliveryRepository.DeleteAsync(cliente.Id);

                return Json(new { success = true, message = "Proveedor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor");
                return Json(new { success = false, message = "Error interno" });
            }
        }


        public IActionResult VentasDelivery()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ListarVentasParaAsignarDelivery(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var delivery = await _DeliveryRepository.GetVentaDelivery(fechaInicio, fechaFin);

                var data = delivery.Select(u => new object[]
                {
                    // 0 - IMAGEN
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "usuario.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",

                    // 1 - CLIENTE
                    u.Cliente,

                    // 2 - METODO PAGO
                    u.MetodoPago,

                    // 3 - TIPO VENTA
                    u.TipoVenta,

                    // 4
                    u.CliCorreo,

                    // 5
                    u.CliDireccion,

                    // 6
                    u.CliRuc,

                    // 7
                    u.Telefono,

                    // 8
                    u.Igv,

                    // 9
                    u.Subtotal,

                    // 10
                    u.Total,

                    // 11 - ENCARGADO
                    u.Encargado,

                    // 12 - DELIVERY
                    u.Delivery,

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
                        _   => "<span class='badge bg-secondary'>Desconocido</span>"
                    },

                    // 17 - ACCIONES (UNA SOLA COLUMNA)
                    $"<button class='btn btn-info btn-icon me-1' onclick='verDetalle({u.Id})' title='Ver detalle'>" +
                    "<i class='ri-eye-line'></i></button>" +

                    $"<button class='btn btn-warning btn-icon' onclick='procesar({u.Id})' title='Asignar delivery'>" +
                    "<i class='ri-truck-line'></i></button>"
                }).ToList();


                return Json(new
                {
                    draw = 1,
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
                    draw = 1,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MostrarVentaFactura(int id)
        {
            try
            {
                var venta = await _DeliveryRepository.MostrarVentaFactura(id);
                if (venta == null)
                    return Json(new { success = false, message = "Venta no encontrado" });

                return Json(new
                {
                    success = true,
                    venta.NumeroFactura,
                    venta.FechaVenta,
                    venta.Cliente,
                    venta.Apellido,
                    venta.Correo,
                    venta.Direccion,
                    venta.Ruc,
                    venta.Telefono,
                    venta.MetodoPago,
                    venta.Subtotal,
                    venta.Igv,
                    venta.Total
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener venta");
                return Json(new { success = false, message = "Error interno" });
            }
        }

        //======================================
        //DETALLE DE VENTA(LISTA)
        //======================================
        [HttpPost]
        public async Task<IActionResult> MostrarVentaDetalle(int id)
        {
            try
            {
                var listaVenta = await _DeliveryRepository.MostrarVentaDetalle(id);

                if (listaVenta == null)
                    return Json(new { success = false, message = "Venta no encontrada" });

                var data = listaVenta.Select(u => new object[]
                    {
                    // 0 - IMAGEN
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "usuario.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",

                    // 1 - Producto
                    u.Producto,

                    // 2 - Cantidad
                    u.Cantidad,

                    // 3 - Precio
                    u.Precio,

                    // 4 TotalVenta
                    u.TotalVenta,

                    }).ToList();


                return Json(new
                {
                    draw = 1,
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
                    draw = 1,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>()
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ProcesarDelivery(
    int ventaId,
    int repartidorId,
    DateTime? fechaAsignacion,
    string comentario
)
        {
            try
            {
                int estadoAsignado = 2; // Estado "Asignado"

                // Llamamos al repository
                bool ok = await _DeliveryRepository.ProcesarDeliveryAsync(
                    ventaId,
                    repartidorId,
                    estadoAsignado,
                    fechaAsignacion,
                    comentario
                );

                if (ok)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Delivery asignado correctamente"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se pudo asignar el delivery"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar delivery | VentaId={VentaId} | RepartidorId={RepartidorId}", ventaId, repartidorId);
                return Json(new
                {
                    success = false,
                    message = "Error interno al procesar delivery"
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ListarRepartidores()
        {
            var lista = await _DeliveryRepository.ListarRepartidoresAsync();

            return Json(lista.Select(r => new
            {
                id = r.Id,
                nombre = $"{r.Nombre} {r.Apellido}"
            }));
        }


        public IActionResult VentasDeliveryPorId()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ListarVentasParaAsignarDeliveryPorId(DateTime? fechaInicio, DateTime? fechaFin, int id)
        {
            try
            {
                var delivery = await _DeliveryRepository.GetVentaDeliveryPorId(fechaInicio, fechaFin, id);

                var data = delivery.Select(u => new object[]
                {
                    // 0 - IMAGEN
                    $"<div class='d-flex align-items-center'>" +
                        $"<div class='flex-shrink-0 me-2'>" +
                            $"<img src='/images/users/{(string.IsNullOrEmpty(u.Imagen) ? "usuario.png" : u.Imagen)}' class='avatar-xs rounded-circle'>" +
                        $"</div>" +
                    $"</div>",

                    // 1 - CLIENTE
                    u.Cliente,

                    // 2 - METODO PAGO
                    u.MetodoPago,

                    // 3 - TIPO VENTA
                    u.TipoVenta,

                    // 4
                    u.CliCorreo,

                    // 5
                    u.CliDireccion,

                    // 6
                    u.CliRuc,

                    // 7
                    u.Telefono,

                    // 8
                    u.Igv,

                    // 9
                    u.Subtotal,

                    // 10
                    u.Total,

                    // 11 - ENCARGADO
                    u.Encargado,

                    // 12 - DELIVERY
                    u.Delivery,

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
                        _   => "<span class='badge bg-secondary'>Desconocido</span>"
                    },

                    // 17 - ACCIONES (UNA SOLA COLUMNA)
                    $"<button class='btn btn-info btn-icon me-1' onclick='verDetalle({u.Id})' title='Ver detalle'>" +
                    "<i class='ri-eye-line'></i></button>" +

                    $"<button class='btn btn-warning btn-icon' onclick='procesar({u.Id})' title='Asignar delivery'>" +
                    "<i class='ri-truck-line'></i></button>"
                }).ToList();


                return Json(new
                {
                    draw = 1,
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
                    draw = 1,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>()
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> ProcesarDeliveryPorId(
    int ventaId, int estadoDelivery
)
        {
            try
            {
               

                // Llamamos al repository
                bool ok = await _DeliveryRepository.ProcesarDeliveryIdAsync(
                    ventaId,

                    estadoDelivery


                );

                if (ok)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Delivery asignado correctamente"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se pudo asignar el delivery"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar delivery | VentaId={VentaId}", ventaId);
                return Json(new
                {
                    success = false,
                    message = "Error interno al procesar delivery"
                });
            }
        }

    }
}
