using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class CompraService
    {
        private readonly ICompraRepository _compraRepository;

        public CompraService(ICompraRepository compraRepository)
        {
            _compraRepository = compraRepository;
        }

        public async Task<DetalleCompraDto> GuardarDetalleCompraAsync(DetalleCompraDto dto)
        {
            var detalle = new DetalleCompraDto
            {
                ComprId = dto.ComprId,
                ProdId = dto.ProdId,
                ProdPCompra = dto.ProdPCompra,
                DetcCant = dto.DetcCant
            };

            return await _compraRepository.RegistrarDetalleCompraAsync(detalle);
        }

        public TotalesCompraDto CalcularTotales(int comprId)
        {
            return _compraRepository.CalcularTotalesConSP(comprId);
        }

        public IEnumerable<DetalleCompraListarDto> ObtenerDetalleCompra(int compraId)
        {
            var detalles = _compraRepository.GetCompraDetalle(compraId);

            // Convertir Unidad int a enum (si es necesario)
            foreach (var det in detalles)
            {
                det.Unidad = Enum.TryParse<UnidadEnum>(det.Unidad.ToString(), out var u) ? u : UnidadEnum.Unidad;
            }

            return detalles;
        }

        public async Task EliminarDetalleCompraAsync(int detc_id)
        {
            await _compraRepository.EliminarDetalleCompraAsync(detc_id);
        }

        public async Task GuardaCompraAsync(CompraGuardarDto dto)
        {
            await _compraRepository.GuardarCompraAsync(dto);
        }

        public CompraVerViewModel ObtenerCompraParaVista(int id)
        {
            var compra = _compraRepository.ObtenerCompraPorId(id);
            if (compra == null) return null;

            var detalles = _compraRepository.ListarDetalleCompra(id);

            var detallesDto = detalles.Select(d => new DetalleCompraFacturaDto
            {
                Categoria = d.Categoria ?? "Sin Categoria",
                ProductoNombre = d.ProductoNombre ?? "Sin Producto",
                Unidad = d.Unidad ?? "UND",
                PrecioUnitario = d.PrecioUnitario,
                Cantidad = d.Cantidad,
                TotalCompra = d.TotalCompra
            }).ToList();

            // Obtener nombre del cliente desde el id
            string provNombre = _compraRepository.ObtenerNombrePorId(compra.ProveedorId) ?? "Sin Cliente";

            // Obtener nombre del encargado desde el id
            string usuNombre = _compraRepository.ObtenerNombrePorId(compra.EncargadoId) ?? "Sin Usuario";

            var model = new CompraVerViewModel
            {
                CompraId = compra.Id,
                Fecha = compra.FechaCreacion,
                ProvNom = provNombre,
                ProvRuc = compra.ProvRuc ?? "-",
                ProvDirecc = compra.ProvDireccion ?? "-",
                ProvCorreo = compra.ProvCorreo ?? "-",
                UsuNom = usuNombre,
                MonNom = compra.Moneda.ToString(),
                PagNom = compra.MetodoPago.ToString(),
                Subtotal = compra.Subtotal.GetValueOrDefault(),
                Igv = compra.Igv.GetValueOrDefault(),
                Total = compra.Total.GetValueOrDefault(),
                Comentario = compra.Comentario ?? "-",
                Detalles = detallesDto
            };

            return model;
        }
    }
}
