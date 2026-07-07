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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}
