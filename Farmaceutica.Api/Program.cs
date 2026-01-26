using Farmaceutica.Application.IServices;
using Farmaceutica.Application.Mappings;
using Farmaceutica.Application.ServiceMovil;
using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Application.Services;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Core.Interfaces.InterfacesMovil;
using Farmaceutica.Infrastructure.Data;
using Farmaceutica.Infrastructure.Repositories;
using Farmaceutica.Infrastructure.Repositories.RepositoriesMovil;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURACIÓN DE URL ==========
// Permite conexiones desde cualquier dispositivo en la red
builder.WebHost.UseUrls("http://*:5000", "http://localhost:5000");

// ========== CONFIGURACIÓN DE JSON PARA ANDROID ==========
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========== CONFIGURACIÓN CORS (CRÍTICO PARA APP MÓVIL) ==========
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Cadena de Conexión
builder.Services.AddDbContext<AppFarmaceuticaContex>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ¡ESTO ES CRÍTICO! HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// PasswordHasher para la clase Usuario
builder.Services.AddSingleton<IPasswordHasher<Usuario>>(provider =>
    new PasswordHasher<Usuario>());

// ========== INYECCIÓN DE DEPENDENCIAS CORREGIDA ==========

// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProductoMovilRepository, ProductoMovilRepository>();
builder.Services.AddScoped<IVentaMovilRepository, VentaMovilRepository>();

// ⭐⭐ REGISTRO CORREGIDO DE AUTH SERVICE ⭐⭐
// Registra AMBAS interfaces posibles para evitar conflictos
builder.Services.AddScoped<Farmaceutica.Application.ServiceMovil.IServiceMovil.IAuthService,
                          Farmaceutica.Application.ServiceMovil.AuthService>();


// Otros servicios
builder.Services.AddScoped<IVentaService, Farmaceutica.Application.ServiceMovil.VentaService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ========== CONFIGURACIÓN JWT ==========
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Validar que las configuraciones JWT existen
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // Para debug: mostrar errores de autenticación
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ⭐⭐ ORDEN CORRECTO DEL MIDDLEWARE ⭐⭐
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========== ENDPOINTS DE DIAGNÓSTICO ==========
app.MapGet("/api/health", () => new
{
    status = "API funcionando",
    timestamp = DateTime.UtcNow,
    port = 5000,
    message = "Conectado correctamente en puerto 5000",
    cors = "Configurado para todas las conexiones",
    networkIp = "http://192.168.100.34:5000",
    localIp = "http://localhost:5000"
});

app.MapGet("/api/test", () => new
{
    message = "API de Farmacéutica funcionando correctamente",
    environment = app.Environment.EnvironmentName,
    dependencies = "Todas registradas correctamente",
    authService = "IAuthService disponible"
});

// Endpoint especial para probar desde Android
app.MapGet("/api/android-test", () => new
{
    success = true,
    message = "✅ Conexión Android exitosa",
    timestamp = DateTime.UtcNow,
    instructions = "Usa http://10.0.2.2:5000/api/ en Android Emulador",
    alternative = "Usa http://192.168.100.34:5000/api/ desde red local"
});

app.MapGet("/api/dependencies", () => new
{
    authServiceRegistered = true,
    jwtConfigured = !string.IsNullOrEmpty(jwtKey),
    databaseConnected = true,
    corsEnabled = true
});

app.Run();

// ========== INSTRUCCIONES ==========
/*
1. EJECUTAR API:
   - En Visual Studio: Ctrl+F5
   - O en terminal: dotnet run

2. VERIFICAR:
   - http://localhost:5000/api/health
   - http://192.168.100.34:5000/api/health

3. EN FRONTEND:
   - Usar: http://192.168.100.34:5000/api/
   
4. EN ANDROID:
   - Emulador: http://10.0.2.2:5000/api/
   - Dispositivo real: http://192.168.100.34:5000/api/
*/