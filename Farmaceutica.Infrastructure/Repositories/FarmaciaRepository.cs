using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories
{
    public class FarmaciaRepository : IFarmaciaRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<FarmaciaRepository> _logger;

        public FarmaciaRepository(AppFarmaceuticaContex context, ILogger<FarmaciaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<ProductoDTO>> GetAllAsync()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "GET");

                // Ejecuta el stored procedure y mapea al DTO
                var productos = await _context.Database
                    .SqlQueryRaw<ProductoDTO>("EXEC SP_PRODUCTO @Operacion", paramOperacion)
                    .ToListAsync();

                return productos; // 👈 corregido
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error ejecutando SP_PRODUCTO");
                return Enumerable.Empty<ProductoDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                return Enumerable.Empty<ProductoDTO>();
            }
        }
    }
}
