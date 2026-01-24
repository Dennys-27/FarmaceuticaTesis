using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces.InterfacesMovil;
using Farmaceutica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories.RepositoriesMovil
{
    public class ProductoMovilRepository : IProductoMovilRepository
    {
        private readonly AppFarmaceuticaContex _context;

        public ProductoMovilRepository(AppFarmaceuticaContex context)
        {
            _context = context;
        }

        // ✅ Listar todos los productos
        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .Where(p => p.IsActive) // Solo productos activos
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // ✅ Obtener por ID
        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        // ✅ Buscar por categoría
        public async Task<IEnumerable<Producto>> GetByCategoriaAsync(int categoriaId)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .Where(p => p.CategoriaId == categoriaId && p.IsActive)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // ✅ Buscar por nombre (para búsqueda)
        public async Task<IEnumerable<Producto>> GetByNombreAsync(string nombre)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .Where(p => p.Nombre.Contains(nombre) && p.IsActive)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // ✅ Productos con stock disponible
        public async Task<IEnumerable<Producto>> GetProductosConStockAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .Where(p => p.StockTotal > 0 && p.IsActive)
                .OrderByDescending(p => p.StockTotal)
                .ToListAsync();
        }

        // ✅ Productos más vendidos (ejemplo básico)
        public async Task<IEnumerable<Producto>> GetMasVendidosAsync(int cantidad = 10)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.SubCategoria)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.DetalleVentas.Count) // O por cantidad vendida si tienes
                .Take(cantidad)
                .ToListAsync();
        }

        // En ProductoRepository.cs, agrega:




        // ✅ Versión 1: Método asíncrono que retorna Task<bool>
        public async Task<bool> UpdateAsync(Producto producto)
        {
            try
            {
                _context.Productos.Update(producto);
                var result = await _context.SaveChangesAsync();
                return result > 0; // Retorna true si se guardaron cambios
            }
            catch (Exception)
            {
                return false; // Retorna false si hubo error
            }
        }

        // ✅ Versión 2: Si solo quieres guardar sin retornar nada
        public async Task UpdateProductoAsync(Producto producto)
        {
            try
            {
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Puedes lanzar la excepción o manejarla
                throw new Exception($"Error al actualizar producto: {ex.Message}");
            }
        }

        // ✅ Versión 3: Método síncrono (no recomendado para APIs)
        public bool Update(Producto producto)
        {
            try
            {
                _context.Productos.Update(producto);
                var result = _context.SaveChanges();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> UpdateStockAsync(int productoId, int cantidadVendida)
        {
            throw new NotImplementedException();
        }
    }
 }
