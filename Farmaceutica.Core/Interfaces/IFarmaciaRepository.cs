using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Interfaces
{
    public interface IFarmaciaRepository
    {
        Task<IEnumerable<ProductoDTO>> GetAllAsync();
    }
}
