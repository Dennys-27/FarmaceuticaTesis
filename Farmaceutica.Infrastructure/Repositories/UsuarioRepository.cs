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
using Microsoft.AspNetCore.Identity;


namespace Farmaceutica.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<UsuarioRepository> _logger;
        private readonly PasswordHasher<Usuario> _passwordHasher = new PasswordHasher<Usuario>();
        public UsuarioRepository(AppFarmaceuticaContex context, ILogger<UsuarioRepository> logger)
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
                var usuarios = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_USUARIOS @Operacion", paramOperacion)
                    .ToListAsync();

                return usuarios;
            }
            catch (SqlException sqlEx)
            {
                // Loguea errores de SQL
                _logger.LogError(sqlEx, "Error ejecutando SP_USUARIOS");
                return Enumerable.Empty<Usuario>();
            }
            catch (Exception ex)
            {
                // Loguea errores generales
                _logger.LogError(ex, "Error al obtener la lista de usuarios");
                return Enumerable.Empty<Usuario>();
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_USUARIOS @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();

                return usuarios.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CreateAsync(Usuario usuario)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_USUARIOS @Operacion={0}, @Nombre={1}, @UsuarioNombre={2}, @Email={3}, @Password={4}, @Rol={5}, @Apellido={6}, @Imagen={7},@Ruc={8},@Direccion={9},@Telefono={10}",
                     "INSERT",
                     usuario.Nombre,
                     usuario.UsuarioNombre,
                     usuario.Email,
                     usuario.Password,
                     (int)usuario.Rol,
                     usuario.Apellido,
                     usuario.Imagen,
                     usuario.Ruc,
                     usuario.Direccion,
                     usuario.Telefono
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(Usuario usuario)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_USUARIOS @Operacion={0},@Id={1}, @Nombre={2}, @UsuarioNombre={3}, @Email={4}, @Password={5}, @Rol={6}, @Apellido={7}, @Imagen={8}",
                     "UPDATE",
                     usuario.Id,
                     usuario.Nombre,
                     usuario.UsuarioNombre,
                     usuario.Email,
                     usuario.Password,
                     (int)usuario.Rol,
                     usuario.Apellido,
                     usuario.Imagen
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_USUARIOS @Operacion={0}, @Id={1}",
                    "DELETE", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioNombre == usuarioNombre);
        }

        public async Task<bool> ValidarPasswordAsync(Usuario usuario, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioNombre== username);
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == correo);
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ObtenerPorTokenAsync(string token)
        {
            var tokenRec = await _context.TokensRecuperacion
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Token == token && !t.Usado && t.FechaExpiracion > DateTime.UtcNow);

            return tokenRec?.Usuario;
        }

        public async Task GuardarTokenAsync(TokenRecuperacion token)
        {
            _context.TokensRecuperacion.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> AddMovilAsync(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return usuario; // ¡IMPORTANTE! Retorna el usuario
            }
            catch (Exception ex)
            {
                // Log del error si quieres
                return null;
            }
        }
    }
}
