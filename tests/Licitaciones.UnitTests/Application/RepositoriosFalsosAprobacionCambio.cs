using Licitaciones.Application.Interfaces;
using Entidad = Licitaciones.Domain.Entidades.TipoCambio;

namespace Licitaciones.UnitTests.Application;

public sealed class NivelAprobacionRepositoryFalso : INivelAprobacionRepository
{
    public List<Licitaciones.Domain.Entidades.NivelAprobacion> Datos { get; } = new();

    public Task<Licitaciones.Domain.Entidades.NivelAprobacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Datos.FirstOrDefault(n => n.Id == id));

    public Task<IReadOnlyList<Licitaciones.Domain.Entidades.NivelAprobacion>> ListarTodosAsync(CancellationToken ct) =>
        Task.FromResult((IReadOnlyList<Licitaciones.Domain.Entidades.NivelAprobacion>)Datos.ToList());

    public Task AgregarAsync(Licitaciones.Domain.Entidades.NivelAprobacion nivel, CancellationToken ct)
    {
        Datos.Add(nivel);
        return Task.CompletedTask;
    }

    public Task EliminarAsync(Licitaciones.Domain.Entidades.NivelAprobacion nivel, CancellationToken ct)
    {
        Datos.Remove(nivel);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
}

public sealed class TipoCambioRepositoryFalso : ITipoCambioRepository
{
    public List<Entidad> Datos { get; } = new();

    public Task<Entidad?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Datos.FirstOrDefault(t => t.Id == id));

    public Task<Entidad?> ObtenerActivoAsync(CancellationToken ct) =>
        Task.FromResult(Datos.FirstOrDefault(t => t.Activo));

    public Task<IReadOnlyList<Entidad>> ListarTodosAsync(CancellationToken ct) =>
        Task.FromResult((IReadOnlyList<Entidad>)Datos.ToList());

    public Task AgregarAsync(Entidad tipoCambio, CancellationToken ct)
    {
        Datos.Add(tipoCambio);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
}
