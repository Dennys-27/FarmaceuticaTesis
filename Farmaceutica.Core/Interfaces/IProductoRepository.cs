using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.DTOs.Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Temporales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<ProductoDTO>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<int> CreateAsync(Producto producto);
        Task<int> UpdateAsync(ProductoImagen producto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<ProductoProcesarPredictivoDTO>> ConsultarProductosParaProcesar();
        Task<IEnumerable<DatosProcesados>> Procesar();

        Task<IEnumerable<ComboProductoCategoriaDto>> GetComboAsync(int categoria);

        Task<IEnumerable<ProductoPrincipalDto>> ProductosPrincipal();

        Task<int> CountActiveAsync();
        Task<decimal> SumStockActiveAsync();
        Task<int> CountCreatedInPeriodAsync(DateTime inicio, DateTime fin);
        Task<int> CountActivePreviousMonthAsync();
        Task<decimal> GetTotalVentasAsync(DateTime inicio, DateTime fin);
        Task<decimal> GetTotalComprasAsync(DateTime inicio, DateTime fin);
        Task<int> CountPedidosPendientesAsync();
        Task<decimal> GetTotalPedidosPendientesAsync();


    }
}
