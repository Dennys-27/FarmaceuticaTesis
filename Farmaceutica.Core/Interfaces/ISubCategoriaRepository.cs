using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface ISubCategoriaRepository
    {
        Task<IEnumerable<SubCategoriaDTO>> GetAllAsync();
        Task<SubCategoria?> GetByIdAsync(int id);
        Task<int> CreateAsync(SubCategoria subcategoria);
        Task<int> UpdateAsync(SubCategoria subcategoria);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<ComboSubCategoriaDto>> GetComboAsync(int categoria);

       
    }
}
