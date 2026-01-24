using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Application.Services;
using Farmaceutica.Core.DTO;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farmaceutica.Api.Controllers.Acceso
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public LoginController(IAuthService authService, IUsuarioRepository usuarioRepository, IConfiguration configuration )
        {
            _authService = authService;
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Core.DTO.LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.UsuarioNombre, dto.Password);

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            // Obtener información del usuario para la respuesta
            var usuario = await _usuarioRepository.GetByUsuarioNombreAsync(dto.UsuarioNombre);

            var response = new LoginResponseDto
            {
                Token = token,
                Expira = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                UsuarioNombre = usuario.UsuarioNombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            };

            return Ok(response);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                // Validación simple
                if (string.IsNullOrEmpty(dto.UsuarioNombre) ||
                    string.IsNullOrEmpty(dto.Password) ||
                    string.IsNullOrEmpty(dto.Email))
                {
                    return BadRequest(new { mensaje = "Datos incompletos" });
                }

                // Intentar registro
                var resultado = await _authService.RegisterAsync(dto);

                if (!resultado)
                    return BadRequest(new { mensaje = "El usuario ya existe" });

                return Ok(new { mensaje = "Usuario registrado exitosamente" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error interno" });
            }
        }

    }
}
