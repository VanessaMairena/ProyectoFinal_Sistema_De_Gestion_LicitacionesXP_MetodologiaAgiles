using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;
using Licitaciones.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Repositorios;

public class LicitacionRepository : ILicitacionRepository
{
    private readonly LicitacionesDbContext _db;

    public LicitacionRepository(LicitacionesDbContext db)
    {
        _db = db;
    }

    public Task<Licitacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        _db.Licitaciones.FirstOrDefaultAsync(l => l.Id == id && !l.Eliminada, ct);

    public Task<bool> ExisteConCodigoNormalizadoAsync(string codigoNormalizado, Guid? excluirId, CancellationToken ct) =>
        _db.Licitaciones.AnyAsync(l =>
            l.CodigoNormalizado == codigoNormalizado && !l.Eliminada && (excluirId == null || l.Id != excluirId), ct);

    public async Task<(IReadOnlyList<Licitacion> Items, int Total)> ListarAsync(
        EstadoLicitacion? filtroEstado, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var query = _db.Licitaciones.Where(l => !l.Eliminada);

        if (filtroEstado.HasValue)
            query = query.Where(l => l.Estado == filtroEstado.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<bool> TieneOfertasRelacionadasAsync(Guid licitacionId, CancellationToken ct) =>
        _db.Ofertas.AnyAsync(o => o.LicitacionId == licitacionId, ct);

    public async Task AgregarAsync(Licitacion licitacion, CancellationToken ct) =>
        await _db.Licitaciones.AddAsync(licitacion, ct);

    public Task GuardarCambiosAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
