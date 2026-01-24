using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class ObtenerDashboardResumenUseCase
    {
        private readonly IDashboardRepository _repository;

        public ObtenerDashboardResumenUseCase(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public DashboardResumenDto Ejecutar()
        {
            return _repository.ObtenerResumen();
        }
    }
}
