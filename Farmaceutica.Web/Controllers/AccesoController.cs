using Farmaceutica.Application.IServices;
using Farmaceutica.Application.Services;
using Farmaceutica.Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Farmaceutica.Web.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AccesoController> _logger;
        private readonly IEmailService _emailService;

        public AccesoController(AuthService authService, IEmailService emailService, ILogger<AccesoController> logger)
        {
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usuarioNombre, string password)
        {
            if (string.IsNullOrEmpty(usuarioNombre) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Usuario o contraseña vacíos";
                return View("Index");
            }

            var usuario = await _authService.LoginAsync(usuarioNombre, password);
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View("Index");
            }

            // --- GUARDAR DATOS EN SESSION ---
            HttpContext.Session.SetInt32("USU_ID", usuario.Id);
            HttpContext.Session.SetString("USU_IMG", string.IsNullOrEmpty(usuario.Imagen) ? "usuario.png" : usuario.Imagen);
            HttpContext.Session.SetString("USU_NOM", usuario.Nombre);
            HttpContext.Session.SetString("USU_APE", usuario.Apellido);
            HttpContext.Session.SetString("USU_ROL", usuario.Rol.ToString());

            // --- CREAR CLAIMS PARA AUTENTICACIÓN ---
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellido),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // --- REDIRECCIÓN SEGÚN ROL ---
            return usuario.Rol switch
            {
                Core.Enums.RolUsuario.Administrador => RedirectToAction("Index", "Dashboard"),
                Core.Enums.RolUsuario.Encargado => RedirectToAction("Index", "Dashboard"),
                Core.Enums.RolUsuario.Delivery => RedirectToAction("VentasDeliveryPorId", "Delivery"),
                Core.Enums.RolUsuario.Cliente => RedirectToAction("Pedidos", "Pedido"),
                _ => RedirectToAction("Index", "Home"),
            };
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Acceso");
        }

        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string email, string username, string password, string nombre, string apellido, string telefono, string ruc, string direccion)
        {
        

            var error = await _authService.LoginAsync(email, username, password, nombre, apellido, telefono, direccion, ruc);

            if (error != null)
            {
                ViewData["Error"] = error;
                return View();
            }

            TempData["Success"] = "Usuario registrado correctamente!";
            return RedirectToAction("Index"); // Redirige a login después del registro
        }

        [HttpGet]
        public IActionResult OlvidoContrasenia() => View();

        [HttpPost]
        public async Task<IActionResult> OlvidoContrasenia(string correo)
        {
            try
            {
                await _authService.EnviarCorreoRecuperacionAsync(correo);
                TempData["Success"] = "Se ha enviado un enlace de recuperación a tu correo.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public IActionResult RestaurarConstrasenia(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestaurarConstrasenia(string token, string NuevaContrasenia, string ConfirmarContrasenia)
        {
            try
            {
                await _authService.RestaurarContraseniaAsync(token, NuevaContrasenia, ConfirmarContrasenia);
                TempData["Success"] = "Contraseña restablecida correctamente.";
                return RedirectToAction("Login", "Acceso");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Token = token;
                return View();
            }
        }



    }
}
