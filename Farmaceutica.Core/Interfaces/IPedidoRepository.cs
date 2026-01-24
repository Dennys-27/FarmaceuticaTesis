using Farmaceutica.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<PedidoPorCliente>>  GetPedidoDeliveryPorCorreo(DateTime? fechaInicio, DateTime? fechaFin, string correo);

        Task<IEnumerable<PedidoPorCliente>> GetVentaPedidoPorId(DateTime? fechaInicio, DateTime? fechaFin, int id);
    }
}
