using Licitaciones.Domain.Entidades;

namespace Licitaciones.Application.Interfaces;

public interface INivelAprobacionRepository
{
    Task<NivelAprobacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<NivelAprobacion>> ListarTodosAsync(CancellationToken ct);
    Task AgregarAsync(NivelAprobacion nivel, CancellationToken ct);
    Task EliminarAsync(NivelAprobacion nivel, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}
