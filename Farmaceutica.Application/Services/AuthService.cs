using Farmaceutica.Application.IServices;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class AuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly PasswordHasher<Usuario> _hasher;
        private readonly IEmailService _emailService; // ✅ Cambiado a IEmailService
        private readonly ILogger<AuthService> _logger; // ✅ Agregado ILogger

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IEmailService emailService, // ✅ Cambiado a IEmailService
            ILogger<AuthService> logger) // ✅ Agregado logger
        {
            _hasher = new PasswordHasher<Usuario>();
            _usuarioRepository = usuarioRepository;
            _emailService = emailService; // ✅
            _logger = logger; // ✅
        }

        public async Task<Usuario?> LoginAsync(string usuarioNombre, string password)
        {
            var usuario = await _usuarioRepository.GetByUsuarioNombreAsync(usuarioNombre);
            if (usuario == null) return null;

            var valid = await _usuarioRepository.ValidarPasswordAsync(usuario, password);
            return valid ? usuario : null;
        }

        public async Task<string?> LoginAsync(string email, string username, string password, string nombre, string apellido, string telefono, string ruc, string direccion)
        {
            // Verificar si ya existe el email o username
            if (await _usuarioRepository.GetByEmailAsync(email) != null)
                return "El correo ya está registrado";

            if (await _usuarioRepository.GetByUsernameAsync(username) != null)
                return "El usuario ya existe";

            var usuario = new Usuario
            {
                Email = email,
                UsuarioNombre = username,
                Rol = Core.Enums.RolUsuario.Cliente, // Por defecto es Cliente
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                Ruc = ruc,
                Direccion = direccion
            };

            // Hashear la contraseña
            usuario.Password = _hasher.HashPassword(usuario, password);

            await _usuarioRepository.AddAsync(usuario);

            return null; // null significa éxito
        }

        public async Task<bool> RestaurarContraseniaAsync(int usuarioId, string nuevaContrasenia, string confirmarContrasenia)
        {
            if (nuevaContrasenia != confirmarContrasenia)
                throw new ArgumentException("Las contraseñas no coinciden.");

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            usuario.Password = _hasher.HashPassword(usuario, nuevaContrasenia);

            await _usuarioRepository.ActualizarAsync(usuario);
            return true;
        }

        public async Task EnviarCorreoRecuperacionAsync(string correo)
        {
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);
            if (usuario == null)
            {
                // Por seguridad, no revelamos si el email existe o no
                _logger.LogInformation($"Solicitud de recuperación para email no registrado: {correo}");
                return;
            }

            // Generar token más seguro
            var token = Guid.NewGuid().ToString("N");

            // Guardar token en base de datos
            var tokenRec = new TokenRecuperacion
            {
                Token = token,
                UsuarioId = usuario.Id,
                FechaExpiracion = DateTime.UtcNow.AddHours(1),
                Usado = false
            };

            await _usuarioRepository.GuardarTokenAsync(tokenRec);

            try
            {
                // ✅ Ahora usa la interfaz IEmailService
                await _emailService.SendPasswordResetEmail(correo, token);

                // Log de éxito
                _logger.LogInformation($"✅ Correo de recuperación enviado a: {correo}");
                _logger.LogInformation($"🔐 Token generado: {token}");
            }
            catch (Exception ex)
            {
                // Log del error pero no lanzamos excepción para no revelar información
                _logger.LogError(ex, $"❌ Error al enviar correo de recuperación a: {correo}");

                // Aquí puedes decidir si quieres lanzar una excepción genérica
                // o simplemente registrar el error y continuar
                throw new Exception("Ocurrió un error al procesar tu solicitud. Por favor, intenta más tarde.");
            }
        }

        public async Task<bool> RestaurarContraseniaAsync(string token, string nuevaContrasenia, string confirmarContrasenia)
        {
            if (nuevaContrasenia != confirmarContrasenia)
                throw new Exception("Las contraseñas no coinciden.");

            var usuario = await _usuarioRepository.ObtenerPorTokenAsync(token);
            if (usuario == null)
                throw new Exception("Token inválido o expirado.");

            usuario.Password = _hasher.HashPassword(usuario, nuevaContrasenia);
            await _usuarioRepository.ActualizarAsync(usuario);

            _logger.LogInformation($"✅ Contraseña restaurada para usuario ID: {usuario.Id}");
            return true;
        }
    }
}