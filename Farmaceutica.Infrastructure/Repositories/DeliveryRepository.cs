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
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<DeliveryRepository> _logger;

        public DeliveryRepository(AppFarmaceuticaContex context, ILogger<DeliveryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                // Parametro seguro para el SP
                var paramOperacion = new SqlParameter("@Operacion", "GET");

                // Ejecuta el stored procedure y mapea a la entidad Usuario
                var Deliberys = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_DELIVERY @Operacion", paramOperacion)
                    .ToListAsync();

                return Deliberys;
            }
            catch (SqlException sqlEx)
            {
                // Loguea errores de SQL
                _logger.LogError(sqlEx, "Error ejecutando SP_DeliberyS");
                return Enumerable.Empty<Usuario>();
            }
            catch (Exception ex)
            {
                // Loguea errores generales
                _logger.LogError(ex, "Error al obtener la lista de DeliberyS");
                return Enumerable.Empty<Usuario>();
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                var Deliberys = await _context.Usuarios
                    .FromSqlRaw("EXEC SP_DELIVERY @Operacion={0}, @Id={1}", "GETBYID", id)
                    .ToListAsync();

                return Deliberys.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_DELIVERY @Operacion={0}, @Id={1}",
                    "DELETE", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return 0;
            }
        }



        async Task<IEnumerable<DeliveryVentaDto>> IDeliveryRepository.GetVentaDelivery(DateTime? fechaInicio, DateTime? fechaFin)
        {
            DateTime sqlMinDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            var validFechaInicio = (fechaInicio.HasValue && fechaInicio.Value >= sqlMinDate)
                                   ? fechaInicio.Value
                                   : (object)DBNull.Value;

            var validFechaFin = (fechaFin.HasValue && fechaFin.Value >= sqlMinDate)
                                ? fechaFin.Value
                                : (object)DBNull.Value;

            var paramOperacion = new SqlParameter("@Operacion", "LISTAR");
            var paramFechaInicio = new SqlParameter("@FechaInicio", validFechaInicio);
            var paramFechaFin = new SqlParameter("@FechaFin", validFechaFin);

            return await _context.Database
                .SqlQueryRaw<DeliveryVentaDto>(
                    "EXEC DELIVERY @Operacion = @Operacion, @FechaInicio = @FechaInicio, @FechaFin = @FechaFin",
                    paramOperacion, paramFechaInicio, paramFechaFin)
                .ToListAsync();
        }

        public async Task<List<DeliveryVentaDeatelleDto>> MostrarVentaDetalle(int id)
        {
            try
            {
                var paramOperacion = new SqlParameter("@Operacion", "DETALLE");
                var paramVentaId = new SqlParameter("@IdVenta", id);

                // Primero convertir a IEnumerable, luego a List asíncrona
                var query = _context.Database
                    .SqlQueryRaw<DeliveryVentaDeatelleDto>(
                        "EXEC [fersoftw_Farmaceutica].[DELIVERY] @Operacion = @Operacion, @IdVenta = @IdVenta",
                        paramOperacion,
                        paramVentaId
                    );

                // Opción 1: Convertir a enumerable y luego a lista
                var result = query.AsEnumerable().ToList();
                return result;

                // O si necesitas async, puedes usar Task.Run (pero no es ideal)
                // return await Task.Run(() => query.AsEnumerable().ToList());
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar DELIVERY con Venta_id={id}", id);
                return new List<DeliveryVentaDeatelleDto>();
            }
        }




        public async Task<DeliveryVentaFacturaDto?> MostrarVentaFactura(int id)
        {
            try
            {
                var paramOperacion = new SqlParameter("@Operacion", "MOSTRAR");
                var paramVentaId = new SqlParameter("@IdVenta", id);

                // FORMA CORRECTA: Usar ToListAsync() o FirstOrDefaultAsync() directamente
                var result = await _context.Database
                    .SqlQueryRaw<DeliveryVentaFacturaDto>(
                        "EXEC [fersoftw_Farmaceutica].[DELIVERY] @Operacion = @Operacion, @IdVenta = @IdVenta",
                        paramOperacion,
                        paramVentaId
                    )
                    .ToListAsync(); // <- CAMBIA A ToListAsync()

                return result.FirstOrDefault();
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar VENTA con Venta_id={id}", id);
                return null;
            }
        }


        public async Task<bool> ProcesarDeliveryAsync(
     int idVenta,
     int repartidorId,
     int estadoDelivery,
     DateTime? fechaAsignacion = null,
     string comentario = "")
        {
            try
            {
                var pOperacion = new SqlParameter("@Operacion", "PROCESAR");
                var pIdVenta = new SqlParameter("@IdVenta", idVenta);
                var pRepartidorId = new SqlParameter("@RepartidorId", repartidorId);
                var pFechaAsignacion = new SqlParameter("@FechaAsignacion", fechaAsignacion ?? (object)DBNull.Value);
                var pEstadoDelivery = new SqlParameter("@EstadoDelivery", estadoDelivery);
                var pComentario = new SqlParameter("@ComentarioDelivery",
                    string.IsNullOrEmpty(comentario) ? (object)DBNull.Value : comentario);

                // ✅ SOLUCIÓN: Usar parámetros nombrados
                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC [fersoftw_Farmaceutica].[DELIVERY] 
                @Operacion = @Operacion,
                @IdVenta = @IdVenta,
                @RepartidorId = @RepartidorId,
                @FechaAsignacion = @FechaAsignacion,
                @EstadoDelivery = @EstadoDelivery,
                @ComentarioDelivery = @ComentarioDelivery",
                    pOperacion, pIdVenta, pRepartidorId, pFechaAsignacion, pEstadoDelivery, pComentario
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al PROCESAR delivery | VentaId={IdVenta} | RepartidorId={RepartidorId}",
                    idVenta, repartidorId);
                return false;
            }
        }



        public async Task<List<RepartidorDto>> ListarRepartidoresAsync()
        {
            try
            {
                var paramOperacion = new SqlParameter("@Operacion", "REPARTIDORES");

                return await _context.Database
                    .SqlQueryRaw<RepartidorDto>(
                        "EXEC DELIVERY @Operacion",
                        paramOperacion
                    )
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar repartidores");
                return new List<RepartidorDto>();
            }
        }

        

        public async Task<IEnumerable<DeliveryVentaDto>> GetVentaDeliveryPorId(DateTime? fechaInicio, DateTime? fechaFin, int id)
        {
            DateTime sqlMinDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            var validFechaInicio = (fechaInicio.HasValue && fechaInicio.Value >= sqlMinDate)
                                   ? fechaInicio.Value
                                   : (object)DBNull.Value;

            var validFechaFin = (fechaFin.HasValue && fechaFin.Value >= sqlMinDate)
                                ? fechaFin.Value
                                : (object)DBNull.Value;

            var paramOperacion = new SqlParameter("@Operacion", "LISTARID");
            var paramFechaInicio = new SqlParameter("@FechaInicio", validFechaInicio);
            var paramFechaFin = new SqlParameter("@FechaFin", validFechaFin);
            var paramId = new SqlParameter("@RepartidorId", id);

            return await _context.Database
                .SqlQueryRaw<DeliveryVentaDto>(
                    "EXEC DELIVERY @Operacion = @Operacion, @FechaInicio = @FechaInicio, @FechaFin = @FechaFin, @RepartidorId = @RepartidorId",
                    paramOperacion, paramFechaInicio, paramFechaFin, paramId)
                .ToListAsync();
        }


        public async Task<bool> ProcesarDeliveryIdAsync(
     int idVenta,
     
     int estadoDelivery
     
     )
        {
            try
            {
                var pOperacion = new SqlParameter("@Operacion", "PROCESO");
                var pIdVenta = new SqlParameter("@IdVenta", idVenta);
              
                
                var pEstadoDelivery = new SqlParameter("@EstadoDelivery", estadoDelivery);
                

                // ✅ SOLUCIÓN: Usar parámetros nombrados
                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC [fersoftw_Farmaceutica].[DELIVERY] 
                @Operacion = @Operacion,
                @IdVenta = @IdVenta,
                @EstadoDelivery = @EstadoDelivery
                ",
                    pOperacion, pIdVenta,  pEstadoDelivery
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al PROCESAR delivery | VentaId={IdVenta} | RepartidorId={RepartidorId}",
                    idVenta);
                return false;
            }
        }


    }
}
