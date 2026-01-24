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
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(AppFarmaceuticaContex context, ILogger<ClienteRepository> logger)
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
                var clientes = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_CLIENTES @Operacion", paramOperacion)
                    .ToListAsync();

                return clientes;
            }
            catch (SqlException sqlEx)
            {
                // Loguea errores de SQL
                _logger.LogError(sqlEx, "Error ejecutando SP_CLIENTES");
                return Enumerable.Empty<Usuario>();
            }
            catch (Exception ex)
            {
                // Loguea errores generales
                _logger.LogError(ex, "Error al obtener la lista de CLIENTES");
                return Enumerable.Empty<Usuario>();
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                var clientes = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_CLIENTES @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();

                return clientes.FirstOrDefault();
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
                    "EXEC SP_CLIENTES @Operacion={0}, @Id={1}",
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
               .SqlQueryRaw<ComboClienteDto>("EXEC SP_CLIENTES @Operacion={0}", "COMBO")
               .ToListAsync();

            return result;
        }

        public async Task<Cliente?> BuscarClientePorUsuarioAsync(string usuario)
        {
            if (string.IsNullOrEmpty(usuario))
                return null;

            try
            {
                using var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SP_CLIENTES";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var operacionParam = cmd.CreateParameter();
                operacionParam.ParameterName = "@Operacion";
                operacionParam.Value = "BUSCAR_POR_USUARIO";
                cmd.Parameters.Add(operacionParam);

                var correoParam = cmd.CreateParameter();
                correoParam.ParameterName = "@Email";
                correoParam.Value = usuario;
                cmd.Parameters.Add(correoParam);

                var telefonoParam = cmd.CreateParameter();
                telefonoParam.ParameterName = "@Telefono";
                telefonoParam.Value = usuario;
                cmd.Parameters.Add(telefonoParam);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    // Mapea los campos del SP al DTO
                    return new Cliente
                    {
                        cli_id = reader.GetInt32(reader.GetOrdinal("Id")),
                        cli_nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        cli_correo = reader.GetString(reader.GetOrdinal("Email")),
                        cli_direcc = reader.GetString(reader.GetOrdinal("Direccion")),
                        cli_ruc = reader.GetString(reader.GetOrdinal("Ruc")),
                        cli_telf = reader.GetString(reader.GetOrdinal("Telefono")),
                        // otros campos según tu DTO
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BuscarClientePorUsuarioAsync: {ex.Message}");
                return null;
            }
        }



    }
}
