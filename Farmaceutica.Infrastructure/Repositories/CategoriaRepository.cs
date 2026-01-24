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
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<CategoriaRepository> _logger;

        public CategoriaRepository(AppFarmaceuticaContex context, ILogger<CategoriaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> CreateAsync(Categoria categoria)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_CATEGORIA @Operacion={0}, @Nombre={1}, @Descripcion={2}, @filter={3}",
                     "INSERT",
                     categoria.Nombre,
                     categoria.Descripcion,
                     categoria.filter
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateAsync: {ex.Message}");
                return 0;
            }
        }

        public async  Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_CATEGORIA @Operacion={0}, @Id={1}",
                    "DELETE", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "GET");

                // Ejecuta el stored procedure y mapea a la entidad categoria
                var categorias = await _context.Categorias
                    .FromSqlRaw("EXEC SP_CATEGORIA @Operacion", paramOperacion)
                    .ToListAsync();

                return categorias;
            }
            catch (SqlException sqlEx)
            {
                // Loguea errores de SQL
                _logger.LogError(sqlEx, "Error ejecutando SP_categorias");
                return Enumerable.Empty<Categoria>();
            }
            catch (Exception ex)
            {
                // Loguea errores generales
                _logger.LogError(ex, "Error al obtener la lista de categorias");
                return Enumerable.Empty<Categoria>();
            }
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            try
            {
                var categorias = await _context.Categorias
                    .FromSqlRaw("EXEC SP_CATEGORIA @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();

                return categorias.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<int> UpdateAsync(Categoria categoria)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_CATEGORIA @Operacion={0},@Id={1}, @Nombre={2}, @Descripcion={3}, @filter={4}",
                     "UPDATE",
                     categoria.Id,
                     categoria.Nombre,
                     categoria.Descripcion,
                     categoria.filter
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<ComboCategoriaDto>> GetComboAsync()
        {
            var result = await _context.Database
                .SqlQueryRaw<ComboCategoriaDto>("EXEC SP_CATEGORIA @Operacion={0}", "COMBO")
                .ToListAsync();

            return result;
        }

    }
}
