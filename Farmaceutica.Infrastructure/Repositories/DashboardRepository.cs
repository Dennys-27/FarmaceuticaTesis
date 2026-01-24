using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Microsoft.Data.SqlClient;
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

        public DashboardRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
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
    }
}
