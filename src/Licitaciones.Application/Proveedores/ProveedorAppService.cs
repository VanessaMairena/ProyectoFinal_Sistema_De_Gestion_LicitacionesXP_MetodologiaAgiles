using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;

namespace Licitaciones.Application.Proveedores;

public sealed class ProveedorAppService
{
    private readonly IProveedorRepository _repositorio;
    private readonly TimeProvider _reloj;

    public ProveedorAppService(IProveedorRepository repositorio, TimeProvider reloj)
    {
        _repositorio = repositorio;
        _reloj = reloj;
    }

    public async Task<ProveedorDto> RegistrarAsync(RegistrarProveedorRequest request, CancellationToken ct)
    {
        var ahora = _reloj.GetUtcNow();
        var proveedor = Proveedor.Registrar(request.Nombre, ahora);

        if (await _repositorio.ExisteConNombreNormalizadoAsync(proveedor.NombreNormalizado, null, ct))
            throw new Domain.Excepciones.DomainException(
                "proveedor.nombre_duplicado", "Ya existe un proveedor con un nombre equivalente.");

        await _repositorio.AgregarAsync(proveedor, ct);
        await _repositorio.GuardarCambiosAsync(ct);

        return AProveedorDto(proveedor);
    }

    public async Task<ProveedorDto> EditarAsync(Guid id, EditarProveedorRequest request, CancellationToken ct)
    {
        var proveedor = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Proveedor", id);

        var ahora = _reloj.GetUtcNow();
        var nombreTemporal = Domain.ValueObjects.NombreProveedor.Crear(request.Nombre);

        if (await _repositorio.ExisteConNombreNormalizadoAsync(nombreTemporal.ValorNormalizado, id, ct))
            throw new Domain.Excepciones.DomainException(
                "proveedor.nombre_duplicado", "Ya existe un proveedor con un nombre equivalente.");

        proveedor.Editar(request.Nombre, ahora);
        await _repositorio.GuardarCambiosAsync(ct);

        return AProveedorDto(proveedor);
    }

    public async Task<ProveedorDto> ObtenerAsync(Guid id, CancellationToken ct)
    {
        var proveedor = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Proveedor", id);

        return AProveedorDto(proveedor);
    }

    public async Task<ResultadoPaginado<ProveedorDto>> ListarAsync(
        string? filtroNombre, int pagina, int tamanoPagina, CancellationToken ct)
    {
        var parametros = new ParametrosPaginacion(pagina, tamanoPagina);
        var (items, total) = await _repositorio.ListarAsync(
            filtroNombre, parametros.Pagina, parametros.TamanoPagina, ct);

        return new ResultadoPaginado<ProveedorDto>(
            items.Select(AProveedorDto).ToList(), total, parametros.Pagina, parametros.TamanoPagina);
    }

    /// <summary>
    /// El borrado siempre es lógico (bandera Eliminado), independientemente de
    /// si el proveedor tiene ofertas relacionadas, para no perder el historial
    /// de ofertas ya presentadas (regla HU-02).
    /// </summary>
    public async Task EliminarAsync(Guid id, CancellationToken ct)
    {
        var proveedor = await _repositorio.ObtenerPorIdAsync(id, ct)
            ?? throw new RecursoNoEncontradoException("Proveedor", id);

        proveedor.MarcarComoEliminado(_reloj.GetUtcNow());
        await _repositorio.GuardarCambiosAsync(ct);
    }

    private static ProveedorDto AProveedorDto(Proveedor p) =>
        new(p.Id, p.Nombre, p.CreatedAt, p.UpdatedAt);
}
