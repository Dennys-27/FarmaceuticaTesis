using Farmaceutica.Application.IServices;
using Farmaceutica.Application.Mappings;
using Farmaceutica.Application.ServiceMovil;
using Farmaceutica.Application.ServiceMovil.IServiceMovil;
using Farmaceutica.Application.Services;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Infrastructure.Data;
using Farmaceutica.Infrastructure.Repositories.RepositoriesMovil;
using Farmaceutica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity; // ¡Añade esto!
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthService = Farmaceutica.Application.ServiceMovil.AuthService;
using Farmaceutica.Core.Interfaces.InterfacesMovil;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cadena de Conexión
builder.Services.AddDbContext<AppFarmaceuticaContex>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ¡ESTO ES CRÍTICO! HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ⭐⭐ SOLUCIÓN: Registrar solo PasswordHasher ⭐⭐
builder.Services.AddSingleton<IPasswordHasher<Usuario>>(provider =>
    new PasswordHasher<Usuario>());

// Repositorios y Servicios se inyectan aquí
// Inyección de dependencias de repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductoMovilRepository, ProductoMovilRepository>();
// Agregar estos registros en Program.cs
builder.Services.AddScoped<IVentaMovilRepository, VentaMovilRepository>();
builder.Services.AddScoped<IVentaService, Farmaceutica.Application.ServiceMovil.VentaService>();


// Si usas AutoMapper:
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Inyección de dependencias de servicios
// IMPORTANTE: EmailService debe estar registrado COMO INTERFAZ
builder.Services.AddScoped<IEmailService, EmailService>();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

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
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⭐⭐ ORDEN IMPORTANTE ⭐⭐
app.UseAuthentication(); // Primero
app.UseAuthorization();  // Después

app.MapControllers();
app.Run();