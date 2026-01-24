using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces.InterfacesMovil
{
    public interface IProductoMovilRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<IEnumerable<Producto>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<Producto>> GetByNombreAsync(string nombre);
        Task<IEnumerable<Producto>> GetProductosConStockAsync();
        Task<IEnumerable<Producto>> GetMasVendidosAsync(int cantidad = 10);
        //Task UpdateAsync(Producto producto);



        // Métodos para actualizar
        Task<bool> UpdateStockAsync(int productoId, int cantidadVendida);
        Task<bool> UpdateAsync(Producto producto);
    }
}
