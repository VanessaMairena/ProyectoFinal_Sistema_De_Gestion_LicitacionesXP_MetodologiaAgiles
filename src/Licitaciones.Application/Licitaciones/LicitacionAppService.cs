using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;

namespace Licitaciones.Application.Licitaciones;

public sealed class LicitacionAppService
{
    private readonly ILicitacionRepository _repositorio;
    private readonly TimeProvider _reloj;

    public LicitacionAppService(ILicitacionRepository repositorio, TimeProvider reloj)
    {
        _repositorio = repositorio;
        _reloj = reloj;
    }

    public async Task<LicitacionDto> CrearAsync(CrearLicitacionRequest request, CancellationToken ct)
    {
        var ahora = _reloj.GetUtcNow();
        var licitacion = Licitacion.Crear(
            request.Codigo, request.Titulo, request.FechaCierre, request.PresupuestoEstimadoCRC, ahora);

        if (await _repositorio.ExisteConCodigoNormalizadoAsync(licitacion.CodigoNormalizado, null, ct))
            throw new Domain.Excepciones.DomainException(
                "licitacion.codigo_duplicado", "Ya existe una licitación con un código equivalente.");

        await _repositorio.AgregarAsync(licitacion, ct);
        await _repositorio.GuardarCambiosAsync(ct);

        return ALicitacionDto(licitacion);
    }

    public async Task<LicitacionDto> ObtenerAsync(Guid id, CancellationToken ct)
    {
        var licitacion = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", id);

        return ALicitacionDto(licitacion);
    }

    public async Task<ResultadoPaginado<LicitacionDto>> ListarAsync(
        EstadoLicitacion? filtroEstado, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var parametros = new ParametrosPaginacion(pagina, tamanoPagina);
        var (items, total) = await _repositorio.ListarAsync(
            filtroEstado, parametros.Pagina, parametros.TamanoPagina, ct);

        return new ResultadoPaginado<LicitacionDto>(
            items.Select(ALicitacionDto).ToList(), total, parametros.Pagina, parametros.TamanoPagina);
    }

    public async Task<LicitacionDto> EditarAsync(Guid id, EditarLicitacionRequest request, CancellationToken ct)
    {
        var licitacion = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", id);

        licitacion.ReducirPresupuesto(request.PresupuestoEstimadoCRC, null);
        await _repositorio.GuardarCambiosAsync(ct);

        return ALicitacionDto(licitacion);
    }

    public async Task<LicitacionDto> CambiarEstadoAsync(
        Guid id, CambiarEstadoLicitacionRequest request, CancellationToken ct)
    {
        var licitacion = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", id);

        var ahora = _reloj.GetUtcNow();

        switch (request.NuevoEstado)
        {
            case EstadoLicitacion.Publicada:
                licitacion.Publicar(ahora);
                break;
            case EstadoLicitacion.Cerrada:
                licitacion.Cerrar(ahora);
                break;
            default:
                throw new Domain.Excepciones.DomainException(
                    "licitacion.transicion_invalida", "Transición de estado no soportada.");
        }

        await _repositorio.GuardarCambiosAsync(ct);
        return ALicitacionDto(licitacion);
    }

    public async Task EliminarAsync(Guid id, CancellationToken ct)
    {
        var licitacion = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", id);

        licitacion.MarcarComoEliminada(_reloj.GetUtcNow());
        await _repositorio.GuardarCambiosAsync(ct);
    }

    private static LicitacionDto ALicitacionDto(Licitacion l) =>
        new(l.Id, l.Codigo, l.Titulo, l.Estado.ToString(), l.FechaCierre, l.PresupuestoEstimadoCRC);
}
