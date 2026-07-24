using System.Net;
using System.Net.Http.Json;
using Licitaciones.Application.DTOs;
using Xunit;

namespace Licitaciones.FunctionalTests;

[Collection("Api E2E")]
public class NivelesAprobacionE2ETests
{
    private readonly HttpClient _cliente;

    public NivelesAprobacionE2ETests(ApiFactory factory)
    {
        _cliente = factory.CreateClient();
    }

    [Fact]
    public async Task Flujo_completo_crear_listar_editar_eliminar()
    {
        // Usamos un rango de montos alto y aleatorio para no chocar con
        // otros niveles creados por otras pruebas que corren en la misma base.
        var basePropia = Random.Shared.Next(1, 1_000_000) * 1000m;

        var respuestaCrear = await _cliente.PostAsJsonAsync(
            "/api/v1/niveles-aprobacion",
            new CrearNivelAprobacionRequest(basePropia, basePropia + 500_000m, "Encargado E2E"));
        Assert.Equal(HttpStatusCode.Created, respuestaCrear.StatusCode);

        var nivelCreado = await respuestaCrear.Content.ReadFromJsonAsync<NivelAprobacionDto>();
        Assert.NotNull(nivelCreado);

        var respuestaListar = await _cliente.GetAsync("/api/v1/niveles-aprobacion");
        Assert.Equal(HttpStatusCode.OK, respuestaListar.StatusCode);
        var niveles = await respuestaListar.Content.ReadFromJsonAsync<List<NivelAprobacionDto>>();
        Assert.Contains(niveles!, n => n.Id == nivelCreado!.Id);

        var respuestaEditar = await _cliente.PutAsJsonAsync(
            $"/api/v1/niveles-aprobacion/{nivelCreado!.Id}",
            new EditarNivelAprobacionRequest(basePropia, basePropia + 500_000m, "Encargado E2E Editado"));
        Assert.Equal(HttpStatusCode.OK, respuestaEditar.StatusCode);
        var nivelEditado = await respuestaEditar.Content.ReadFromJsonAsync<NivelAprobacionDto>();
        Assert.Equal("Encargado E2E Editado", nivelEditado!.Aprobador);

        var respuestaEliminar = await _cliente.DeleteAsync($"/api/v1/niveles-aprobacion/{nivelCreado.Id}");
        Assert.Equal(HttpStatusCode.NoContent, respuestaEliminar.StatusCode);
    }

    [Fact]
    public async Task Crear_rango_traslapado_devuelve_422()
    {
        var basePropia = Random.Shared.Next(1, 1_000_000) * 1000m + 999_999_000m;

        var primeraRespuesta = await _cliente.PostAsJsonAsync(
            "/api/v1/niveles-aprobacion",
            new CrearNivelAprobacionRequest(basePropia, basePropia + 100_000m, "Aprobador A"));
        Assert.Equal(HttpStatusCode.Created, primeraRespuesta.StatusCode);

        var segundaRespuesta = await _cliente.PostAsJsonAsync(
            "/api/v1/niveles-aprobacion",
            new CrearNivelAprobacionRequest(basePropia + 50_000m, basePropia + 150_000m, "Aprobador B"));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, segundaRespuesta.StatusCode);
    }

    [Fact]
    public async Task Resolver_aprobador_para_monto_sin_nivel_configurado_devuelve_422()
    {
        // Un monto negativo nunca va a estar cubierto por ningún rango válido.
        var respuesta = await _cliente.GetAsync("/api/v1/niveles-aprobacion/resolver?montoCRC=-1");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
    }
}
