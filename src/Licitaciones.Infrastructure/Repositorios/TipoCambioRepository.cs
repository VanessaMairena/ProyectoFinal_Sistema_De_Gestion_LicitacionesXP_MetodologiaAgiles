using Licitaciones.Application.Interfaces;
using Licitaciones.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Entidad = Licitaciones.Domain.Entidades.TipoCambio;

namespace Licitaciones.Infrastructure.Repositorios;

public class TipoCambioRepository : ITipoCambioRepository
{
    private readonly LicitacionesDbContext _db;

    public TipoCambioRepository(LicitacionesDbContext db)
    {
        _db = db;
    }

    public Task<Entidad?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        _db.TiposCambio.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<Entidad?> ObtenerActivoAsync(CancellationToken ct) =>
        _db.TiposCambio.FirstOrDefaultAsync(t => t.Activo, ct);

    public async Task<IReadOnlyList<Entidad>> ListarTodosAsync(CancellationToken ct) =>
        await _db.TiposCambio.ToListAsync(ct);

    public async Task AgregarAsync(Entidad tipoCambio, CancellationToken ct) =>
        await _db.TiposCambio.AddAsync(tipoCambio, ct);

    public Task GuardarCambiosAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
