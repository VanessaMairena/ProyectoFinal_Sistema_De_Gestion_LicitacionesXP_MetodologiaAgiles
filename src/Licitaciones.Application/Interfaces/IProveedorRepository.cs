using Licitaciones.Domain.Entidades;

namespace Licitaciones.Application.Interfaces;

public interface IProveedorRepository
{
    Task<Proveedor?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExisteConNombreNormalizadoAsync(string nombreNormalizado, Guid? excluirId, CancellationToken ct);
    Task<(IReadOnlyList<Proveedor> Items, int Total)> ListarAsync(
        string? filtroNombre, int pagina, int tamanoPagina, CancellationToken ct);
    Task<bool> TieneOfertasRelacionadasAsync(Guid proveedorId, CancellationToken ct);
    Task<IReadOnlyList<Proveedor>> ListarActivosParaSeleccionAsync(CancellationToken ct);
    Task AgregarAsync(Proveedor proveedor, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}
