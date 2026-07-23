using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;

namespace Licitaciones.UnitTests.Application;

public sealed class ProveedorRepositoryFalso : IProveedorRepository
{
    public List<Proveedor> Datos { get; } = new();

    public Task<Proveedor?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Datos.FirstOrDefault(p => p.Id == id && !p.Eliminado));

    public Task<bool> ExisteConNombreNormalizadoAsync(string nombreNormalizado, Guid? excluirId, CancellationToken ct) =>
        Task.FromResult(Datos.Any(p => p.NombreNormalizado == nombreNormalizado && !p.Eliminado &&
                                        (excluirId == null || p.Id != excluirId)));

    public Task<(IReadOnlyList<Proveedor> Items, int Total)> ListarAsync(
        string? filtroNombre, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var items = Datos.Where(p => !p.Eliminado).ToList();
        return Task.FromResult(((IReadOnlyList<Proveedor>)items, items.Count));
    }

    public Task<bool> TieneOfertasRelacionadasAsync(Guid proveedorId, CancellationToken ct) =>
        Task.FromResult(false);

    public Task<IReadOnlyList<Proveedor>> ListarActivosParaSeleccionAsync(CancellationToken ct) =>
        Task.FromResult((IReadOnlyList<Proveedor>)Datos.Where(p => !p.Eliminado).ToList());

    public Task AgregarAsync(Proveedor proveedor, CancellationToken ct)
    {
        Datos.Add(proveedor);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
}

public sealed class LicitacionRepositoryFalso : ILicitacionRepository
{
    public List<Licitacion> Datos { get; } = new();

    public Task<Licitacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Datos.FirstOrDefault(l => l.Id == id && !l.Eliminada));

    public Task<bool> ExisteConCodigoNormalizadoAsync(string codigoNormalizado, Guid? excluirId, CancellationToken ct) =>
        Task.FromResult(Datos.Any(l => l.CodigoNormalizado == codigoNormalizado && !l.Eliminada &&
                                        (excluirId == null || l.Id != excluirId)));

    public Task<(IReadOnlyList<Licitacion> Items, int Total)> ListarAsync(
        EstadoLicitacion? filtroEstado, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var items = Datos.Where(l => !l.Eliminada).ToList();
        return Task.FromResult(((IReadOnlyList<Licitacion>)items, items.Count));
    }

    public Task<bool> TieneOfertasRelacionadasAsync(Guid licitacionId, CancellationToken ct) =>
        Task.FromResult(false);

    public Task AgregarAsync(Licitacion licitacion, CancellationToken ct)
    {
        Datos.Add(licitacion);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
}

public sealed class OfertaRepositoryFalso : IOfertaRepository
{
    public List<Oferta> Datos { get; } = new();

    public Task<Oferta?> ObtenerPorIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Datos.FirstOrDefault(o => o.Id == id));

    public Task<IReadOnlyList<Oferta>> ListarPorLicitacionAsync(Guid licitacionId, CancellationToken ct) =>
        Task.FromResult((IReadOnlyList<Oferta>)Datos.Where(o => o.LicitacionId == licitacionId).ToList());

    public Task<bool> ExisteOfertaDeProveedorAsync(Guid licitacionId, Guid proveedorId, CancellationToken ct) =>
        Task.FromResult(Datos.Any(o => o.LicitacionId == licitacionId && o.ProveedorId == proveedorId));

    public Task<bool> ExisteAlgunaOfertaDeProveedorAsync(Guid proveedorId, CancellationToken ct) =>
        Task.FromResult(Datos.Any(o => o.ProveedorId == proveedorId));

    public Task<bool> ExisteAlgunaOfertaDeLicitacionAsync(Guid licitacionId, CancellationToken ct) =>
        Task.FromResult(Datos.Any(o => o.LicitacionId == licitacionId));

    public Task AgregarAsync(Oferta oferta, CancellationToken ct)
    {
        Datos.Add(oferta);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct) => Task.CompletedTask;
}
