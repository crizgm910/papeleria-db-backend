using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class CajaService : ICajaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CajaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CorteCajaResponseDto> AbrirCajaAsync(AbrirCajaDto dto)
    {
        var caja = await _unitOfWork.Cajas.GetByIdAsync(dto.CajaId);
        if (caja == null) return new CorteCajaResponseDto { Exito = false, Mensaje = "La caja no existe." };
        if (caja.EstaAbierta) return new CorteCajaResponseDto { Exito = false, Mensaje = "La caja ya está abierta." };

        // Crear nuevo turno (corte)
        var corte = new CorteCaja
        {
            CajaId = dto.CajaId,
            UsuarioId = dto.UsuarioId,
            FechaApertura = DateTime.Now,
            MontoInicial = dto.MontoInicial,
            Estado = "Abierto"
        };

        caja.EstaAbierta = true;
        caja.Cortes.Add(corte);
        
        await _unitOfWork.Cajas.UpdateAsync(caja);
        await _unitOfWork.CompleteAsync();

        return new CorteCajaResponseDto { Exito = true, Mensaje = "Caja abierta exitosamente." };
    }

    public async Task<CorteCajaResponseDto> CerrarCajaAsync(CerrarCajaDto dto)
    {
        var caja = await _unitOfWork.Cajas.GetCajaConVentasDelDiaAsync(dto.CajaId, DateTime.Now);
        if (caja == null || !caja.EstaAbierta) 
            return new CorteCajaResponseDto { Exito = false, Mensaje = "La caja no existe o no está abierta." };

        var corteAbierto = await _unitOfWork.Cajas.GetUltimoCorteAbiertoAsync(dto.CajaId);
        if (corteAbierto == null) 
            return new CorteCajaResponseDto { Exito = false, Mensaje = "No se encontró un turno abierto para esta caja." };

        // Calcular matemática del corte de caja
        var ventasEfectivo = caja.Ventas.Where(v => v.MetodoPagoPrincipal == "Efectivo").Sum(v => v.Total);
        var ventasTarjeta = caja.Ventas.Where(v => v.MetodoPagoPrincipal == "Tarjeta").Sum(v => v.Total);

        corteAbierto.EfectivoEsperado = corteAbierto.MontoInicial + ventasEfectivo;
        corteAbierto.EfectivoContado = dto.EfectivoContado;
        corteAbierto.Diferencia = corteAbierto.EfectivoContado - corteAbierto.EfectivoEsperado;
        
        corteAbierto.TotalVendidoTarjeta = ventasTarjeta;
        corteAbierto.FechaCierre = DateTime.Now;
        corteAbierto.Estado = "Cerrado";

        caja.EstaAbierta = false;

        await _unitOfWork.Cajas.UpdateAsync(caja);
        await _unitOfWork.CompleteAsync();

        return new CorteCajaResponseDto 
        { 
            Exito = true, 
            Mensaje = "Corte realizado con éxito.",
            EfectivoEsperado = corteAbierto.EfectivoEsperado,
            Diferencia = corteAbierto.Diferencia,
            TotalVendidoTarjeta = corteAbierto.TotalVendidoTarjeta
        };
    }
}
