using Farmaceutica.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.ServiceMovil.IServiceMovil
{
    public interface IVentaService
    {
        Task<VentaResponseDto?> CrearVentaAsync(VentaCrearDto ventaDto);
        Task<VentaResponseDto?> GetVentaByIdAsync(int id);
        Task<IEnumerable<VentaResponseDto>> GetVentasByUsuarioAsync(int usuarioId);
        Task<IEnumerable<VentaResponseDto>> GetVentasByFechaAsync(DateTime fecha);
    }
}
