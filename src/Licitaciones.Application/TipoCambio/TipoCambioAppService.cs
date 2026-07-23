using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Excepciones;
using Entidad = Licitaciones.Domain.Entidades.TipoCambio;

namespace Licitaciones.Application.TipoCambio;

public sealed class TipoCambioAppService
{
    private readonly ITipoCambioRepository _repositorio;
    private readonly TimeProvider _reloj;

    public TipoCambioAppService(ITipoCambioRepository repositorio, TimeProvider reloj)
    {
        _repositorio = repositorio;
        _reloj = reloj;
    }

    /// <summary>
    /// Registra un nuevo tipo de cambio y lo activa de inmediato, desactivando
    /// el anterior (regla HU-10: solo uno puede estar activo a la vez).
    /// </summary>
    public async Task<TipoCambioDto> RegistrarYActivarAsync(CrearTipoCambioRequest request, CancellationToken ct)
    {
        var ahora = _reloj.GetUtcNow();
        var nuevo = Entidad.Crear(request.CRCporUSD, request.FechaVigencia, ahora);

        var activoActual = await _repositorio.ObtenerActivoAsync(ct);
        activoActual?.Desactivar(ahora);

        nuevo.Activar(ahora);

        await _repositorio.AgregarAsync(nuevo, ct);
        await _repositorio.GuardarCambiosAsync(ct);

        return ATipoCambioDto(nuevo);
    }

    public async Task<IReadOnlyList<TipoCambioDto>> ListarAsync(CancellationToken ct)
    {
        var lista = await _repositorio.ListarTodosAsync(ct);
        return lista.OrderByDescending(t => t.FechaVigencia).Select(ATipoCambioDto).ToList();
    }

    public async Task<ConversionDto> ConvertirAsync(decimal montoCRC, CancellationToken ct)
    {
        var activo = await _repositorio.ObtenerActivoAsync(ct)
            ?? throw new DomainException(
                "tipocambio.sin_activo",
                "No hay un tipo de cambio activo configurado. Registrá uno primero.");

        return new ConversionDto(montoCRC, activo.ConvertirCrcAUsd(montoCRC), activo.CRCporUSD, activo.FechaVigencia);
    }

    private static TipoCambioDto ATipoCambioDto(Entidad t) =>
        new(t.Id, t.CRCporUSD, t.FechaVigencia, t.Activo);
}
