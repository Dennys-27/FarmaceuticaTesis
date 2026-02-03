using Farmaceutica.Application.IServices;
using Farmaceutica.Core;
using Farmaceutica.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Farmaceutica.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(ContactoViewModel model)
        {
            _logger.LogInformation("?? Recibiendo formulario de contacto de: {Nombre}", model.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("? Formulario inválido para: {Email}", model.Email);
                TempData["ErrorMessage"] = "Por favor, completa todos los campos requeridos correctamente.";
                return RedirectToAction("Index", "Home", new { section = "contacto" });
            }

            try
            {
                _logger.LogInformation("?? Iniciando envío de correo de contacto...");

                // Usa tu servicio de email existente
                await _emailService
                    .SendContactEmailAsync(model);

                _logger.LogInformation("? Correo de contacto procesado exitosamente para: {Email}", model.Email);

                TempData["SuccessMessage"] = "¡Mensaje enviado exitosamente! Te contactaremos pronto.";
                return RedirectToAction("Index", "Home", new { section = "contacto" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error al enviar correo de contacto para: {Email}", model.Email);

                // Opcional: Guardar en base de datos como fallback
                await SaveContactAsFallback(model);

                TempData["ErrorMessage"] = $"Hubo un error al enviar el mensaje. Por favor, inténtalo nuevamente o contáctanos directamente.";
                return RedirectToAction("Index", "Home", new { section = "contacto" });
            }
        }

        private async Task SaveContactAsFallback(ContactoViewModel model)
        {
            try
            {
                _logger.LogInformation("?? Guardando contacto como fallback en logs...");
                _logger.LogInformation("?? Contacto fallback - Nombre: {Nombre}, Email: {Email}", model.Name, model.Email);
                _logger.LogInformation("?? Asunto: {Asunto}", model.Subject);
                _logger.LogInformation("?? Mensaje: {Mensaje}", model.Comments);

                // Aquí podrías guardar en base de datos si tienes una tabla para contactos
                // await _context.Contactos.AddAsync(new Contacto { ... });
                // await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error guardando contacto fallback");
            }
        }
    
}
}
