using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;
        private readonly AppFarmaceuticaContex _context;
        public DashboardRepository(IConfiguration config, AppFarmaceuticaContex context)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _context = context;
        }

       

        public DashboardResumenDto ObtenerResumen()
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_dashboard_resumen", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (!dr.Read()) return new DashboardResumenDto();

            return new DashboardResumenDto
            {
                TotalProductos = dr.GetInt32(0),
                TotalVentas = dr.IsDBNull(1) ? 0 : dr.GetDecimal(1),
                PorcentajeCrecimiento = dr.IsDBNull(2) ? 0 : dr.GetDecimal(2)
            };
        }

        public async Task<List<DashboardVentaDTO>> ObtenerVentasRecientes()
        {
            var paramOperacion = new SqlParameter("@Operacion", "VENTAS");
            return await _context.Database
                .SqlQueryRaw<DashboardVentaDTO>(
                    "[fersoftw_Farmaceutica].[SP_DASHBOARD] @Operacion = @Operacion",
                    paramOperacion)
                .ToListAsync();
        }

        public async Task<List<DashboardPedidoDTO>> ObtenerPedidosPendientes()
        {
            var paramOperacion = new SqlParameter("@Operacion", "PEDIDOS");
            return await _context.Database
                .SqlQueryRaw<DashboardPedidoDTO>(
                    "EXEC [fersoftw_Farmaceutica].[SP_DASHBOARD] @Operacion = @Operacion",
                    paramOperacion)
                .ToListAsync();
        }

        public async Task<List<DashboardProductoDTO>> ObtenerProductosMasVendidos()
        {
            var paramOperacion = new SqlParameter("@Operacion", "PRODUCTOS");
            return await _context.Database
                .SqlQueryRaw<DashboardProductoDTO>(
                    "EXEC [fersoftw_Farmaceutica].[SP_DASHBOARD] @Operacion = @Operacion",
                    paramOperacion)
                .ToListAsync();
        }
    }
}
