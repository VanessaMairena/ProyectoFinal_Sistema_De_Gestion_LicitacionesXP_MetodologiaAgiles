using System.Net;
using System.Net.Http.Json;
using Licitaciones.Application.DTOs;
using Xunit;

namespace Licitaciones.FunctionalTests;

[Collection("Api E2E")]
public class TipoCambioE2ETests
{
    private readonly HttpClient _cliente;

    public TipoCambioE2ETests(ApiFactory factory)
    {
        _cliente = factory.CreateClient();
    }

    [Fact]
    public async Task Registrar_tipo_de_cambio_y_convertir_un_monto()
    {
        var respuestaCrear = await _cliente.PostAsJsonAsync(
            "/api/v1/tipo-cambio", new CrearTipoCambioRequest(520m, DateTimeOffset.UtcNow));
        Assert.Equal(HttpStatusCode.OK, respuestaCrear.StatusCode);

        var tipoCambioCreado = await respuestaCrear.Content.ReadFromJsonAsync<TipoCambioDto>();
        Assert.True(tipoCambioCreado!.Activo);

        var respuestaConvertir = await _cliente.GetAsync("/api/v1/tipo-cambio/convertir?montoCRC=52000");
        Assert.Equal(HttpStatusCode.OK, respuestaConvertir.StatusCode);

        var conversion = await respuestaConvertir.Content.ReadFromJsonAsync<ConversionDto>();
        Assert.Equal(100m, conversion!.MontoUSD);
    }

    [Fact]
    public async Task Registrar_segundo_tipo_de_cambio_desactiva_el_anterior()
    {
        var primero = await (await _cliente.PostAsJsonAsync(
            "/api/v1/tipo-cambio", new CrearTipoCambioRequest(500m, DateTimeOffset.UtcNow)))
            .Content.ReadFromJsonAsync<TipoCambioDto>();

        await _cliente.PostAsJsonAsync(
            "/api/v1/tipo-cambio", new CrearTipoCambioRequest(510m, DateTimeOffset.UtcNow.AddMinutes(1)));

        var respuestaListar = await _cliente.GetAsync("/api/v1/tipo-cambio");
        var lista = await respuestaListar.Content.ReadFromJsonAsync<List<TipoCambioDto>>();

        var primeroActualizado = lista!.First(t => t.Id == primero!.Id);
        Assert.False(primeroActualizado.Activo);
    }
}
