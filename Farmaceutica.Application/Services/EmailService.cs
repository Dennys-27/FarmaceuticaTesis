using Farmaceutica.Application.IServices;
using Farmaceutica.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            // Verificar configuración al iniciar
            ValidateAndLogConfiguration();
        }

        private void ValidateAndLogConfiguration()
        {
            _logger.LogInformation("=== CONFIGURACIÓN SMTP CARGADA ===");
            _logger.LogInformation("Servidor: {Server}:{Port}", _emailSettings.SmtpServer, _emailSettings.Port);
            _logger.LogInformation("Usuario: {User}", _emailSettings.UserName);
            _logger.LogInformation("Remitente: {Name} <{Email}>", _emailSettings.SenderName, _emailSettings.SenderEmail);
            _logger.LogInformation("SSL: {Ssl}", _emailSettings.EnableSsl);
            _logger.LogInformation("Contraseña: {HasPassword}",
                string.IsNullOrEmpty(_emailSettings.Password) ? "NO" : "SÍ (App Password)");
            _logger.LogInformation("===============================");

            // Validar que todo esté configurado
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
                throw new ArgumentException("SmtpServer no configurado en appsettings.json");

            if (string.IsNullOrEmpty(_emailSettings.UserName))
                throw new ArgumentException("UserName no configurado en appsettings.json");

            if (string.IsNullOrEmpty(_emailSettings.Password))
                throw new ArgumentException("Password no configurado en appsettings.json");
        }

        public async Task SendPasswordResetEmail(string email, string resetToken)
        {
            _logger.LogInformation("🔐 Enviando recuperación a: {Email}", email);

            try
            {
                // Construir URL de restablecimiento
                var request = _httpContextAccessor.HttpContext?.Request;
                var host = request?.Host.ToString() ?? "localhost:7101";
                var scheme = request?.Scheme ?? "https";
                var resetUrl = $"{scheme}://{host}/Acceso/RestaurarConstrasenia?token={resetToken}";

                // Crear mensaje
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = "🔐 Restablecimiento de Contraseña - Farmacia La Puna",
                    Body = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; max-width: 600px; margin: 0 auto;'>
                        <div style='background: linear-gradient(135deg, #2196F3, #1976D2); color: white; padding: 20px; border-radius: 10px 10px 0 0; text-align: center;'>
                            <h2 style='margin: 0;'>🔐 Restablecer Contraseña</h2>
                        </div>
                        <div style='background-color: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px;'>
                            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
                            <p>Haz clic en el siguiente botón:</p>
                            <div style='text-align: center; margin: 25px 0;'>
                                <a href='{resetUrl}' style='
                                    background-color: #2196F3;
                                    color: white;
                                    padding: 12px 30px;
                                    text-decoration: none;
                                    border-radius: 5px;
                                    font-weight: bold;
                                    display: inline-block;
                                '>🔄 Restablecer Contraseña</a>
                            </div>
                            <p>O copia y pega esta URL en tu navegador:</p>
                            <div style='background-color: #f0f0f0; padding: 12px; border-radius: 5px; word-break: break-all; font-size: 12px;'>
                                {resetUrl}
                            </div>
                            <p style='margin-top: 20px;'><strong>Token de verificación:</strong> {resetToken}</p>
                            <hr style='border: none; border-top: 1px solid #ddd; margin: 25px 0;'>
                            <p style='font-size: 12px; color: #666;'>
                                ⏰ <strong>Este enlace es válido por 1 hora.</strong><br>
                                🛡️ Si no solicitaste este restablecimiento, puedes ignorar este correo.
                            </p>
                        </div>
                    </div>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                // Configurar cliente SMTP para Gmail
                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000 // 30 segundos
                };

                // Configurar TLS 1.2 (requerido por Gmail)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                _logger.LogInformation("🚀 Enviando correo via Gmail SMTP...");
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("✅ Correo enviado exitosamente a {Email}", email);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "❌ Error SMTP al enviar a {Email}", email);

                // Mensajes específicos para errores comunes de Gmail
                string errorMessage = smtpEx.Message.ToLower();
                if (errorMessage.Contains("authentication failed") || errorMessage.Contains("5.7.8"))
                {
                    throw new Exception("❌ Error de autenticación Gmail. Verifica:\n" +
                        "1. Que la contraseña sea una 'App Password' (no tu contraseña normal)\n" +
                        "2. Que hayas activado la verificación en dos pasos\n" +
                        "3. Que la App Password sea correcta");
                }
                else if (errorMessage.Contains("timed out"))
                {
                    throw new Exception("❌ Timeout al conectar con Gmail. Verifica tu conexión a internet.");
                }
                else
                {
                    throw new Exception($"❌ Error SMTP: {smtpEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general al enviar correo a {Email}", email);

                // Fallback: generar URL para mostrar
                var request = _httpContextAccessor.HttpContext?.Request;
                var host = request?.Host.ToString() ?? "localhost:7101";
                var scheme = request?.Scheme ?? "https";
                var resetUrl = $"{scheme}://{host}/Acceso/RestaurarConstrasenia?token={resetToken}";

                _logger.LogInformation("📧 TOKEN FALLBACK para {Email}: {Token}", email, resetToken);
                _logger.LogInformation("🔗 URL: {Url}", resetUrl);

                throw new Exception($"No se pudo enviar el correo. Usa este enlace: {resetUrl}");
            }
        }

        public async Task SendVerificationEmail(string email, string verificationCode)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var host = request?.Host.ToString() ?? "localhost:7101";
                var scheme = request?.Scheme ?? "https";
                var verificationUrl = $"{scheme}://{host}/Acceso/VerificarEmail?email={WebUtility.UrlEncode(email)}&token={verificationCode}";

                var subject = "✅ Verifica tu cuenta - Farmacia La Puna";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                            .container {{ max-width: 600px; margin: 0 auto; }}
                            .header {{ background: linear-gradient(135deg, #4CAF50, #2E7D32); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                            .content {{ padding: 30px; background-color: #f9f9f9; border-radius: 0 0 10px 10px; }}
                            .button {{ display: inline-block; padding: 14px 28px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px; }}
                            .code {{ font-size: 32px; font-weight: bold; color: #2196F3; text-align: center; margin: 25px 0; letter-spacing: 5px; background-color: #f0f8ff; padding: 15px; border-radius: 8px; }}
                            .url-box {{ background-color: #f5f5f5; padding: 12px; border-radius: 5px; word-break: break-all; font-size: 13px; margin: 15px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2 style='margin: 0;'>✅ Verifica tu Cuenta</h2>
                                <p style='margin: 10px 0 0 0; opacity: 0.9;'>Farmacia La Puna</p>
                            </div>
                            <div class='content'>
                                <p>¡Gracias por registrarte! Para activar tu cuenta, verifica tu dirección de correo electrónico.</p>
                                
                                <div class='code'>{verificationCode}</div>
                                
                                <p style='text-align: center;'>
                                    <a href='{verificationUrl}' class='button'>✨ Verificar Mi Cuenta</a>
                                </p>
                                
                                <p>Si el botón no funciona, copia y pega este enlace:</p>
                                <div class='url-box'>{verificationUrl}</div>
                                
                                <p style='color: #666; font-size: 14px;'>
                                    ⚠️ <strong>Este código expirará en 24 horas.</strong>
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("✅ Email de verificación enviado a: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando email de verificación a: {Email}", email);
                _logger.LogInformation("📧 CÓDIGO FALLBACK para {Email}: {Code}", email, verificationCode);
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
                            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                            .container {{ max-width: 600px; margin: 0 auto; }}
                            .header {{ background: linear-gradient(135deg, #2196F3, #1976D2); color: white; padding: 40px; text-align: center; border-radius: 10px 10px 0 0; }}
                            .content {{ padding: 35px; background-color: #f9f9f9; border-radius: 0 0 10px 10px; }}
                            .welcome-icon {{ font-size: 60px; margin-bottom: 20px; }}
                            .features {{ margin: 25px 0; }}
                            .feature-item {{ margin: 10px 0; padding-left: 25px; position: relative; }}
                            .feature-item:before {{ content: '✓'; position: absolute; left: 0; color: #4CAF50; font-weight: bold; }}
                            .btn-primary {{ display: inline-block; background: linear-gradient(135deg, #4CAF50, #2E7D32); color: white; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 16px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <div class='welcome-icon'>👋</div>
                                <h1 style='margin: 10px 0;'>¡Hola {nombreUsuario}!</h1>
                                <p style='font-size: 18px; opacity: 0.9;'>Te damos la bienvenida a Farmacia La Puna</p>
                            </div>
                            <div class='content'>
                                <p>Estamos emocionados de tenerte con nosotros. Ahora puedes acceder a todos nuestros servicios:</p>
                                
                                <div class='features'>
                                    <div class='feature-item'>📦 Realizar pedidos de medicamentos en línea</div>
                                    <div class='feature-item'>💊 Consultar nuestro catálogo completo</div>
                                    <div class='feature-item'>🚀 Acceder a promociones y descuentos exclusivos</div>
                                    <div class='feature-item'>📱 Gestionar tu perfil y historial de compras</div>
                                    <div class='feature-item'>🛡️ Comprar con total seguridad y confidencialidad</div>
                                </div>
                                
                                <p>Si necesitas ayuda o tienes alguna pregunta, no dudes en contactarnos.</p>
                                
                                <div style='text-align: center; margin-top: 35px;'>
                                    <a href='https://fersoftweb.com' class='btn-primary'>🏪 Comenzar a Comprar</a>
                                </div>
                                
                                <p style='margin-top: 25px; font-size: 14px; color: #666; text-align: center;'>
                                    Farmacia La Puna · Tu salud es nuestra prioridad
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("✅ Email de bienvenida enviado a: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando email de bienvenida a: {Email}", email);
                // No lanzar excepción para no afectar el flujo de registro
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Validar configuración
            if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
                throw new ArgumentException("SenderEmail no configurado");

            if (string.IsNullOrEmpty(_emailSettings.SenderName))
                throw new ArgumentException("SenderName no configurado");

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
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000 // 30 segundos
            };

            // TLS 1.2 para Gmail
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}