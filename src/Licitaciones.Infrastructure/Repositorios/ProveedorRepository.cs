using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Repositorios;

public class ProveedorRepository : IProveedorRepository
{
    private readonly LicitacionesDbContext _db;

    public ProveedorRepository(LicitacionesDbContext db)
    {
        _db = db;
    }

    public Task<Proveedor?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        _db.Proveedores.FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado, ct);

    public Task<bool> ExisteConNombreNormalizadoAsync(string nombreNormalizado, Guid? excluirId, CancellationToken ct) =>
        _db.Proveedores.AnyAsync(p =>
            p.NombreNormalizado == nombreNormalizado && !p.Eliminado && (excluirId == null || p.Id != excluirId), ct);

    public async Task<(IReadOnlyList<Proveedor> Items, int Total)> ListarAsync(
        string? filtroNombre, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var query = _db.Proveedores.Where(p => !p.Eliminado);

        if (!string.IsNullOrWhiteSpace(filtroNombre))
        {
            var filtro = filtroNombre.Trim().ToLowerInvariant();
            query = query.Where(p => p.NombreNormalizado.Contains(filtro));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<bool> TieneOfertasRelacionadasAsync(Guid proveedorId, CancellationToken ct) =>
        _db.Ofertas.AnyAsync(o => o.ProveedorId == proveedorId, ct);

    public async Task<IReadOnlyList<Proveedor>> ListarActivosParaSeleccionAsync(CancellationToken ct) =>
        await _db.Proveedores
            .Where(p => !p.Eliminado)
            .OrderBy(p => p.Nombre)
            .ToListAsync(ct);

    public async Task AgregarAsync(Proveedor proveedor, CancellationToken ct) =>
        await _db.Proveedores.AddAsync(proveedor, ct);

    public Task GuardarCambiosAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
