namespace PapeleriaDB.Domain.Entities;

public class Caja
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // Ej. Caja 1, Caja Principal
    public bool EstaAbierta { get; set; } = false;
    
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public ICollection<CorteCaja> Cortes { get; set; } = new List<CorteCaja>();
}
