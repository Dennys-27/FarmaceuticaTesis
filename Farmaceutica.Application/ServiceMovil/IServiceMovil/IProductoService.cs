using Farmaceutica.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.ServiceMovil.IServiceMovil
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetAllProductosAsync();
        Task<ProductoDto?> GetProductoByIdAsync(int id);
        Task<IEnumerable<ProductoDto>> GetProductosByCategoriaAsync(int categoriaId);
        Task<IEnumerable<ProductoDto>> SearchProductosAsync(string searchTerm);
    }
}
