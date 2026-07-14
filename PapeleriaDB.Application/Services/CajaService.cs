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

    public async Task<IEnumerable<CajaSupervisionDto>> GetSupervisionAsync(int historyLimit)
    {
        var cajas = await _unitOfWork.Cajas.GetAllWithActivityAsync();
        var result = new List<CajaSupervisionDto>();
        foreach (var caja in cajas)
        {
            var cortes = caja.Cortes.OrderByDescending(c => c.FechaApertura).ToList();
            var mapped = new List<CorteSupervisionDto>();
            foreach (var corte in cortes.Take(Math.Clamp(historyLimit, 1, 100)))
            {
                var usuario = await _unitOfWork.Usuarios.GetByIdAsync(corte.UsuarioId);
                var end = corte.FechaCierre ?? DateTime.Now;
                var ventas = caja.Ventas.Where(v => v.Estado == "Completada" && v.Fecha >= corte.FechaApertura && v.Fecha <= end).ToList();
                var movimientos = caja.Movimientos.Where(m => m.Fecha >= corte.FechaApertura && m.Fecha <= end).ToList();
                var efectivo = ventas.Where(v => v.MetodoPagoPrincipal == "Efectivo").Sum(v => v.Total);
                var tarjeta = ventas.Where(v => v.MetodoPagoPrincipal == "Tarjeta").Sum(v => v.Total);
                var transferencia = ventas.Where(v => v.MetodoPagoPrincipal == "Transferencia").Sum(v => v.Total);
                var ingresos = movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
                var egresos = movimientos.Where(m => m.Tipo == "Egreso").Sum(m => m.Monto);
                var esperado = corte.Estado == "Abierto" ? corte.MontoInicial + efectivo + ingresos - egresos : corte.EfectivoEsperado;
                mapped.Add(new CorteSupervisionDto
                {
                    Id = corte.Id, Usuario = usuario?.NombreCompleto ?? $"Usuario {corte.UsuarioId}",
                    FechaApertura = corte.FechaApertura, FechaCierre = corte.FechaCierre,
                    MontoInicial = corte.MontoInicial, VentasEfectivo = efectivo, VentasTarjeta = tarjeta,
                    VentasTransferencia = transferencia, IngresosExtra = ingresos, Egresos = egresos,
                    EfectivoEsperado = esperado,
                    EfectivoContado = corte.Estado == "Cerrado" ? corte.EfectivoContado : null,
                    Diferencia = corte.Estado == "Cerrado" ? corte.Diferencia : null, Estado = corte.Estado
                });
            }
            result.Add(new CajaSupervisionDto { Id = caja.Id, Nombre = caja.Nombre, EstaAbierta = caja.EstaAbierta, CorteActual = mapped.FirstOrDefault(c => c.Estado == "Abierto"), Historial = mapped.Where(c => c.Estado == "Cerrado") });
        }
        return result;
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

        var movimientosIngreso = caja.Movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
        var movimientosEgreso = caja.Movimientos.Where(m => m.Tipo == "Egreso").Sum(m => m.Monto);

        corteAbierto.EfectivoEsperado = corteAbierto.MontoInicial + ventasEfectivo + movimientosIngreso - movimientosEgreso;
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
