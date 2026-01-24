using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppFarmaceuticaContex _context;
        private readonly ILogger<PedidoRepository> _logger;


        public PedidoRepository(AppFarmaceuticaContex context, ILogger<PedidoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<PedidoPorCliente>> GetPedidoDeliveryPorCorreo(DateTime? fechaInicio, DateTime? fechaFin, string correo)
        {
            // Prepara los parámetros con tipos explícitos
            var paramOperacion = new SqlParameter("@Operacion", SqlDbType.VarChar, 20)
            {
                Value = "LISTAR"
            };

            var paramFechaInicio = new SqlParameter("@FechaInicio", SqlDbType.DateTime)
            {
                Value = fechaInicio ?? (object)DBNull.Value
            };

            var paramFechaFin = new SqlParameter("@FechaFin", SqlDbType.DateTime)
            {
                Value = fechaFin ?? (object)DBNull.Value
            };

            // Parámetros que no usas pero el SP espera
            var paramIdVenta = new SqlParameter("@IdVenta", SqlDbType.Int)
            {
                Value = DBNull.Value
            };

            var paramClienteId = new SqlParameter("@ClienteId", SqlDbType.Int)
            {
                Value = DBNull.Value  // ¡IMPORTANTE! Esto es INT, no string
            };

            var paramFechaAsignacion = new SqlParameter("@FechaAsignacion", SqlDbType.DateTime)
            {
                Value = DBNull.Value
            };

            var paramEstadoDelivery = new SqlParameter("@EstadoDelivery", SqlDbType.Int)
            {
                Value = DBNull.Value
            };

            var paramComentarioDelivery = new SqlParameter("@ComentarioDelivery", SqlDbType.VarChar, 500)
            {
                Value = DBNull.Value
            };

            // El parámetro de Email (posición 9)
            var paramEmail = new SqlParameter("@Email", SqlDbType.VarChar, 500)
            {
                Value = string.IsNullOrEmpty(correo) ? (object)DBNull.Value : correo
            };

            // Ejecuta en el orden CORRECTO de los parámetros del SP
            return await _context.Database
                .SqlQueryRaw<PedidoPorCliente>(
                    "EXEC [fersoftw_Farmaceutica].[PEDIDO] " +
                    "@Operacion, @FechaInicio, @FechaFin, @IdVenta, @ClienteId, " +
                    "@FechaAsignacion, @EstadoDelivery, @ComentarioDelivery, @Email",
                    paramOperacion,    // 1
                    paramFechaInicio,  // 2
                    paramFechaFin,     // 3
                    paramIdVenta,      // 4
                    paramClienteId,    // 5 ← Si pasas string aquí, causa el error
                    paramFechaAsignacion,   // 6
                    paramEstadoDelivery,    // 7
                    paramComentarioDelivery,// 8
                    paramEmail)             // 9
                .ToListAsync();
        }

        public async Task<IEnumerable<PedidoPorCliente>> GetVentaPedidoPorId(DateTime? fechaInicio, DateTime? fechaFin, int id)
        {
            DateTime sqlMinDate = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            var validFechaInicio = (fechaInicio.HasValue && fechaInicio.Value >= sqlMinDate)
                                   ? fechaInicio.Value
                                   : (object)DBNull.Value;

            var validFechaFin = (fechaFin.HasValue && fechaFin.Value >= sqlMinDate)
                                ? fechaFin.Value
                                : (object)DBNull.Value;

            // Crear TODOS los parámetros que el SP espera
            var paramOperacion = new SqlParameter("@Operacion", "LISTARPORUSUARIO");
            var paramFechaInicio = new SqlParameter("@FechaInicio", validFechaInicio);
            var paramFechaFin = new SqlParameter("@FechaFin", validFechaFin);
            var paramIdVenta = new SqlParameter("@IdVenta", DBNull.Value);
            var paramClienteId = new SqlParameter("@ClienteId", id);
            var paramFechaAsignacion = new SqlParameter("@FechaAsignacion", DBNull.Value);
            var paramEstadoDelivery = new SqlParameter("@EstadoDelivery", DBNull.Value);
            var paramComentarioDelivery = new SqlParameter("@ComentarioDelivery", DBNull.Value);
            var paramEmail = new SqlParameter("@Email", DBNull.Value);

            return await _context.Database
                .SqlQueryRaw<PedidoPorCliente>(
                    // Enviar TODOS los parámetros en el orden correcto
                    "EXEC [fersoftw_Farmaceutica].[PEDIDO] " +
                    "@Operacion, @FechaInicio, @FechaFin, @IdVenta, @ClienteId, " +
                    "@FechaAsignacion, @EstadoDelivery, @ComentarioDelivery, @Email",
                    paramOperacion,          // 1
                    paramFechaInicio,        // 2
                    paramFechaFin,           // 3
                    paramIdVenta,            // 4
                    paramClienteId,          // 5 ← Este es el parámetro correcto
                    paramFechaAsignacion,    // 6
                    paramEstadoDelivery,     // 7
                    paramComentarioDelivery, // 8
                    paramEmail)              // 9
                .ToListAsync();
        }
    }
}
