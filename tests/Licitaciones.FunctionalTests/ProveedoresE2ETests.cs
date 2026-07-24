using System.Net;
using System.Net.Http.Json;
using Licitaciones.Application.DTOs;
using Xunit;

namespace Licitaciones.FunctionalTests;

/// <summary>
/// Pruebas de extremo a extremo: cada una levanta un cliente HTTP real
/// contra la Api completa (controlador, servicio de aplicación, EF Core
/// y PostgreSQL real), sin simular ninguna capa.
/// </summary>
[Collection("Api E2E")]
public class ProveedoresE2ETests
{
    private readonly HttpClient _cliente;

    public ProveedoresE2ETests(ApiFactory factory)
    {
        _cliente = factory.CreateClient();
    }

    [Fact]
    public async Task Flujo_completo_crear_consultar_editar_eliminar()
    {
        var nombreUnico = $"Proveedor E2E {Guid.NewGuid():N}";

        // 1. Crear
        var respuestaCrear = await _cliente.PostAsJsonAsync(
            "/api/v1/proveedores", new RegistrarProveedorRequest(nombreUnico));
        Assert.Equal(HttpStatusCode.Created, respuestaCrear.StatusCode);

        var proveedorCreado = await respuestaCrear.Content.ReadFromJsonAsync<ProveedorDto>();
        Assert.NotNull(proveedorCreado);
        Assert.Equal(nombreUnico, proveedorCreado!.Nombre);

        // 2. Consultar por Id
        var respuestaConsulta = await _cliente.GetAsync($"/api/v1/proveedores/{proveedorCreado.Id}");
        Assert.Equal(HttpStatusCode.OK, respuestaConsulta.StatusCode);

        // 3. Editar
        var nombreEditado = $"{nombreUnico} (editado)";
        var respuestaEditar = await _cliente.PutAsJsonAsync(
            $"/api/v1/proveedores/{proveedorCreado.Id}", new EditarProveedorRequest(nombreEditado));
        Assert.Equal(HttpStatusCode.OK, respuestaEditar.StatusCode);

        var proveedorEditado = await respuestaEditar.Content.ReadFromJsonAsync<ProveedorDto>();
        Assert.Equal(nombreEditado, proveedorEditado!.Nombre);

        // 4. Eliminar
        var respuestaEliminar = await _cliente.DeleteAsync($"/api/v1/proveedores/{proveedorCreado.Id}");
        Assert.Equal(HttpStatusCode.NoContent, respuestaEliminar.StatusCode);

        // 5. Ya no debe encontrarse (borrado lógico)
        var respuestaTrasEliminar = await _cliente.GetAsync($"/api/v1/proveedores/{proveedorCreado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, respuestaTrasEliminar.StatusCode);
    }

    [Fact]
    public async Task Registrar_nombre_duplicado_devuelve_422()
    {
        var nombreUnico = $"Proveedor Duplicado {Guid.NewGuid():N}";

        var primeraRespuesta = await _cliente.PostAsJsonAsync(
            "/api/v1/proveedores", new RegistrarProveedorRequest(nombreUnico));
        Assert.Equal(HttpStatusCode.Created, primeraRespuesta.StatusCode);

        // Mismo nombre, con mayúsculas/espacios distintos: debe rechazarse igual.
        var segundaRespuesta = await _cliente.PostAsJsonAsync(
            "/api/v1/proveedores", new RegistrarProveedorRequest($"  {nombreUnico.ToUpperInvariant()}  "));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, segundaRespuesta.StatusCode);
    }

    [Fact]
    public async Task Consultar_id_inexistente_devuelve_404()
    {
        var respuesta = await _cliente.GetAsync($"/api/v1/proveedores/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
