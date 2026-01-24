using Farmaceutica.Core.Temporales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IProductoTempRepository
    {
        Task LimpiarAsync();
        Task InsertarRangoAsync(IEnumerable<ProductoTemp> productos);
    }
}
