using AutoMapper;
using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Core.DTO;
using Farmaceutica.Core.Interfaces.InterfacesMovil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.ServiceMovil
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoMovilRepository _productoRepository;
        private readonly IMapper _mapper;

        public ProductoService(IProductoMovilRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            var productos = await _productoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            return _mapper.Map<ProductoDto>(producto);
        }

        public async Task<IEnumerable<ProductoDto>> GetProductosByCategoriaAsync(int categoriaId)
        {
            var productos = await _productoRepository.GetByCategoriaAsync(categoriaId);
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }

        public async Task<IEnumerable<ProductoDto>> SearchProductosAsync(string searchTerm)
        {
            var productos = await _productoRepository.GetByNombreAsync(searchTerm);
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
    }
}
