using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Venta> Ventas { get; set; } = null!;
    public DbSet<DetalleVenta> DetallesVenta { get; set; } = null!;
    public DbSet<Caja> Cajas { get; set; } = null!;
    public DbSet<CorteCaja> CortesCaja { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<MovimientoCaja> MovimientosCaja { get; set; } = null!;
    public DbSet<Servicio> Servicios { get; set; } = null!;
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; } = null!;
    public DbSet<Auditoria> Auditorias { get; set; } = null!;
    public DbSet<Proveedor> Proveedores { get; set; } = null!;
    public DbSet<Compra> Compras { get; set; } = null!;
    public DbSet<DetalleCompra> DetallesCompra { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Supabase expone `public` mediante su API de datos. Las tablas internas
        // del backend viven en un esquema separado y solo se alcanzan desde la API .NET.
        if (Database.IsNpgsql())
        {
            modelBuilder.HasDefaultSchema("papeleria");
        }

        // Configuraciones básicas con Fluent API
        
        modelBuilder.Entity<Venta>()
            .HasMany(v => v.Detalles)
            .WithOne(d => d.Venta)
            .HasForeignKey(d => d.VentaId);

        modelBuilder.Entity<Producto>()
            .Property(p => p.PrecioVenta)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Producto>()
            .Property(p => p.CostoCompra)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Producto>()
            .HasIndex(p => p.CodigoInterno)
            .IsUnique();

        modelBuilder.Entity<Producto>()
            .HasIndex(p => p.CodigoBarras)
            .IsUnique();

        modelBuilder.Entity<Venta>()
            .Property(v => v.Subtotal)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Venta>()
            .Property(v => v.Total)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Venta>()
            .Property(v => v.Descuento)
            .HasPrecision(18, 2);

        modelBuilder.Entity<DetalleVenta>()
            .Property(d => d.PrecioUnitario)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<DetalleVenta>()
            .Property(d => d.Subtotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MovimientoCaja>()
            .Property(m => m.Monto)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Servicio>()
            .Property(s => s.PrecioBase)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Proveedor>().HasIndex(p => p.Rfc).IsUnique();
        modelBuilder.Entity<Compra>().Property(c => c.Total).HasPrecision(18, 2);
        modelBuilder.Entity<DetalleCompra>().Property(d => d.CostoUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<DetalleCompra>().Property(d => d.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Compra>().HasMany(c => c.Detalles).WithOne(d => d.Compra).HasForeignKey(d => d.CompraId);
    }
}
