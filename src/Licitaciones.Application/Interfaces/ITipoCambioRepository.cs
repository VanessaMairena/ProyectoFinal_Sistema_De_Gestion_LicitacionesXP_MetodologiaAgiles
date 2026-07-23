using Entidad = Licitaciones.Domain.Entidades.TipoCambio;

namespace Licitaciones.Application.Interfaces;

public interface ITipoCambioRepository
{
    Task<Entidad?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<Entidad?> ObtenerActivoAsync(CancellationToken ct);
    Task<IReadOnlyList<Entidad>> ListarTodosAsync(CancellationToken ct);
    Task AgregarAsync(Entidad tipoCambio, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}
