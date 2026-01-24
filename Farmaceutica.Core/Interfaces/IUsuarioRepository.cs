using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<int> CreateAsync(Usuario usuario);
        Task<int> UpdateAsync(Usuario usuario);
        Task<int> DeleteAsync(int id);
        Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre);
        Task<bool> ValidarPasswordAsync(Usuario usuario, string password);

        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByUsernameAsync(string username);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorCorreoAsync(string correo);
        Task<Usuario?> ObtenerPorTokenAsync(string token);
        Task AddAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);

        Task GuardarTokenAsync(TokenRecuperacion token);

        Task<Usuario?> AddMovilAsync(Usuario usuario);

    }
}
