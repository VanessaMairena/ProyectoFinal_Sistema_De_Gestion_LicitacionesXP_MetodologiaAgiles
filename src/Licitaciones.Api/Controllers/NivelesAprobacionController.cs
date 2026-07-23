using Licitaciones.Application.DTOs;
using Licitaciones.Application.NivelesAprobacion;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/niveles-aprobacion")]
public class NivelesAprobacionController : ControllerBase
{
    private readonly NivelAprobacionAppService _servicio;

    public NivelesAprobacionController(NivelAprobacionAppService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NivelAprobacionDto>>> Listar(CancellationToken ct) =>
        Ok(await _servicio.ListarAsync(ct));

    [HttpPost]
    public async Task<ActionResult<NivelAprobacionDto>> Crear(
        [FromBody] CrearNivelAprobacionRequest request, CancellationToken ct)
    {
        var nivel = await _servicio.CrearAsync(request, ct);
        return CreatedAtAction(nameof(Listar), nivel);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<NivelAprobacionDto>> Editar(
        Guid id, [FromBody] EditarNivelAprobacionRequest request, CancellationToken ct) =>
        Ok(await _servicio.EditarAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        return NoContent();
    }

    [HttpGet("resolver")]
    public async Task<ActionResult<AprobadorResueltoDto>> Resolver([FromQuery] decimal montoCRC, CancellationToken ct) =>
        Ok(await _servicio.ResolverAprobadorAsync(new ResolverAprobadorRequest(montoCRC), ct));
}
