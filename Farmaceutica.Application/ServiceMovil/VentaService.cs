using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Core.DTO;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Core.Interfaces.InterfacesMovil;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.ServiceMovil
{
    public class VentaService : IVentaService
    {
        private readonly IVentaMovilRepository _ventaRepository;
        private readonly IProductoMovilRepository _productoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<VentaService> _logger;

        public VentaService(
            IVentaMovilRepository ventaRepository,
            IProductoMovilRepository productoRepository,
            IUsuarioRepository usuarioRepository,
            ILogger<VentaService> logger)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<VentaResponseDto?> CrearVentaAsync(VentaCrearDto ventaDto)
        {
            try
            {
                // 1. Validar que el usuario existe
                var usuario = await _usuarioRepository.GetByIdAsync(ventaDto.UsuarioId);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuario {ventaDto.UsuarioId} no encontrado");
                    return null;
                }

                // 2. Validar stock de productos y calcular totales
                decimal subtotal = 0;
                var detallesVenta = new List<DetalleVenta>();

                foreach (var detalleDto in ventaDto.Detalles)
                {
                    var producto = await _productoRepository.GetByIdAsync(detalleDto.ProductoId);
                    if (producto == null)
                    {
                        _logger.LogWarning($"Producto {detalleDto.ProductoId} no encontrado");
                        return null;
                    }

                    // Validar stock
                    if (producto.StockTotal < detalleDto.Cantidad)
                    {
                        _logger.LogWarning($"Stock insuficiente para {producto.Nombre}. Stock: {producto.StockTotal}, Solicitado: {detalleDto.Cantidad}");
                        return null;
                    }

                    // Calcular total por producto
                    var totalProducto = detalleDto.Cantidad * detalleDto.Precio;
                    subtotal += totalProducto;

                    // Crear detalle de venta
                    var detalleVenta = new DetalleVenta
                    {
                        ProductoId = detalleDto.ProductoId,
                        Cantidad = detalleDto.Cantidad,
                        Precio = detalleDto.Precio,
                        TotalVenta = totalProducto,
                        IsActive = true
                    };

                    detallesVenta.Add(detalleVenta);

                    // Actualizar stock (podría hacerse en una transacción separada)
                    producto.StockTotal -= detalleDto.Cantidad;
                    await _productoRepository.UpdateAsync(producto);
                }

                // 3. Calcular IGV (18%)
                decimal igv = subtotal * 0.18m;
                decimal total = subtotal + igv;

                // 4. Crear la venta
                var venta = new Venta
                {
                    EncargadoId = ventaDto.UsuarioId,
                    MetodoPago = ventaDto.MetodoPago,
                    Moneda = ventaDto.Moneda,
                    Unidad = ventaDto.Unidad,
                    DocumentoTipo = ventaDto.DocumentoTipo,
                    TipoVenta = ventaDto.TipoVenta,
                    CliRuc = ventaDto.ClienteRuc,
                    CliCorreo = ventaDto.ClienteCorreo,
                    CliDireccion = ventaDto.ClienteDireccion,
                    DireccionEntrega = ventaDto.DireccionEntrega,
                    Subtotal = subtotal,
                    Igv = igv,
                    Total = total,
                    Comentario = ventaDto.Comentario,
                    FechaCreacion = DateTime.UtcNow,
                    IsActive = 1,
                    DetalleVentas = detallesVenta
                };

                // 5. Guardar venta
                var ventaCreada = await _ventaRepository.CreateAsync(venta);

                // 6. Mapear a DTO de respuesta
                return MapToResponseDto(ventaCreada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear venta");
                return null;
            }
        }

        public async Task<VentaResponseDto?> GetVentaByIdAsync(int id)
        {
            var venta = await _ventaRepository.GetByIdAsync(id);
            if (venta == null) return null;

            return MapToResponseDto(venta);
        }

        public async Task<IEnumerable<VentaResponseDto>> GetVentasByUsuarioAsync(int usuarioId)
        {
            var ventas = await _ventaRepository.GetByUsuarioIdAsync(usuarioId);
            return ventas.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<VentaResponseDto>> GetVentasByFechaAsync(DateTime fecha)
        {
            var ventas = await _ventaRepository.GetByFechaAsync(fecha);
            return ventas.Select(MapToResponseDto);
        }

        private VentaResponseDto MapToResponseDto(Venta venta)
        {
            return new VentaResponseDto
            {
                Id = venta.Id,
                
                Encargado = venta.Encargado?.Nombre ?? "Sin encargado",
                MetodoPago = venta.MetodoPago.ToString(),
                Moneda = venta.Moneda.ToString(),
                DocumentoTipo = venta.DocumentoTipo.ToString(),
                TipoVenta = venta.TipoVenta.ToString(),
                ClienteNombre = venta.Cliente?.Nombre ?? venta.CliRuc,
                ClienteRuc = venta.CliRuc,
                Subtotal = venta.Subtotal ?? 0,
                Igv = venta.Igv ?? 0,
                Total = venta.Total ?? 0,
                Estado = "COMPLETADA", // Puedes agregar lógica de estado
                Comentario = venta.Comentario,
                FechaVenta = venta.FechaCreacion,
                Detalles = venta.DetalleVentas?.Select(d => new DetalleVentaResponseDto
                {
                    ProductoId = d.ProductoId,
                    ProductoNombre = d.Producto?.Nombre ?? "Producto desconocido",
                    ProductoCodigo = d.Producto?.Codigo ?? "N/A",
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Total = d.TotalVenta
                }).ToList() ?? new List<DetalleVentaResponseDto>()
            };
        }
    }
}
