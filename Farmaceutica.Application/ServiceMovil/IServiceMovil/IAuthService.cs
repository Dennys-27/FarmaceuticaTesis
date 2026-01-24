using Farmaceutica.Core.DTO;
using Farmaceutica.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.ServiceMovil.IServiceMovil
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string usuarioNombre, string password);
        string GenerateToken(Usuario usuario);
        Task<bool> RegisterAsync(RegisterDto registerDto); // Método simple
    }
}
