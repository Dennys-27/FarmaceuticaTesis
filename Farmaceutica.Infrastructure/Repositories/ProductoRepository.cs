using DocumentFormat.OpenXml.Office2010.Excel;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.DTOs.Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Core.Temporales;
using Farmaceutica.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<ProductoRepository> _logger;

        public ProductoRepository(AppFarmaceuticaContex context, ILogger<ProductoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductoProcesarPredictivoDTO>> ConsultarProductosParaProcesar()
        {
            try
            {
                
                var paramOperacion = new SqlParameter("@Operacion", "VISUALIZAR");

                
                var productos = await _context.Database
                    .SqlQueryRaw<ProductoProcesarPredictivoDTO>("EXEC SP_PRODUCTO @Operacion", paramOperacion)
                    .ToListAsync();

                return productos; 
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error ejecutando SP_PRODUCTO");
                return Enumerable.Empty<ProductoProcesarPredictivoDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                return Enumerable.Empty<ProductoProcesarPredictivoDTO>();
            }
        }

        public async Task<int> CreateAsync(Producto producto)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_PRODUCTO @Operacion={0}, @Nombre={1}, @Descripcion={2}, @Precio={3},@StockTotal={4},@StockLocal={5},@StockDelivery={6},@CategoriaId={7},@SubCategoriaId={8},@Codigo={9}",
                     "INSERT",
                     producto.Nombre,
                     producto.Descripcion,
                     producto.Precio,
                     producto.StockTotal,
                     producto.StockLocal,
                     producto.StockDelivery,
                     producto.CategoriaId,
                     producto.SubCategoriaId,
                     producto.Codigo

                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_PRODUCTO @Operacion={0}, @Id={1}",
                    "DELETE", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<ProductoDTO>> GetAllAsync()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "GET");

                // Ejecuta el stored procedure y mapea al DTO
                var productos = await _context.Database
                    .SqlQueryRaw<ProductoDTO>("EXEC SP_PRODUCTO @Operacion", paramOperacion)
                    .ToListAsync();

                return productos; // 👈 corregido
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error ejecutando SP_PRODUCTO");
                return Enumerable.Empty<ProductoDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                return Enumerable.Empty<ProductoDTO>();
            }
        }


        public async Task<Producto?> GetByIdAsync(int id)
        {
            try
            {

                var producto = await _context.Productos
                    .FromSqlRaw("EXEC SP_PRODUCTO @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();


                return producto.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<DatosProcesados>> Procesar()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "PROCESAR");

                // Ejecuta el stored procedure y mapea al DTO
                var productos = await _context.Database
                    .SqlQueryRaw<DatosProcesados>("EXEC SP_PRODUCTO @Operacion", paramOperacion)
                    .ToListAsync();

                return productos; // 👈 corregido
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error ejecutando SP_PRODUCTO");
                return Enumerable.Empty<DatosProcesados>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                return Enumerable.Empty<DatosProcesados>();
            }
        }

        public async Task<int> UpdateAsync(ProductoImagen producto)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                     "EXEC SP_PRODUCTO @Operacion={0},@Id={1}, @Imagen={2}",
                     "UPDATE",
                     producto.Id,
                     producto.Imagen
                 );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<IEnumerable<ComboProductoCategoriaDto>> GetComboAsync(int categoria)
        {
            var result = await _context.Database
                 .SqlQueryRaw<ComboProductoCategoriaDto>("EXEC SP_PRODUCTO @Operacion={0}, @CategoriaId={1}", "COMBO", categoria)
                 .ToListAsync();

            return result;
        }

        // Solo UN método, no dos
        // En ProductoRepository.cs
        public async Task<IEnumerable<ProductoPrincipalDto>> ProductosPrincipal()
        {
            try
            {
                // OPCIÓN 1: Productos completamente aleatorios
                var productosAleatorios = await _context.Productos
                    .Where(p => p.StockTotal > 0)
                    .OrderBy(p => Guid.NewGuid()) // Orden aleatorio
                    .Take(10)
                    .Select(p => new ProductoPrincipalDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre ?? "Producto sin nombre",
                        Categoria = p.Categoria != null ? p.Categoria.Nombre : "General",
                        SubCategoria = p.SubCategoria != null ? p.SubCategoria.Nombre : "",
                        Descripcion = p.Descripcion ?? "Sin descripción disponible",
                        Imagen = p.Imagen ?? "/images/nft/img-05.jpg",
                        StockTotal = p.StockTotal,
                        Precio = p.Precio
                    })
                    .ToListAsync();

                return productosAleatorios;
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error en ProductosPrincipal (LINQ): {ex.Message}");

                // Retorna productos demo si hay error
                return GetProductosDemo();
            }
        }

        private List<ProductoPrincipalDto> GetProductosDemo()
        {
            return new List<ProductoPrincipalDto>
            {
                new() {
                    Id = 1,
                    Nombre = "Pañales Premium Húmedos",
                    Categoria = "Bebés",
                    SubCategoria = "Pañales",
                    Descripcion = "Pañales súper absorbentes para bebés, hipoalergénicos",
                    Imagen = "/images/nft/img-05.jpg",
                    StockTotal = 10,
                    Precio = 25.00m
                },
                new() {
                    Id = 2,
                    Nombre = "Jarabe para la Tos Infantil",
                    Categoria = "Medicamentos",
                    SubCategoria = "Jarabes",
                    Descripcion = "Alivia la tos seca y productiva en niños",
                    Imagen = "/images/nft/img-06.jpg",
                    StockTotal = 15,
                    Precio = 18.50m
                }
            };
        }
    }
}
