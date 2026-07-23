using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Interfaces;
using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;
using Licitaciones.Domain.Servicios;

namespace Licitaciones.Application.Ofertas;

public sealed class OfertaAppService
{
    private readonly IOfertaRepository _ofertas;
    private readonly ILicitacionRepository _licitaciones;
    private readonly IProveedorRepository _proveedores;
    private readonly TimeProvider _reloj;

    public OfertaAppService(
        IOfertaRepository ofertas,
        ILicitacionRepository licitaciones,
        IProveedorRepository proveedores,
        TimeProvider reloj)
    {
        _ofertas = ofertas;
        _licitaciones = licitaciones;
        _proveedores = proveedores;
        _reloj = reloj;
    }

    public async Task<OfertaDto> RegistrarAsync(Guid licitacionId, RegistrarOfertaRequest request, CancellationToken ct)
    {
        var licitacion = await _licitaciones.ObtenerPorIdAsync(licitacionId, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", licitacionId);

        var proveedor = await _proveedores.ObtenerPorIdAsync(request.ProveedorId, ct)
            ?? throw new RecursoNoEncontradoException("Proveedor", request.ProveedorId);

        // Un proveedor no puede registrar más de una oferta para la misma licitación.
        if (await _ofertas.ExisteOfertaDeProveedorAsync(licitacionId, request.ProveedorId, ct))
            throw new OfertaDuplicadaException();

        var ahora = _reloj.GetUtcNow();
        var oferta = Oferta.Registrar(licitacion, request.ProveedorId, request.MontoOfertadoCRC, ahora);

        await _ofertas.AgregarAsync(oferta, ct);
        await _ofertas.GuardarCambiosAsync(ct);

        return AOfertaDto(oferta, proveedor.Nombre);
    }

    public async Task<IReadOnlyList<OfertaDto>> ListarPorLicitacionAsync(Guid licitacionId, CancellationToken ct)
    {
        _ = await _licitaciones.ObtenerPorIdAsync(licitacionId, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", licitacionId);

        var ofertas = await _ofertas.ListarPorLicitacionAsync(licitacionId, ct);
        var resultado = new List<OfertaDto>(ofertas.Count);

        foreach (var oferta in ofertas.OrderBy(o => o.MontoOfertadoCRC).ThenBy(o => o.FechaRegistro))
        {
            var proveedor = await _proveedores.ObtenerPorIdAsync(oferta.ProveedorId, ct);
            resultado.Add(AOfertaDto(oferta, proveedor?.Nombre ?? "(proveedor eliminado)"));
        }

        return resultado;
    }

    public async Task<MejorOfertaDto> ObtenerMejorOfertaAsync(Guid licitacionId, CancellationToken ct)
    {
        var licitacion = await _licitaciones.ObtenerPorIdAsync(licitacionId, ct)
            ?? throw new RecursoNoEncontradoException("Licitación", licitacionId);

        var ofertas = await _ofertas.ListarPorLicitacionAsync(licitacionId, ct);
        var resultado = ServicioMejorOferta.Calcular(licitacion, ofertas);

        OfertaDto? mejorDto = null;
        if (resultado.MejorOferta is not null)
        {
            var proveedor = await _proveedores.ObtenerPorIdAsync(resultado.MejorOferta.ProveedorId, ct);
            mejorDto = AOfertaDto(resultado.MejorOferta, proveedor?.Nombre ?? "(proveedor eliminado)");
        }

        return new MejorOfertaDto(mejorDto, resultado.Clasificacion.ToString(), resultado.PorcentajeAhorro);
    }

    private static OfertaDto AOfertaDto(Oferta o, string nombreProveedor) =>
        new(o.Id, o.LicitacionId, o.ProveedorId, nombreProveedor, o.MontoOfertadoCRC, o.FechaRegistro);
}
