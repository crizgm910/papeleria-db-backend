namespace PapeleriaDB.Domain.Entities;

public class MovimientoCaja
{
    public int Id { get; set; }
    public int CajaId { get; set; }
    public Caja? Caja { get; set; }
    
    // "Ingreso" o "Egreso"
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int UsuarioId { get; set; }
}
