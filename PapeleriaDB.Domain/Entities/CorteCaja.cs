namespace PapeleriaDB.Domain.Entities;

public class CorteCaja
{
    public int Id { get; set; }
    public int CajaId { get; set; }
    public Caja? Caja { get; set; }
    public int UsuarioId { get; set; }
    
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    
    public decimal MontoInicial { get; set; }
    public decimal EfectivoEsperado { get; set; }
    public decimal EfectivoContado { get; set; }
    public decimal Diferencia { get; set; }
    
    public decimal TotalVendidoTarjeta { get; set; }
    public decimal TotalVendidoTransferencia { get; set; }
    
    public string Estado { get; set; } = "Abierto"; // Abierto, Cerrado
}
