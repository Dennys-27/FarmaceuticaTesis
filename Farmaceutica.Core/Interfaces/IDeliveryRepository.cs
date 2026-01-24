using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);

        Task<int> DeleteAsync(int id);

        Task<IEnumerable<DeliveryVentaDto>> GetVentaDelivery(DateTime? fechaInicio, DateTime? fechaFin);

        Task<List<DeliveryVentaDeatelleDto>> MostrarVentaDetalle(int id);

        Task<DeliveryVentaFacturaDto?> MostrarVentaFactura(int id);

        Task<bool> ProcesarDeliveryAsync(
            int idVenta,
            int repartidorId,
            int estadoDelivery,
            DateTime? fechaAsignacion = null,
            string comentario = ""
        );

        Task<List<RepartidorDto>> ListarRepartidoresAsync();

        Task<IEnumerable<DeliveryVentaDto>> GetVentaDeliveryPorId(DateTime? fechaInicio, DateTime? fechaFin, int id);
        Task<bool> ProcesarDeliveryIdAsync(int ventaId, int estadoAsignado);
    }
}
