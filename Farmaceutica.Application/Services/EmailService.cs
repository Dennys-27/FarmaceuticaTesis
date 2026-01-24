using Farmaceutica.Application.IServices;
using Farmaceutica.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public async Task SendPasswordResetEmail(string email, string resetToken)
        {
            Exception lastException = null;

            // Lista de configuraciones a probar (comunes en Plesk)
            var configsToTry = new[]
            {
        new { Host = "mail.fersoftweb.com", Port = 587, EnableSsl = true, Description = "mail. + 587 SSL" },
        new { Host = "mail.fersoftweb.com", Port = 465, EnableSsl = true, Description = "mail. + 465 SSL" },
        new { Host = "fersoftweb.com", Port = 587, EnableSsl = true, Description = "sin mail. + 587 SSL" },
        new { Host = "fersoftweb.com", Port = 465, EnableSsl = true, Description = "sin mail. + 465 SSL" },
        new { Host = "179.61.12.165", Port = 25, EnableSsl = false, Description = "IP + 25 sin SSL" }, // Tu IP del servidor
        new { Host = "localhost", Port = 25, EnableSsl = false, Description = "localhost + 25" }
    };

            foreach (var config in configsToTry)
            {
                try
                {
                    _logger.LogInformation($"🔧 Probando: {config.Description} ({config.Host}:{config.Port})");

                    await SendEmailWithConfig(email, resetToken, config.Host, config.Port, config.EnableSsl);

                    _logger.LogInformation($"✅ ¡ÉXITO con {config.Description}!");
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning($"❌ Falló {config.Description}: {ex.Message}");
                }
            }

            // Si todas fallan
            _logger.LogError(lastException, "❌ Todas las configuraciones SMTP fallaron");

            // Fallback: mostrar token en consola
            var request = _httpContextAccessor.HttpContext?.Request;
            var host = request?.Host.ToString() ?? "localhost:7101";
            var scheme = request?.Scheme ?? "https";
            var resetUrl = $"{scheme}://{host}/Acceso/RestaurarConstrasenia?token={resetToken}";

            _logger.LogInformation($"📧 TOKEN FALLBACK para {email}: {resetToken}");
            _logger.LogInformation($"🔗 URL: {resetUrl}");

            throw new Exception($"No se pudo enviar el correo. Usa este enlace: {resetUrl}");
        }

        private async Task SendEmailWithConfig(string email, string resetToken, string host, int port, bool enableSsl)
        {
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Pass"];

            // Construir URL
            var request = _httpContextAccessor.HttpContext?.Request;
            var appHost = request?.Host.ToString() ?? "localhost:7101";
            var scheme = request?.Scheme ?? "https";
            var resetUrl = $"{scheme}://{appHost}/Acceso/RestaurarConstrasenia?token={resetToken}";

            // Crear mensaje
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "Farmacia La Puna"),
                Subject = "Restablecimiento de Contraseña",
                Body = $@"
            <h3>Restablecer Contraseña</h3>
            <p>Haz clic aquí: <a href='{resetUrl}'>{resetUrl}</a></p>
            <p><strong>Token:</strong> {resetToken}</p>
            <p>Válido por 1 hora.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            // Configurar cliente SMTP
            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000 // 10 segundos por prueba
            };

            // Configurar seguridad
            if (enableSsl)
            {
                System.Net.ServicePointManager.SecurityProtocol =
                    System.Net.SecurityProtocolType.Tls12;
            }

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendVerificationEmail(string email, string verificationCode)
        {
            try
            {
                // Modo desarrollo: Mostrar en consola
                if (_emailSettings.SmtpServer.Contains("gmail"))
                {
                    _logger.LogInformation($"🔐 Código de verificación para {email}: {verificationCode}");
                }

                var request = _httpContextAccessor.HttpContext?.Request;
                var host = request?.Host.ToString() ?? "localhost:port";
                var scheme = request?.Scheme ?? "https";

                var verificationUrl = $"{scheme}://{host}/Acceso/VerificarEmail?email={WebUtility.UrlEncode(email)}&token={verificationCode}";

                var subject = "✅ Verifica tu cuenta - Farmacia La Puna";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
                            .content {{ padding: 30px; background-color: #f9f9f9; }}
                            .button {{ display: inline-block; padding: 12px 24px; background-color: #2196F3; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                            .code {{ font-size: 24px; font-weight: bold; color: #2196F3; text-align: center; margin: 20px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>✅ Bienvenido a Farmacia La Puna</h2>
                            </div>
                            <div class='content'>
                                <p>¡Gracias por registrarte! Para activar tu cuenta, verifica tu dirección de correo electrónico.</p>
                                
                                <div class='code'>{verificationCode}</div>
                                
                                <p style='text-align: center;'>
                                    <a href='{verificationUrl}' class='button'>✨ Verificar Mi Cuenta</a>
                                </p>
                                
                                <p>Si no puedes hacer clic en el botón, copia y pega este enlace:</p>
                                <p><small>{verificationUrl}</small></p>
                                
                                <p>Este código expirará en 24 horas.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"✅ Email de verificación enviado a: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error enviando email de verificación a: {email}");
                _logger.LogInformation($"📧 CÓDIGO FALLBACK para {email}: {verificationCode}");
                throw;
            }
        }

        public async Task SendWelcomeEmail(string email, string nombreUsuario)
        {
            try
            {
                var subject = "👋 ¡Bienvenido a Farmacia La Puna!";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background: linear-gradient(135deg, #4CAF50, #2E7D32); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                            .welcome-icon {{ font-size: 48px; margin-bottom: 20px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <div class='welcome-icon'>👋</div>
                                <h2>¡Hola {nombreUsuario}!</h2>
                                <p>Te damos la bienvenida a Farmacia La Puna</p>
                            </div>
                            <div style='padding: 30px; background-color: #f9f9f9;'>
                                <p>Estamos emocionados de tenerte con nosotros. Ahora puedes:</p>
                                <ul>
                                    <li>📦 Realizar pedidos de medicamentos</li>
                                    <li>💊 Consultar nuestro catálogo</li>
                                    <li>🚀 Acceder a promociones exclusivas</li>
                                    <li>📱 Gestionar tu perfil</li>
                                </ul>
                                <p>Si necesitas ayuda, no dudes en contactarnos.</p>
                                <p style='text-align: center; margin-top: 30px;'>
                                    <a href='https://tudominio.com' style='background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                                        🏪 Ir a la Farmacia
                                    </a>
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"✅ Email de bienvenida enviado a: {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error enviando email de bienvenida a: {email}");
                // No lanzamos excepción para no afectar el registro
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Validar configuración
            ValidateEmailSettings();

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                EnableSsl = _emailSettings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000 // 30 segundos
            };

            await smtpClient.SendMailAsync(mailMessage);
        }

        private void ValidateEmailSettings()
        {
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
                throw new ArgumentNullException(nameof(_emailSettings.SmtpServer), "Servidor SMTP no configurado");

            if (string.IsNullOrEmpty(_emailSettings.Username))
                throw new ArgumentNullException(nameof(_emailSettings.Username), "Usuario SMTP no configurado");

            if (string.IsNullOrEmpty(_emailSettings.Password))
                throw new ArgumentNullException(nameof(_emailSettings.Password), "Contraseña SMTP no configurada");

            _logger.LogDebug($"📧 Configuración SMTP: {_emailSettings.SmtpServer}:{_emailSettings.Port}");
        }
    }
}
