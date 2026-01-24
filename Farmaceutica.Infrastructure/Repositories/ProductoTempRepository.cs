using Farmaceutica.Core.Interfaces;
using Farmaceutica.Core.Temporales;
using Farmaceutica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories
{
    public class ProductoTempRepository : IProductoTempRepository
    {
        private readonly AppFarmaceuticaContex _context;
        public ProductoTempRepository(AppFarmaceuticaContex context)
        {
            _context = context;
        }
        public async Task LimpiarAsync()
        {
            var allRecords = await _context.ProductoTemps.ToListAsync();
            _context.ProductoTemps.RemoveRange(allRecords);
            await _context.SaveChangesAsync();
        }

        public async Task InsertarRangoAsync(IEnumerable<ProductoTemp> productos)
        {
            await _context.ProductoTemps.AddRangeAsync(productos);
            await _context.SaveChangesAsync();
        }
    }
}
