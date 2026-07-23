using Licitaciones.Application.DTOs;
using Licitaciones.Application.TipoCambio;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Application;

public class TipoCambioAppServiceTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    private static (TipoCambioAppService servicio, TipoCambioRepositoryFalso repo) CrearServicio()
    {
        var repo = new TipoCambioRepositoryFalso();
        var servicio = new TipoCambioAppService(repo, new RelojFijo(Ahora));
        return (servicio, repo);
    }

    [Fact]
    public async Task Registrar_primer_tipo_de_cambio_queda_activo()
    {
        var (servicio, _) = CrearServicio();

        var resultado = await servicio.RegistrarYActivarAsync(
            new CrearTipoCambioRequest(520m, Ahora), CancellationToken.None);

        Assert.True(resultado.Activo);
    }

    [Fact]
    public async Task Registrar_segundo_tipo_de_cambio_desactiva_el_anterior()
    {
        var (servicio, repo) = CrearServicio();

        var primero = await servicio.RegistrarYActivarAsync(
            new CrearTipoCambioRequest(520m, Ahora), CancellationToken.None);
        await servicio.RegistrarYActivarAsync(
            new CrearTipoCambioRequest(530m, Ahora.AddDays(1)), CancellationToken.None);

        var primeroActualizado = repo.Datos.First(t => t.Id == primero.Id);
        Assert.False(primeroActualizado.Activo);
    }

    [Fact]
    public async Task Convertir_usa_el_tipo_de_cambio_activo()
    {
        var (servicio, _) = CrearServicio();
        await servicio.RegistrarYActivarAsync(new CrearTipoCambioRequest(520m, Ahora), CancellationToken.None);

        var resultado = await servicio.ConvertirAsync(52_000m, CancellationToken.None);

        Assert.Equal(100m, resultado.MontoUSD);
    }

    [Fact]
    public async Task Convertir_sin_tipo_de_cambio_activo_lanza_excepcion()
    {
        var (servicio, _) = CrearServicio();

        await Assert.ThrowsAsync<DomainException>(() =>
            servicio.ConvertirAsync(1000m, CancellationToken.None));
    }
}
