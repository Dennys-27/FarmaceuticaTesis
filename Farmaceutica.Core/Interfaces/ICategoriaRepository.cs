using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<int> CreateAsync(Categoria categoria);
        Task<int> UpdateAsync(Categoria categoria);
        Task<int> DeleteAsync(int id);

        Task<IEnumerable<ComboCategoriaDto>> GetComboAsync();
    }
}
