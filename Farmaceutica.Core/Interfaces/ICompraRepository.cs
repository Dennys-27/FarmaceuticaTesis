using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface ICompraRepository
    {
        TotalesCompraDto CalcularTotalesConSP(int comprId);
        Task<int> RegistrarCompraAsync(int usuId);

        Task<DetalleCompraDto> RegistrarDetalleCompraAsync(DetalleCompraDto compra);

        IEnumerable<DetalleCompraListarDto> GetCompraDetalle(int compraId);
        Task EliminarDetalleCompraAsync(int detc_id);

        Task GuardarCompraAsync(CompraGuardarDto dto);

        Compra ObtenerCompraPorId(int id);
        List<DetalleCompraFacturaDto> ListarDetalleCompra(int compraId);

        string? ObtenerNombrePorId(int encargadoId);
        string? ObtenerNombrePorId(int? proveedorId);

        Task<IEnumerable<CompraDto>> GetAllAsync();
        Task<IEnumerable<MostrarCompraDTO>> GetByIdAsync(int numeroFactura);
        Task<IEnumerable<ProductosCompradosDTO>> GetProductosCompradosAsync(DateTime? fechaInicio, DateTime? fechaFin);
    }
}
