using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Application.Services;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence;
using PapeleriaDB.Persistence.Contexts;
using PapeleriaDB.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using PapeleriaDB.Application.Validators;
using System.Text;
using PapeleriaDB.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CrearProductoDtoValidator>();
builder.Services.AddEndpointsApiExplorer();


// Configure Database (SQLite para portabilidad sin instalación de servidor)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=papeleria.db"));

// Register Repositories & Unit of Work (Inyección de dependencias)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

// Register Application Services
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<ICajaService, CajaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReportesService, ReportesService>();

// Configure JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "SuperSecretaClaveLargaParaJWTPapeleria2026!!");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Inicializar la base de datos y crear el usuario por defecto
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    // Aplica las migraciones de SQLite automáticamente si no existen
    context.Database.Migrate();

    if (!context.Usuarios.Any())
    {
        context.Usuarios.Add(new PapeleriaDB.Domain.Entities.Usuario
        {
            NombreCompleto = "Administrador del Sistema",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Rol = "Admin"
        });
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
