using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IventaRepository
    {
        
        Task<int> RegistrarVentaAsync(int usuId);

        Task<DetalleVentaDto> RegistrarDetalleVentaAsync(DetalleVentaDto venta);

        TotalesVentaDto CalcularTotalesConSP(int comprId);
        IEnumerable<DetalleVentaListarDto> GetVentaDetalle(int ventaId);
        Task GuardarVentaAsync(VentaGuardarDto dto);
        Task EliminarDetalleVentaAsync(int detv_id);

        Venta ObtenerVentaPorId(int id);
        List<DetalleVentaFacturaDto> ListarDetalleVenta(int ventaId);
        string? ObtenerNombrePorId(int encargadoId);
        string? ObtenerNombrePorId(int? clienteId);

        Task<IEnumerable<VentaDto>> GetAllAsync();
        Task<List<MostrarVentaDTO>> GetByIdAsync(int id);

        Task<IEnumerable<ProductosVendidosDTO>> GetProductosVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin);

        Task RegistrarVentaAsync(Venta venta);
    }
}
