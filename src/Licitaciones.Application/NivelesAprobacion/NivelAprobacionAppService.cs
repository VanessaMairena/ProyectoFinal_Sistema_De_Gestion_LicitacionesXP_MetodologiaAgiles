using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Servicios;

namespace Licitaciones.Application.NivelesAprobacion;

public sealed class NivelAprobacionAppService
{
    private readonly INivelAprobacionRepository _repositorio;
    private readonly TimeProvider _reloj;

    public NivelAprobacionAppService(INivelAprobacionRepository repositorio, TimeProvider reloj)
    {
        _repositorio = repositorio;
        _reloj = reloj;
    }

    public async Task<NivelAprobacionDto> CrearAsync(CrearNivelAprobacionRequest request, CancellationToken ct)
    {
        var existentes = await _repositorio.ListarTodosAsync(ct);
        var ahora = _reloj.GetUtcNow();

        var nivel = NivelAprobacion.Crear(
            request.MontoMinimoCRC, request.MontoMaximoCRC, request.Aprobador, existentes, ahora);

        await _repositorio.AgregarAsync(nivel, ct);
        await _repositorio.GuardarCambiosAsync(ct);

        return ANivelDto(nivel);
    }

    public async Task<IReadOnlyList<NivelAprobacionDto>> ListarAsync(CancellationToken ct)
    {
        var niveles = await _repositorio.ListarTodosAsync(ct);
        return niveles
            .OrderBy(n => n.MontoMinimoCRC)
            .Select(ANivelDto)
            .ToList();
    }

    public async Task<AprobadorResueltoDto> ResolverAprobadorAsync(ResolverAprobadorRequest request, CancellationToken ct)
    {
        var niveles = await _repositorio.ListarTodosAsync(ct);
        var nivel = ServicioNivelAprobacion.ResolverAprobador(request.MontoCRC, niveles);

        return new AprobadorResueltoDto(nivel.Aprobador, nivel.MontoMinimoCRC, nivel.MontoMaximoCRC);
    }

    public async Task<NivelAprobacionDto> ObtenerAsync(Guid id, CancellationToken ct)
    {
        var nivel = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Nivel de aprobación", id);

        return ANivelDto(nivel);
    }

    public async Task<NivelAprobacionDto> EditarAsync(Guid id, EditarNivelAprobacionRequest request, CancellationToken ct)
    {
        var nivel = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Nivel de aprobación", id);

        var otros = (await _repositorio.ListarTodosAsync(ct)).Where(n => n.Id != id);
        nivel.Editar(request.MontoMinimoCRC, request.MontoMaximoCRC, request.Aprobador, otros, _reloj.GetUtcNow());

        await _repositorio.GuardarCambiosAsync(ct);
        return ANivelDto(nivel);
    }

    public async Task EliminarAsync(Guid id, CancellationToken ct)
    {
        var nivel = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Nivel de aprobación", id);

        await _repositorio.EliminarAsync(nivel, ct);
        await _repositorio.GuardarCambiosAsync(ct);
    }

    private static NivelAprobacionDto ANivelDto(NivelAprobacion n) =>
        new(n.Id, n.MontoMinimoCRC, n.MontoMaximoCRC, n.Aprobador);
}
