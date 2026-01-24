using DocumentFormat.OpenXml.Math;
using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Core.DTO;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.ServiceMovil
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration, IPasswordHasher<Usuario> passwordHasher , ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<string?> LoginAsync(string usuarioNombre, string password)
        {
            var usuario = await _usuarioRepository.GetByUsuarioNombreAsync(usuarioNombre);

            if (usuario == null)
            {
                _logger.LogWarning($"Usuario {usuarioNombre} no encontrado");
                return null;
            }

            // Usar IPasswordHasher para verificar
            var passwordValid = await ValidarPasswordAsync(usuario, password);

            if (!passwordValid)
            {
                _logger.LogWarning($"Contraseña inválida para usuario {usuarioNombre}");
                return null;
            }

            // Generar token
            return GenerateToken(usuario);
        }

        public async Task<bool> ValidarPasswordAsync(Usuario usuario, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public string GenerateToken(Usuario usuario)
        {
            // Tu código de generación de token JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.UsuarioNombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("NombreCompleto", $"{usuario.Nombre} {usuario.Apellido}")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Método para hashear contraseñas (si necesitas crear usuarios)
        public string HashPassword(string password, Usuario usuario = null)
        {
            var user = usuario ?? new Usuario();
            return _passwordHasher.HashPassword(user, password);
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Verificar si el usuario ya existe
            var usuarioExistente = await _usuarioRepository.GetByUsuarioNombreAsync(registerDto.UsuarioNombre);
            if (usuarioExistente != null)
                return false; // Usuario ya existe

            // 2. Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                UsuarioNombre = registerDto.UsuarioNombre,
                Email = registerDto.Email,
                Password = HashPassword(registerDto.Password), // Hashear la contraseña
                Rol = registerDto.Rol,
                FechaCreacion = DateTime.UtcNow,
                Telefono = registerDto.Telefono,
                Ruc = registerDto.Ruc,
                Direccion = registerDto.Ruc,
                IsActive = true
            };

            // 3. Guardar en base de datos
            var resultado = await _usuarioRepository.AddMovilAsync(nuevoUsuario);

            // 4. Retornar true si se guardó correctamente
            return resultado != null;
        }


    }
}
