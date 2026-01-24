using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Enums;
using Farmaceutica.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class VentaService
    {
        private readonly IventaRepository _ventaRepository;

        public VentaService(IventaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        public async Task<int> GuardarDetalleAsync(int usuId)
        {
            return await _ventaRepository.RegistrarVentaAsync(usuId);
        }

        public async Task<DetalleVentaDto> GuardarDetalleVentaAsync(DetalleVentaDto dto)
        {
            var detalle = new DetalleVentaDto
            {
                VentId = dto.VentId,
                ProdId = dto.ProdId,
                ProdPVenta = dto.ProdPVenta,
                DetvCant = dto.DetvCant
            };

            return await _ventaRepository.RegistrarDetalleVentaAsync(detalle);
        }

        public TotalesVentaDto CalcularTotales(int comprId)
        {
            return _ventaRepository.CalcularTotalesConSP(comprId);
        }


        public async Task EliminarDetalleVentaAsync(int detv_id)
        {
            await _ventaRepository.EliminarDetalleVentaAsync(detv_id);
        }
        public IEnumerable<DetalleVentaListarDto> ObtenerDetalleVenta(int ventaId)
        {
            var detalles = _ventaRepository.GetVentaDetalle(ventaId);

            // Convertir Unidad int a enum (si es necesario)
            foreach (var det in detalles)
            {
                det.Unidad = Enum.TryParse<UnidadEnum>(det.Unidad.ToString(), out var u) ? u : UnidadEnum.Unidad;
            }

            return detalles;
        }

        public async Task GuardarVentaAsync(VentaGuardarDto dto)
        {
            await _ventaRepository.GuardarVentaAsync(dto);
        }

        public VentaVerViewModel ObtenerVentaParaVista(int id)
        {
            var venta = _ventaRepository.ObtenerVentaPorId(id);
            if (venta == null) return null;

            var detalles = _ventaRepository.ListarDetalleVenta(id);

            var detallesDto = detalles.Select(d => new DetalleVentaFacturaDto
            {
                Categoria = d.Categoria ?? "Sin Categoria",
                ProductoNombre = d.ProductoNombre ?? "Sin Producto",
                Unidad = d.Unidad ?? "UND",
                PrecioUnitario = d.PrecioUnitario,
                Cantidad = d.Cantidad,
                TotalVenta = d.TotalVenta
            }).ToList();

            // Obtener nombre del cliente desde el id
            string cliNombre = _ventaRepository.ObtenerNombrePorId(venta.ClienteId) ?? "Sin Cliente";

            // Obtener nombre del encargado desde el id
            string usuNombre = _ventaRepository.ObtenerNombrePorId(venta.EncargadoId) ?? "Sin Usuario";

            var model = new VentaVerViewModel
            {
                VentaId = venta.Id,
                Fecha = venta.FechaCreacion,
                CliNom = cliNombre,
                CliRuc = venta.CliRuc ?? "-",
                CliDirecc = venta.CliDireccion ?? "-",
                CliCorreo = venta.CliCorreo ?? "-",
                UsuNom = usuNombre,
                MonNom = venta.Moneda.ToString(),
                PagNom = venta.MetodoPago.ToString(),
                Subtotal = venta.Subtotal.GetValueOrDefault(),
                Igv = venta.Igv.GetValueOrDefault(),
                Total = venta.Total.GetValueOrDefault(),
                Comentario = venta.Comentario ?? "-",
                Detalles = detallesDto
            };

            return model;
        }


        // Reemplaza el método RegistrarVentaAsync para corregir el error CS0029
        public async Task RegistrarVentaAsync(Venta venta)
        {
            if (venta.DetalleVentas == null || !venta.DetalleVentas.Any())
                throw new ArgumentException("La venta debe tener al menos un producto.");

            venta.Subtotal = venta.DetalleVentas.Sum(d => d.TotalVenta);
            venta.Igv = venta.Subtotal * 0.12m; // Ejemplo: 12% IGV
            venta.Total = venta.Subtotal + venta.Igv;
            venta.FechaCreacion = DateTime.Now;
            venta.TipoVenta = TipoVenta.Web;

            await _ventaRepository.RegistrarVentaAsync(venta);
        }

    }
}