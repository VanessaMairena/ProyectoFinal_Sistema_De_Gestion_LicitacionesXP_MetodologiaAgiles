using Licitaciones.Application.DTOs;
using Licitaciones.Application.Licitaciones;
using Licitaciones.Domain.Enumeraciones;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/licitaciones")]
public class LicitacionesController : ControllerBase
{
    private readonly LicitacionAppService _servicio;

    public LicitacionesController(LicitacionAppService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] EstadoLicitacion? estado, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var resultado = await _servicio.ListarAsync(estado, pagina, tamanoPagina, ct);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LicitacionDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var licitacion = await _servicio.ObtenerAsync(id, ct);
        return Ok(licitacion);
    }

    [HttpPost]
    public async Task<ActionResult<LicitacionDto>> Crear(
        [FromBody] CrearLicitacionRequest request, CancellationToken ct)
    {
        var licitacion = await _servicio.CrearAsync(request, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = licitacion.Id }, licitacion);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LicitacionDto>> Editar(
        Guid id, [FromBody] EditarLicitacionRequest request, CancellationToken ct)
    {
        var licitacion = await _servicio.EditarAsync(id, request, ct);
        return Ok(licitacion);
    }

    [HttpPost("{id:guid}/estado")]
    public async Task<ActionResult<LicitacionDto>> CambiarEstado(
        Guid id, [FromBody] CambiarEstadoLicitacionRequest request, CancellationToken ct)
    {
        var licitacion = await _servicio.CambiarEstadoAsync(id, request, ct);
        return Ok(licitacion);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        return NoContent();
    }
}
