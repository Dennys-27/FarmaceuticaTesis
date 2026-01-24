using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces.InterfacesMovil
{
    public interface IVentaMovilRepository
    {
        Task<Venta?> GetByIdAsync(int id);
        Task<Venta> CreateAsync(Venta venta);
        Task<IEnumerable<Venta>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Venta>> GetByFechaAsync(DateTime fecha);
        Task<int> GetNextNumeroFacturaAsync();
        Task<bool> UpdateAsync(Venta venta);
    }
}
