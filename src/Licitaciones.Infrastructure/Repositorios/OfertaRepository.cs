using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Repositorios;

public class OfertaRepository : IOfertaRepository
{
    private readonly LicitacionesDbContext _db;

    public OfertaRepository(LicitacionesDbContext db)
    {
        _db = db;
    }

    public Task<Oferta?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        _db.Ofertas.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Oferta>> ListarPorLicitacionAsync(Guid licitacionId, CancellationToken ct) =>
        await _db.Ofertas.Where(o => o.LicitacionId == licitacionId).ToListAsync(ct);

    public Task<bool> ExisteOfertaDeProveedorAsync(Guid licitacionId, Guid proveedorId, CancellationToken ct) =>
        _db.Ofertas.AnyAsync(o => o.LicitacionId == licitacionId && o.ProveedorId == proveedorId, ct);

    public Task<bool> ExisteAlgunaOfertaDeProveedorAsync(Guid proveedorId, CancellationToken ct) =>
        _db.Ofertas.AnyAsync(o => o.ProveedorId == proveedorId, ct);

    public Task<bool> ExisteAlgunaOfertaDeLicitacionAsync(Guid licitacionId, CancellationToken ct) =>
        _db.Ofertas.AnyAsync(o => o.LicitacionId == licitacionId, ct);

    public async Task AgregarAsync(Oferta oferta, CancellationToken ct) =>
        await _db.Ofertas.AddAsync(oferta, ct);

    public Task GuardarCambiosAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
