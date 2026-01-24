using DocumentFormat.OpenXml.Office2010.Excel;
using Farmaceutica.Application.Services;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Farmaceutica.Infrastructure.Migrations;
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
    public class VentaRepository : IventaRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<VentaRepository> _logger;
        public VentaRepository(AppFarmaceuticaContex context, ILogger<VentaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DetalleVentaDto> RegistrarDetalleVentaAsync(DetalleVentaDto detalle)
        {
            var pVentaId = new SqlParameter("@COMPR_ID", detalle.VentId);
            var pProductoId = new SqlParameter("@PROD_ID", detalle.ProdId);
            var pPrecio = new SqlParameter("@PROD_PCOMPRA", detalle.ProdPVenta);
            var pCantidad = new SqlParameter("@DETC_CANT", detalle.DetvCant);

            // Ejecutamos el SP
            await _context.Database
                .ExecuteSqlRawAsync(
                    "EXEC SP_I_COMPRA_02 @COMPR_ID, @PROD_ID, @PROD_PCOMPRA, @DETC_CANT",
                    pVentaId, pProductoId, pPrecio, pCantidad);

            return detalle;
        }

        public async Task<int> RegistrarVentaAsync(int usuId)
        {
            var param = new SqlParameter("@USU_ID", usuId);

            // Ejecutamos el SP y obtenemos el ID insertado
            var result = await _context.Database
                .SqlQueryRaw<int>("EXEC SP_I_COMPRA_01 @USU_ID", param)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public TotalesVentaDto CalcularTotalesConSP(int comprId)
        {
            // Usando FromSqlRaw para ejecutar el SP y mapear al DTO
            var resultado = _context.Set<TotalesVentaDto>()
                .FromSqlRaw("EXEC SP_U_COMPRA_01 @COMPR_ID = {0}", comprId)
                .AsEnumerable()
                .FirstOrDefault();

            if (resultado == null)
                throw new Exception("Venta no encontrada");

            return resultado;
        }

        public IEnumerable<DetalleVentaListarDto> GetVentaDetalle(int ventId)
        {
            var param = new SqlParameter("@COMPR_ID", ventId);

            // Keyless entity mapping
            return _context.Set<DetalleVentaListarDto>()
                .FromSqlRaw("EXEC SP_L_COMPRA_01 @COMPR_ID", param)
                .AsNoTracking()
                .ToList();
        }

        public async Task GuardarVentaAsync(VentaGuardarDto dto)
        {
            var parameters = new[]
            {
            new SqlParameter("@COMPR_ID", dto.VentId),
            new SqlParameter("@PAG_ID", dto.PagId),
            new SqlParameter("@PROV_ID", dto.CliId),
            new SqlParameter("@PROV_RUC", dto.CliRuc ?? (object)DBNull.Value),
            new SqlParameter("@PROV_DIRECC", dto.CliDirecc ?? (object)DBNull.Value),
            new SqlParameter("@PROV_CORREO", dto.CliCorreo ?? (object)DBNull.Value),
            new SqlParameter("@COMPR_COMENT", dto.VentComent ?? (object)DBNull.Value),
            new SqlParameter("@MON_ID", dto.MonId)
        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[SP_U_COMPRA_03] @COMPR_ID, @PAG_ID, @PROV_ID, @PROV_RUC, @PROV_DIRECC, @PROV_CORREO, @COMPR_COMENT, @MON_ID",
                parameters
            );
        }

        public async Task EliminarDetalleVentaAsync(int detv_id)
        {
            var param = new SqlParameter("@DETC_ID", detv_id);
            await _context.Database.ExecuteSqlRawAsync("EXEC SP_D_COMPRA_01 @DETC_ID", param);
            Console.WriteLine($"✅ SP_D_COMPRA_01 ejecutado para detv_id={detv_id}");
        }


        public Venta ObtenerVentaPorId(int id)
        {
            return _context.Ventas
                .FirstOrDefault(v => v.Id == id);
        }


        public List<DetalleVentaFacturaDto> ListarDetalleVenta(int ventaId)
        {
            return _context.DetalleVentas
                .Where(d => d.VentaId == ventaId && d.IsActive)
                .Select(d => new DetalleVentaFacturaDto
                {
                    Categoria = d.Producto.Categoria.Nombre,
                    ProductoNombre = d.Producto.Nombre,
                   
                    PrecioUnitario = d.Precio,
                    Cantidad = d.Cantidad,
                    TotalVenta = d.TotalVenta
                }).ToList();
        }

        

        public string? ObtenerNombrePorId(int EncargadoId)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Id == EncargadoId);

            return usuario?.Nombre; // Retorna null si no existe
        }

        public string? ObtenerNombrePorId(int? clienteId)
        {
            var usuario = _context.Usuarios
               .FirstOrDefault(u => u.Id == clienteId);

            return usuario?.Nombre; // Retorna null si no existe
        }

        public async Task<IEnumerable<VentaDto>> GetAllAsync()
        {
            try
            {
                // Parámetro seguro para el procedimiento almacenado
                var paramOperacion = new SqlParameter("@Operacion", "LISTAR");

                // Ejecuta el procedimiento almacenado y mapea directamente a VentaDto
                var ventas = await _context.Database
                    .SqlQueryRaw<VentaDto>("EXEC VENTA @Operacion", paramOperacion)
                    .ToListAsync();

                return ventas;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar el procedimiento VENTA");
                return Enumerable.Empty<VentaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al obtener la lista de ventas");
                return Enumerable.Empty<VentaDto>();
            }
        }


        public async Task<List<MostrarVentaDTO>> GetByIdAsync(int id)
        {
            try
            {
                var paramOperacion = new SqlParameter("@Operacion", "DETALLE");
                var paramVentaId = new SqlParameter("@Venta_id", id);

                var ventas = await _context.Database
                    .SqlQueryRaw<MostrarVentaDTO>(
                        "EXEC VENTA @Operacion, @Venta_id",
                        paramOperacion,
                        paramVentaId
                    )
                    .ToListAsync();

                return ventas; // Devuelve toda la lista
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar el procedimiento VENTA con Venta_id={id}", id);
                return new List<MostrarVentaDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al obtener los datos de venta");
                return new List<MostrarVentaDTO>();
            }
        }


        //public async Task<IEnumerable<ProductosVendidosDTO>> GetProductosVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin)
        //{
        //    // Definir el mínimo válido para SQL Server
        //    DateTime sqlMinDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

        //    // Validar fechas antes de enviar
        //    var validFechaInicio = (fechaInicio.HasValue && fechaInicio.Value >= sqlMinDate)
        //                           ? fechaInicio.Value
        //                           : (object)DBNull.Value;

        //    var validFechaFin = (fechaFin.HasValue && fechaFin.Value >= sqlMinDate)
        //                        ? fechaFin.Value
        //                        : (object)DBNull.Value;

        //    var paramInicio = new SqlParameter("@FechaInicio", validFechaInicio);
        //    var paramFin = new SqlParameter("@FechaFin", validFechaFin);
        //    var paramOperacion = new SqlParameter("@Operacion", "REPORTE");

        //    // Ejecutar SP
        //    return await _context.Database
        //        .SqlQueryRaw<ProductosVendidosDTO>("EXEC VENTA @Operacion,@FechaInicio, @FechaFin",
        //                                           paramOperacion, paramInicio, paramFin)
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<ProductosVendidosDTO>> GetProductosVendidosAsync(DateTime? fechaInicio, DateTime? fechaFin)
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
                .SqlQueryRaw<ProductosVendidosDTO>(
                    "EXEC VENTA @Operacion = @Operacion, @FechaInicio = @FechaInicio, @FechaFin = @FechaFin",
                    paramOperacion, paramFechaInicio, paramFechaFin)
                .ToListAsync();
        }

        public async Task RegistrarVentaAsync(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🕐 1️⃣ Registrar la venta en estado "pendiente"
                venta.FechaCreacion = DateTime.Now;
                venta.IsActive = 2;

                // Se quitan los detalles antes de guardar la venta
                var detallesTemp = venta.DetalleVentas?.ToList();
                venta.DetalleVentas = null;

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync(); // Se genera el ID de la venta

                // 🧾 2️⃣ Insertar los detalles con el ID de la venta recién creado
                if (detallesTemp != null && detallesTemp.Any())
                {
                    foreach (var detalle in detallesTemp)
                    {
                        detalle.Id = 0; // evitar conflicto con IDENTITY
                        detalle.VentaId = venta.Id;
                    }

                    await _context.DetalleVentas.AddRangeAsync(detallesTemp);
                    await _context.SaveChangesAsync();
                }

                // ✅ 3️⃣ Actualizar la venta a estado "confirmada"
                venta.IsActive = 1;
                _context.Ventas.Update(venta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al registrar la venta: " + ex.Message, ex);
            }
        }






    }
}
