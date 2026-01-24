using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Farmaceutica.Infrastructure.Migrations;
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
    public class SubCategoriaRepository : ISubCategoriaRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<SubCategoriaRepository> _logger;

        public SubCategoriaRepository(AppFarmaceuticaContex context, ILogger<SubCategoriaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> CreateAsync(SubCategoria subcategoria)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_SUBCATEGORIA @Operacion={0}, @Nombre={1}, @Descripcion={2}, @filter={3},@CategoriaId={4}",
                     "INSERT",
                     subcategoria.Nombre,
                     subcategoria.Descripcion,
                     subcategoria.Filter,
                     subcategoria.CategoriaId
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_SUBCATEGORIA @Operacion={0}, @Id={1}",
                    "DELETE", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<SubCategoriaDTO>> GetAllAsync()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "GET");

                // Ejecuta el stored procedure y mapea al DTO
                var subcategorias = await _context.Database
                    .SqlQueryRaw<SubCategoriaDTO>("EXEC SP_SUBCATEGORIA @Operacion", paramOperacion)
                    .ToListAsync();

                return subcategorias; // 👈 corregido
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error ejecutando SP_SUBCATEGORIA");
                return Enumerable.Empty<SubCategoriaDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de subcategorias");
                return Enumerable.Empty<SubCategoriaDTO>();
            }
        }


        public async Task<SubCategoria?> GetByIdAsync(int id)
        {
            try
            {

                var subcategoria = await _context.SubCategorias
                    .FromSqlRaw("EXEC SP_SUBCATEGORIA @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();


                return subcategoria.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<int> UpdateAsync(SubCategoria subcategoria)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_SUBCATEGORIA @Operacion={0},@Id={1}, @Nombre={2}, @Descripcion={3}, @filter={4}, @CategoriaId={5}",
                     "UPDATE",
                     subcategoria.Id,
                     subcategoria.Nombre,
                     subcategoria.Descripcion,
                     subcategoria.Filter,
                     subcategoria.CategoriaId
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<ComboSubCategoriaDto>> GetComboAsync(int categoria)
        {
            var result = await _context.Database
                 .SqlQueryRaw<ComboSubCategoriaDto>("EXEC SP_SUBCATEGORIA @Operacion={0}, @CategoriaId={1}", "COMBO", categoria)
                 .ToListAsync();

            return result;
        }

       
    }
}
