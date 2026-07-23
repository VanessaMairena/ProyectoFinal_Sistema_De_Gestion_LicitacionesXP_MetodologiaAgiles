using Licitaciones.Application.DTOs;
using Licitaciones.Application.NivelesAprobacion;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Application;

public class NivelAprobacionAppServiceTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    private static (NivelAprobacionAppService servicio, NivelAprobacionRepositoryFalso repo) CrearServicio()
    {
        var repo = new NivelAprobacionRepositoryFalso();
        var servicio = new NivelAprobacionAppService(repo, new RelojFijo(Ahora));
        return (servicio, repo);
    }

    [Fact]
    public async Task Crear_nivel_valido_se_persiste()
    {
        var (servicio, _) = CrearServicio();

        var resultado = await servicio.CrearAsync(
            new CrearNivelAprobacionRequest(0.01m, 999_999.99m, "Encargado de área"), CancellationToken.None);

        Assert.Equal("Encargado de área", resultado.Aprobador);
    }

    [Fact]
    public async Task Crear_nivel_traslapado_lanza_excepcion()
    {
        var (servicio, _) = CrearServicio();

        await servicio.CrearAsync(
            new CrearNivelAprobacionRequest(0.01m, 999_999.99m, "Encargado de área"), CancellationToken.None);

        await Assert.ThrowsAsync<RangoAprobacionTraslapadoException>(() =>
            servicio.CrearAsync(new CrearNivelAprobacionRequest(500_000m, 2_000_000m, "Gerencia"), CancellationToken.None));
    }

    [Fact]
    public async Task ResolverAprobador_devuelve_el_aprobador_correcto_segun_monto()
    {
        var (servicio, _) = CrearServicio();

        await servicio.CrearAsync(
            new CrearNivelAprobacionRequest(0.01m, 999_999.99m, "Encargado de área"), CancellationToken.None);
        await servicio.CrearAsync(
            new CrearNivelAprobacionRequest(1_000_000m, null, "Junta Directiva"), CancellationToken.None);

        var resultado = await servicio.ResolverAprobadorAsync(
            new ResolverAprobadorRequest(5_000_000m), CancellationToken.None);

        Assert.Equal("Junta Directiva", resultado.Aprobador);
    }

    [Fact]
    public async Task Editar_nivel_manteniendo_rangos_validos_actualiza_datos()
    {
        var (servicio, repo) = CrearServicio();
        var creado = await servicio.CrearAsync(
            new CrearNivelAprobacionRequest(0.01m, 999_999.99m, "Encargado de área"), CancellationToken.None);

        var actualizado = await servicio.EditarAsync(
            creado.Id, new EditarNivelAprobacionRequest(0.01m, 999_999.99m, "Nuevo Aprobador"), CancellationToken.None);

        Assert.Equal("Nuevo Aprobador", actualizado.Aprobador);
    }
}
