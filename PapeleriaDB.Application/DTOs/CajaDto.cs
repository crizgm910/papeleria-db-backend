namespace PapeleriaDB.Application.DTOs;

public class AbrirCajaDto
{
    public int CajaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal MontoInicial { get; set; }
}

public class CerrarCajaDto
{
    public int CajaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal EfectivoContado { get; set; }
}

public class CorteCajaResponseDto
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public decimal EfectivoEsperado { get; set; }
    public decimal Diferencia { get; set; } // + Sobrante, - Faltante, 0 Cuadrado
    public decimal TotalVendidoTarjeta { get; set; }
}
