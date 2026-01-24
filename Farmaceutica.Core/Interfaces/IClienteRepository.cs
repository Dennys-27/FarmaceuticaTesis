using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
       
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<ComboClienteDto>> GetComboAsync();
        Task<Cliente?> BuscarClientePorUsuarioAsync(string usuario);
    }
}
