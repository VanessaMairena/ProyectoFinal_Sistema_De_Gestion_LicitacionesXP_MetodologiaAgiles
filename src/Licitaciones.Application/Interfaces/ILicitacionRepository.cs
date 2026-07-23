using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;

namespace Licitaciones.Application.Interfaces;

public interface ILicitacionRepository
{
    Task<Licitacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExisteConCodigoNormalizadoAsync(string codigoNormalizado, Guid? excluirId, CancellationToken ct);
    Task<(IReadOnlyList<Licitacion> Items, int Total)> ListarAsync(
        EstadoLicitacion? filtroEstado, int pagina, int tamanoPagina, CancellationToken ct);
    Task<bool> TieneOfertasRelacionadasAsync(Guid licitacionId, CancellationToken ct);
    Task AgregarAsync(Licitacion licitacion, CancellationToken ct);
    Task GuardarCambiosAsync(CancellationToken ct);
}
