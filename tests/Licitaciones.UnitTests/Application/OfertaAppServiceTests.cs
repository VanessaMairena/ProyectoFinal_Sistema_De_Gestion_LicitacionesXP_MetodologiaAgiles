using Licitaciones.Application.DTOs;
using Licitaciones.Application.Ofertas;
using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Application;

public class OfertaAppServiceTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    private static (OfertaAppService servicio, LicitacionRepositoryFalso licitaciones,
        ProveedorRepositoryFalso proveedores, OfertaRepositoryFalso ofertas) CrearServicio(DateTimeOffset ahora)
    {
        var licitaciones = new LicitacionRepositoryFalso();
        var proveedores = new ProveedorRepositoryFalso();
        var ofertas = new OfertaRepositoryFalso();
        var servicio = new OfertaAppService(ofertas, licitaciones, proveedores, new RelojFijo(ahora));
        return (servicio, licitaciones, proveedores, ofertas);
    }

    [Fact]
    public async Task Registrar_oferta_valida_se_persiste_correctamente()
    {
        var (servicio, licitaciones, proveedores, _) = CrearServicio(Ahora);

        var licitacion = Licitacion.Crear("LIC-300", "Título", Ahora.AddDays(1), 1_000_000m, Ahora);
        licitacion.Publicar(Ahora.AddMinutes(1));
        licitaciones.Datos.Add(licitacion);

        var proveedor = Proveedor.Registrar("Proveedor de Prueba", Ahora);
        proveedores.Datos.Add(proveedor);

        var resultado = await servicio.RegistrarAsync(
            licitacion.Id, new RegistrarOfertaRequest(proveedor.Id, 800_000m), CancellationToken.None);

        Assert.Equal(800_000m, resultado.MontoOfertadoCRC);
        Assert.Equal("Proveedor de Prueba", resultado.NombreProveedor);
    }

    [Fact]
    public async Task Registrar_segunda_oferta_del_mismo_proveedor_lanza_excepcion()
    {
        var (servicio, licitaciones, proveedores, _) = CrearServicio(Ahora);

        var licitacion = Licitacion.Crear("LIC-301", "Título", Ahora.AddDays(1), 1_000_000m, Ahora);
        licitacion.Publicar(Ahora.AddMinutes(1));
        licitaciones.Datos.Add(licitacion);

        var proveedor = Proveedor.Registrar("Proveedor Repetido", Ahora);
        proveedores.Datos.Add(proveedor);

        await servicio.RegistrarAsync(licitacion.Id, new RegistrarOfertaRequest(proveedor.Id, 800_000m), CancellationToken.None);

        await Assert.ThrowsAsync<OfertaDuplicadaException>(() =>
            servicio.RegistrarAsync(licitacion.Id, new RegistrarOfertaRequest(proveedor.Id, 700_000m), CancellationToken.None));
    }

    [Fact]
    public async Task ObtenerMejorOferta_devuelve_la_de_menor_monto_con_nombre_de_proveedor()
    {
        var (servicio, licitaciones, proveedores, _) = CrearServicio(Ahora);

        var licitacion = Licitacion.Crear("LIC-302", "Título", Ahora.AddDays(1), 1_000_000m, Ahora);
        licitacion.Publicar(Ahora.AddMinutes(1));
        licitaciones.Datos.Add(licitacion);

        var proveedorA = Proveedor.Registrar("Proveedor A", Ahora);
        var proveedorB = Proveedor.Registrar("Proveedor B", Ahora);
        proveedores.Datos.Add(proveedorA);
        proveedores.Datos.Add(proveedorB);

        await servicio.RegistrarAsync(licitacion.Id, new RegistrarOfertaRequest(proveedorA.Id, 950_000m), CancellationToken.None);
        await servicio.RegistrarAsync(licitacion.Id, new RegistrarOfertaRequest(proveedorB.Id, 850_000m), CancellationToken.None);

        var resultado = await servicio.ObtenerMejorOfertaAsync(licitacion.Id, CancellationToken.None);

        Assert.Equal("Proveedor B", resultado.MejorOferta!.NombreProveedor);
        Assert.Equal("OfertaConveniente", resultado.Clasificacion);
    }
}
