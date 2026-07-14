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
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

// Mantiene la semántica de fechas del sistema SQLite existente durante la
// transición a PostgreSQL. La normalización total a UTC queda para una migración posterior.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CrearProductoDtoValidator>();
builder.Services.AddEndpointsApiExplorer();


// SQLite continúa siendo el modo local. Render establece una conexión PostgreSQL
// para centralizar los datos en Supabase sin cambiar los contratos de la API.
var postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var usePostgres = string.Equals(builder.Configuration["DatabaseProvider"], "Postgres", StringComparison.OrdinalIgnoreCase)
    || !string.IsNullOrWhiteSpace(postgresConnection);

if (usePostgres && string.IsNullOrWhiteSpace(postgresConnection))
{
    throw new InvalidOperationException("Configura ConnectionStrings:DefaultConnection para utilizar PostgreSQL.");
}

if (usePostgres)
{
    postgresConnection = NormalizePostgresConnectionString(postgresConnection!);
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (usePostgres)
    {
        options.UseNpgsql(postgresConnection, npgsql =>
            npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
    }
    else
    {
        options.UseSqlite("Data Source=papeleria.db");
    }
});

// Register Repositories & Unit of Work (Inyección de dependencias)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IMovimientoInventarioRepository, MovimientoInventarioRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();

// Register Application Services
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<ICajaService, CajaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReportesService, ReportesService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMovimientoInventarioService, MovimientoInventarioService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IMobileDashboardService, MobileDashboardService>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<ICompraService, CompraService>();

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
    
    if (usePostgres)
    {
        await BootstrapPostgresAsync(context);
    }
    else
    {
        // Conserva el flujo existente para instalaciones locales.
        await context.Database.MigrateAsync();
    }

    if (!await context.Usuarios.AnyAsync())
    {
        context.Usuarios.Add(new PapeleriaDB.Domain.Entities.Usuario
        {
            NombreCompleto = "Administrador del Sistema",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Rol = "Admin"
        });
        await context.SaveChangesAsync();
    }

    if (!await context.Cajas.AnyAsync())
    {
        context.Cajas.Add(new PapeleriaDB.Domain.Entities.Caja
        {
            Nombre = "Caja Principal",
            EstaAbierta = false
        });
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<AuditMiddleware>();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.MapControllers();

app.Run();

static async Task BootstrapPostgresAsync(ApplicationDbContext context)
{
    var connection = context.Database.GetDbConnection();
    await connection.OpenAsync();
    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = 'papeleria' AND table_name = 'Usuarios'
            );
            """;
        var schemaExists = (bool)(await command.ExecuteScalarAsync() ?? false);
        if (!schemaExists)
        {
            var creator = context.GetService<IRelationalDatabaseCreator>();
            await creator.CreateTablesAsync();
        }
    }
    finally
    {
        await connection.CloseAsync();
    }
}

static string NormalizePostgresConnectionString(string value)
{
    if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
        && !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        return value;
    }

    var uri = new Uri(value);
    var credentials = uri.UserInfo.Split(':', 2);
    if (credentials.Length != 2)
    {
        throw new InvalidOperationException("La conexión de PostgreSQL no contiene usuario y contraseña.");
    }

    return new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Database = uri.AbsolutePath.Trim('/'),
        Username = Uri.UnescapeDataString(credentials[0]),
        Password = Uri.UnescapeDataString(credentials[1]),
        SslMode = SslMode.Require
    }.ConnectionString;
}
