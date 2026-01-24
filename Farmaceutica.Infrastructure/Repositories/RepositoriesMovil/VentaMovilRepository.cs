using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Core.Interfaces.InterfacesMovil;
using Farmaceutica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories.RepositoriesMovil
{
    public class VentaMovilRepository : IVentaMovilRepository
    {
        private readonly AppFarmaceuticaContex _context;

        public VentaMovilRepository(AppFarmaceuticaContex context)
        {
            _context = context;
        }

        public async Task<Venta?> GetByIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Encargado)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venta> CreateAsync(Venta venta)
        {
            // Obtener próximo número de factura
            venta.Id = await GetNextNumeroFacturaAsync();

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return venta;
        }

        public async Task<int> GetNextNumeroFacturaAsync()
        {
            var ultimaVenta = await _context.Ventas
                .OrderByDescending(v => v.Id)
                .FirstOrDefaultAsync();

            return (ultimaVenta?.Id ?? 0) + 1;
        }

        public async Task<IEnumerable<Venta>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Ventas
                .Include(v => v.Encargado)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.EncargadoId == usuarioId)
                .OrderByDescending(v => v.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetByFechaAsync(DateTime fecha)
        {
            return await _context.Ventas
                .Include(v => v.Encargado)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.FechaCreacion.Date == fecha.Date)
                .OrderByDescending(v => v.FechaCreacion)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Venta venta)
        {
            _context.Ventas.Update(venta);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
