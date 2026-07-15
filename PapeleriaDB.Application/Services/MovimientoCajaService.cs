using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class MovimientoCajaService : IMovimientoCajaService
{
    private readonly IUnitOfWork _unitOfWork;
    public MovimientoCajaService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<MovimientoCajaDto> RegistrarAsync(RegistrarMovimientoCajaDto dto, int usuarioId)
    {
        var caja = await _unitOfWork.Cajas.GetByIdAsync(dto.CajaId)
            ?? throw new KeyNotFoundException("La caja no existe.");
        if (!caja.EstaAbierta) throw new InvalidOperationException("La caja está cerrada. Ábrela antes de registrar movimientos.");

        var movimiento = new MovimientoCaja
        {
            CajaId = caja.Id,
            Tipo = dto.Tipo.Trim(),
            Monto = dto.Monto,
            Motivo = dto.Motivo.Trim(),
            Fecha = DateTime.Now,
            UsuarioId = usuarioId
        };
        caja.Movimientos.Add(movimiento);
        await _unitOfWork.Cajas.UpdateAsync(caja);
        await _unitOfWork.CompleteAsync();
        return Map(movimiento);
    }

    public async Task<IEnumerable<MovimientoCajaDto>> GetRecentAsync(int cajaId, int limit) =>
        (await _unitOfWork.Cajas.GetMovimientosRecientesAsync(cajaId, Math.Clamp(limit, 1, 200))).Select(Map);

    private static MovimientoCajaDto Map(MovimientoCaja m) => new()
    {
        Id = m.Id, CajaId = m.CajaId, Tipo = m.Tipo, Monto = m.Monto,
        Motivo = m.Motivo, Fecha = m.Fecha, UsuarioId = m.UsuarioId
    };
}
