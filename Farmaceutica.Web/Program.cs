using Farmaceutica.Application.IServices;
using Farmaceutica.Application.Services; // ? Asegúrate que sea este namespace
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar DbContext
builder.Services.AddDbContext<AppFarmaceuticaContex>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar EmailSettings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Inyección de dependencias de repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IEncargadoRepository, EncargadoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ISubCategoriaRepository, SubCategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IventaRepository, VentaRepository>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<IFarmaciaRepository, FarmaciaRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<ObtenerDashboardResumenUseCase>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

// Servicios y procesos masivos
builder.Services.AddScoped<IProductoTempRepository, ProductoTempRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


// Inyección de dependencias de servicios
builder.Services.AddScoped<ProcesarExcelService>();

// ? CORRECCIÓN SIMPLIFICADA: Solo registrar una vez
builder.Services.AddHttpContextAccessor();

// Dependiendo de cómo tengas estructurado tu proyecto, usa UNA de estas opciones:

// OPCIÓN 1: Si IEmailService está en Farmaceutica.Application.Services
builder.Services.AddScoped<IEmailService, EmailService>();

// OPCIÓN 2: Si NO tienes interfaz IEmailService, usa esto:
// builder.Services.AddScoped<EmailService>();
// builder.Services.AddScoped<AuthService>(provider => 
//     new AuthService(
//         provider.GetRequiredService<IUsuarioRepository>(),
//         provider.GetRequiredService<EmailService>(),
//         provider.GetRequiredService<ILogger<AuthService>>(),
//         provider.GetRequiredService<IHttpContextAccessor>()
//     ));

// ? Registrar AuthService
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<VentaService>();
builder.Services.AddScoped<CompraService>();

// Agrega esto antes de Build()
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configurar autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index";
        options.AccessDeniedPath = "/Acceso/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministradorOnly", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("EncargadoOnly", policy => policy.RequireRole("Encargado"));
    options.AddPolicy("DeliveryOnly", policy => policy.RequireRole("Delivery"));
    options.AddPolicy("ClienteOnly", policy => policy.RequireRole("Cliente"));
});

// --- NUEVO: Configurar sesión ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // Duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Después de 'var app = builder.Build();'
try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        logger.LogInformation("=== VERIFICACIÓN DE CONFIGURACIÓN SMTP ===");

        // Verificar AMBAS configuraciones (antigua y nueva)
        logger.LogInformation("--- Configuración ANTIGUA (EmailSettings) ---");
        var oldSmtpServer = configuration["EmailSettings:SmtpServer"];
        var oldPort = configuration["EmailSettings:Port"];
        var oldUser = configuration["EmailSettings:Username"];
        var oldPass = configuration["EmailSettings:Password"];

        logger.LogInformation($"Servidor: {oldSmtpServer ?? "NO CONFIGURADO"}");
        logger.LogInformation($"Puerto: {oldPort ?? "NO CONFIGURADO"}");
        logger.LogInformation($"Usuario: {oldUser ?? "NO CONFIGURADO"}");
        logger.LogInformation($"Contraseña: {(string.IsNullOrEmpty(oldPass) ? "NO" : "SÍ")}");

        logger.LogInformation("--- Configuración NUEVA (Smtp) ---");
        var newSmtpHost = configuration["Smtp:Host"];
        var newSmtpPort = configuration["Smtp:Port"];
        var newSmtpUser = configuration["Smtp:User"];
        var newSmtpPass = configuration["Smtp:Pass"];

        logger.LogInformation($"Host: {newSmtpHost ?? "NO CONFIGURADO"}");
        logger.LogInformation($"Puerto: {newSmtpPort ?? "NO CONFIGURADO"}");
        logger.LogInformation($"Usuario: {newSmtpUser ?? "NO CONFIGURADO"}");
        logger.LogInformation($"Contraseña: {(string.IsNullOrEmpty(newSmtpPass) ? "NO" : "SÍ")}");

        // Detectar cuál está activa
        var emailService = scope.ServiceProvider.GetService<IEmailService>();
        if (emailService != null)
        {
            logger.LogInformation("--- Servicio de correo detectado ---");
            logger.LogInformation($"Tipo: {emailService.GetType().Name}");
        }

        logger.LogInformation("===========================================");

        // Advertencia si hay configuraciones duplicadas
        if (!string.IsNullOrEmpty(oldSmtpServer) && !string.IsNullOrEmpty(newSmtpHost))
        {
            logger.LogWarning("⚠️  Tienes DOS configuraciones SMTP. Esto puede causar confusión.");
            logger.LogWarning("   Recomendación: Usa solo UNA (Smtp o EmailSettings)");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error al verificar configuración SMTP");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- NUEVO: Habilitar sesión ---
app.UseSession();

app.UseAuthentication(); // Asegúrate de que Authentication esté antes de Authorization
app.UseAuthorization();

// Rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();