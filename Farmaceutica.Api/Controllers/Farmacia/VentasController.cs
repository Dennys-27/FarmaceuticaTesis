using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Api.Controllers.Farmacia
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<IActionResult> CrearVenta([FromBody] VentaCrearDto ventaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos inválidos",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var ventaCreada = await _ventaService.CrearVentaAsync(ventaDto);

                if (ventaCreada == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo crear la venta. Verifique stock o datos."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Venta creada exitosamente",
                    data = ventaCreada
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        // GET: api/ventas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenta(int id)
        {
            try
            {
                var venta = await _ventaService.GetVentaByIdAsync(id);

                if (venta == null)
                    return NotFound(new { success = false, message = "Venta no encontrada" });

                return Ok(new { success = true, data = venta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener venta",
                    error = ex.Message
                });
            }
        }

        // GET: api/ventas/usuario/5
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetVentasByUsuario(int usuarioId)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByUsuarioAsync(usuarioId);

                return Ok(new
                {
                    success = true,
                    count = ventas.Count(),
                    data = ventas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener ventas",
                    error = ex.Message
                });
            }
        }

        // GET: api/ventas/fecha/2024-01-15
        [HttpGet("fecha/{fecha}")]
        public async Task<IActionResult> GetVentasByFecha(DateTime fecha)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByFechaAsync(fecha);

                return Ok(new
                {
                    success = true,
                    count = ventas.Count(),
                    data = ventas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener ventas por fecha",
                    error = ex.Message
                });
            }
        }
    }
}
