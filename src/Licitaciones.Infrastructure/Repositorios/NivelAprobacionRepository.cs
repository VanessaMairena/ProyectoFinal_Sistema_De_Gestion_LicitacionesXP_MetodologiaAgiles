using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Repositorios;

public class NivelAprobacionRepository : INivelAprobacionRepository
{
    private readonly LicitacionesDbContext _db;

    public NivelAprobacionRepository(LicitacionesDbContext db)
    {
        _db = db;
    }

    public Task<NivelAprobacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        _db.NivelesAprobacion.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<IReadOnlyList<NivelAprobacion>> ListarTodosAsync(CancellationToken ct) =>
        await _db.NivelesAprobacion.ToListAsync(ct);

    public async Task AgregarAsync(NivelAprobacion nivel, CancellationToken ct) =>
        await _db.NivelesAprobacion.AddAsync(nivel, ct);

    public Task EliminarAsync(NivelAprobacion nivel, CancellationToken ct)
    {
        _db.NivelesAprobacion.Remove(nivel);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
