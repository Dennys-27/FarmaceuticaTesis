using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
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
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<ProveedorRepository> _logger;

        public ProveedorRepository(AppFarmaceuticaContex context, ILogger<ProveedorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "GET");

                // Ejecuta el stored procedure y mapea a la entidad Usuario
                var empresas = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_PROVEEDOR @Operacion", paramOperacion)
                    .ToListAsync();

                return empresas;
            }
            catch (SqlException sqlEx)
            {
                // Loguea errores de SQL
                _logger.LogError(sqlEx, "Error ejecutando SP_PROVEDOR");
                return Enumerable.Empty<Usuario>();
            }
            catch (Exception ex)
            {
                // Loguea errores generales
                _logger.LogError(ex, "Error al obtener la lista de Proveedores");
                return Enumerable.Empty<Usuario>();
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                var empresas = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_PROVEEDOR @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();

                return empresas.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_PROVEEDOR @Operacion={0}, @Id={1}",
                    "DELETE", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<ComboClienteDto>> GetComboAsync()
        {
            var result = await _context.Database
               .SqlQueryRaw<ComboClienteDto>("EXEC SP_PROVEEDOR @Operacion={0}", "COMBO")
               .ToListAsync();

            return result;
        }
    }
}
