using DocumentFormat.OpenXml.Office2010.Excel;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
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
    public class CompraRepository : ICompraRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<CompraRepository> _logger;

        public CompraRepository(AppFarmaceuticaContex context, ILogger<CompraRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> RegistrarCompraAsync(int usuId)
        {
            var param = new SqlParameter("@USU_ID", usuId);
            var paramOperacion = new SqlParameter("@Operacion", "REGISTRAR_COMPRA");
            // Ejecutamos el SP y obtenemos el ID insertado
            var result = await _context.Database
                .SqlQueryRaw<int>("EXEC SP_I_COMPRA_PROCESO_01 @USU_ID, @Operacion", param, paramOperacion)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<DetalleCompraDto> RegistrarDetalleCompraAsync(DetalleCompraDto detalle)
        {
            // @USU_ID no se usa en esta operación
            var pUsuId = new SqlParameter("@USU_ID", DBNull.Value);

            // Operación debe coincidir con tu SP
            var paramOperacion = new SqlParameter("@Operacion", "REGISTRAR");

            var pCompraId = new SqlParameter("@COMPR_ID", detalle.ComprId);
            var pProductoId = new SqlParameter("@PROD_ID", detalle.ProdId);
            var pPrecio = new SqlParameter("@PROD_PCOMPRA", detalle.ProdPCompra);
            var pCantidad = new SqlParameter("@DETC_CANT", detalle.DetcCant);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SP_I_COMPRA_PROCESO_01 @USU_ID, @Operacion, @COMPR_ID, @PROD_ID, @PROD_PCOMPRA, @DETC_CANT",
                pUsuId, paramOperacion, pCompraId, pProductoId, pPrecio, pCantidad
            );

            return detalle;
        }


        public TotalesCompraDto CalcularTotalesConSP(int comprId)
        {
            var pUsuId = new SqlParameter("@USU_ID", DBNull.Value);
            var paramOperacion = new SqlParameter("@Operacion", "CALCULO");
            var pCompraId = new SqlParameter("@COMPR_ID", comprId);

            var resultado = _context.Set<TotalesCompraDto>()
                .FromSqlRaw(
                    "EXEC SP_I_COMPRA_PROCESO_01 @USU_ID, @Operacion, @COMPR_ID",
                    pUsuId, paramOperacion, pCompraId
                )
                .AsEnumerable()
                .FirstOrDefault();

            if (resultado == null)
                throw new Exception("Compra no encontrada");

            return resultado;
        }


        public IEnumerable<DetalleCompraListarDto> GetCompraDetalle(int comprId)
        {
            // @USU_ID no se usa, se pasa NULL
            var pUsuId = new SqlParameter("@USU_ID", DBNull.Value);

            // Operación que coincide con tu SP
            var paramOperacion = new SqlParameter("@Operacion", "LISTAR");

            // ID de la compra que quieres consultar
            var pCompraId = new SqlParameter("@COMPR_ID", comprId);

            return _context.Set<DetalleCompraListarDto>()
                .FromSqlRaw(
                    "EXEC SP_I_COMPRA_PROCESO_01 @USU_ID, @Operacion, @COMPR_ID",
                    pUsuId, paramOperacion, pCompraId
                )
                .AsNoTracking()
                .ToList();
        }

        public async Task EliminarDetalleCompraAsync(int detc_id)
        {
            // @USU_ID no se usa, se pasa NULL
            var pUsuId = new SqlParameter("@USU_ID", DBNull.Value);

            // Operación que coincide con tu SP
            var paramOperacion = new SqlParameter("@Operacion", "ELIMINAR");
            var param = new SqlParameter("@DETC_ID", detc_id);
            await _context.Database.ExecuteSqlRawAsync("EXEC SP_I_COMPRA_PROCESO_01 @USU_ID, @Operacion, @DETC_ID", pUsuId,paramOperacion, param);
            Console.WriteLine($"✅ SP_D_COMPRA_01 ejecutado para detv_id={detc_id}");
        }

        public async Task GuardarCompraAsync(CompraGuardarDto dto)
        {
            var parameters = new[]
            {
            new SqlParameter("@COMPR_ID", dto.ComprId),
            new SqlParameter("@PAG_ID", dto.PagId),
            new SqlParameter("@PROV_ID", dto.ProvId),
            new SqlParameter("@PROV_RUC", dto.ProvRuc?? (object)DBNull.Value),
            new SqlParameter("@PROV_DIRECC", dto.ProvDirecc ?? (object)DBNull.Value),
            new SqlParameter("@PROV_CORREO", dto.ProvCorreo ?? (object)DBNull.Value),
            new SqlParameter("@COMPR_COMENT", dto.ComprComent?? (object)DBNull.Value),
            new SqlParameter("@MON_ID", dto.MonId)
        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[SP_U_COMPRA_03_PROCESO] @COMPR_ID, @PAG_ID, @PROV_ID, @PROV_RUC, @PROV_DIRECC, @PROV_CORREO, @COMPR_COMENT, @MON_ID",
                parameters
            );
        }

        public Compra ObtenerCompraPorId(int id)
        {
            return _context.Compras
                .FirstOrDefault(v => v.Id == id);
        }


        public List<DetalleCompraFacturaDto> ListarDetalleCompra(int compraId)
        {
            return _context.DetalleCompra
                .Where(d => d.CompraId == compraId && d.IsActive)
                .Select(d => new DetalleCompraFacturaDto
                {
                    Categoria = d.Producto.Categoria.Nombre,
                    ProductoNombre = d.Producto.Nombre,

                    PrecioUnitario = d.Precio,
                    Cantidad = d.Cantidad,
                    TotalCompra = d.TotalCompra
                }).ToList();
        }



        public string? ObtenerNombrePorId(int EncargadoId)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Id == EncargadoId);

            return usuario?.Nombre; // Retorna null si no existe
        }

        public string? ObtenerNombrePorId(int? proveedorId)
        {
            var usuario = _context.Usuarios
               .FirstOrDefault(u => u.Id == proveedorId);

            return usuario?.Nombre; // Retorna null si no existe
        }

        public async Task<IEnumerable<CompraDto>> GetAllAsync()
        {
            try
            {
                // Parámetro seguro para el procedimiento almacenado
                var paramOperacion = new SqlParameter("@Operacion", "LISTAR");

                // Ejecuta el procedimiento almacenado y mapea directamente a CompraDto
                var compras = await _context.Database
                    .SqlQueryRaw<CompraDto>("EXEC COMPRA @Operacion", paramOperacion)
                    .ToListAsync();

                return compras;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar el procedimiento COMPRA");
                return Enumerable.Empty<CompraDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al obtener la lista de compras");
                return Enumerable.Empty<CompraDto>();
            }
        }


        

         async Task<IEnumerable<MostrarCompraDTO>> ICompraRepository.GetByIdAsync(int numeroFactura)
        {
            try
            {
                var paramOperacion = new SqlParameter("@Operacion", "DETALLE");
                var paramCompraId = new SqlParameter("@Compra_id", numeroFactura);

                var compras = await _context.Database
                    .SqlQueryRaw<MostrarCompraDTO>(
                        "EXEC COMPRA @Operacion, @Compra_id",
                        paramOperacion,
                        paramCompraId
                    )
                    .ToListAsync();

                return compras; // Devuelve toda la lista
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar el procedimiento COMPRA con Compra_id={id}", numeroFactura);
                return new List<MostrarCompraDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al obtener los datos de venta");
                return new List<MostrarCompraDTO>();
            }
        }

        public async Task<IEnumerable<ProductosCompradosDTO>> GetProductosCompradosAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            DateTime sqlMinDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            var validFechaInicio = (fechaInicio.HasValue && fechaInicio.Value >= sqlMinDate)
                                   ? fechaInicio.Value
                                   : (object)DBNull.Value;

            var validFechaFin = (fechaFin.HasValue && fechaFin.Value >= sqlMinDate)
                                ? fechaFin.Value
                                : (object)DBNull.Value;

            var paramOperacion = new SqlParameter("@Operacion", "REPORTE");
            var paramFechaInicio = new SqlParameter("@FechaInicio", validFechaInicio);
            var paramFechaFin = new SqlParameter("@FechaFin", validFechaFin);

            return await _context.Database
                .SqlQueryRaw<ProductosCompradosDTO>(
                    "EXEC COMPRA @Operacion = @Operacion, @FechaInicio = @FechaInicio, @FechaFin = @FechaFin",
                    paramOperacion, paramFechaInicio, paramFechaFin)
                .ToListAsync();
        }
    }
}
