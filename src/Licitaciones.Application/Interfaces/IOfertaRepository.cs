using Licitaciones.Domain.Entidades;

namespace Licitaciones.Application.Interfaces;

public interface IOfertaRepository
{
    Task<Oferta?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Oferta>> ListarPorLicitacionAsync(Guid licitacionId, CancellationToken ct);
    Task<bool> ExisteOfertaDeProveedorAsync(Guid licitacionId, Guid proveedorId, CancellationToken ct);
    Task<bool> ExisteAlgunaOfertaDeProveedorAsync(Guid proveedorId, CancellationToken ct);
    Task<bool> ExisteAlgunaOfertaDeLicitacionAsync(Guid licitacionId, CancellationToken ct);
    Task AgregarAsync(Oferta oferta, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}
