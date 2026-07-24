using Licitaciones.Application.DTOs;
using Licitaciones.Application.NivelesAprobacion;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

/// <summary>
/// Administra los rangos de monto que determinan quién debe aprobar
/// cada licitación (por ejemplo: hasta ₡1.000.000 aprueba el Encargado
/// de área; montos mayores requieren Gerencia o Junta Directiva).
/// </summary>
[ApiController]
[Route("api/v1/niveles-aprobacion")]
[Produces("application/json")]
public class NivelesAprobacionController : ControllerBase
{
    private readonly NivelAprobacionAppService _servicio;

    public NivelesAprobacionController(NivelAprobacionAppService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista todos los niveles de aprobación configurados.</summary>
    /// <response code="200">Listado de niveles, ordenado por monto mínimo.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NivelAprobacionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<NivelAprobacionDto>>> Listar(CancellationToken ct) =>
        Ok(await _servicio.ListarAsync(ct));

    /// <summary>Crea un nuevo nivel de aprobación (rango de monto + aprobador).</summary>
    /// <remarks>
    /// Dejá <c>montoMaximoCRC</c> en <c>null</c> para definir el rango abierto
    /// (sin límite superior). Solo puede existir un rango abierto a la vez, y
    /// los rangos no pueden traslaparse entre sí.
    /// </remarks>
    /// <response code="201">Nivel de aprobación creado correctamente.</response>
    /// <response code="422">El rango es inválido, se traslapa con otro existente, o ya hay un rango abierto.</response>
    [HttpPost]
    [ProducesResponseType(typeof(NivelAprobacionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<NivelAprobacionDto>> Crear(
        [FromBody] CrearNivelAprobacionRequest request, CancellationToken ct)
    {
        var nivel = await _servicio.CrearAsync(request, ct);
        return CreatedAtAction(nameof(Listar), nivel);
    }

    /// <summary>Edita un nivel de aprobación existente.</summary>
    /// <param name="id">Identificador único (GUID) del nivel de aprobación.</param>
    /// <response code="200">Nivel actualizado correctamente.</response>
    /// <response code="404">No existe un nivel con ese Id.</response>
    /// <response code="422">El rango es inválido o se traslapa con otro existente.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(NivelAprobacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<NivelAprobacionDto>> Editar(
        Guid id, [FromBody] EditarNivelAprobacionRequest request, CancellationToken ct) =>
        Ok(await _servicio.EditarAsync(id, request, ct));

    /// <summary>Elimina un nivel de aprobación.</summary>
    /// <param name="id">Identificador único (GUID) del nivel de aprobación.</param>
    /// <response code="204">Nivel eliminado correctamente.</response>
    /// <response code="404">No existe un nivel con ese Id.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        return NoContent();
    }

    /// <summary>Resuelve automáticamente quién debe aprobar un monto dado.</summary>
    /// <param name="montoCRC">Monto en colones a evaluar.</param>
    /// <response code="200">El aprobador correspondiente al monto.</response>
    /// <response code="422">No hay ningún nivel configurado que cubra ese monto.</response>
    [HttpGet("resolver")]
    [ProducesResponseType(typeof(AprobadorResueltoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AprobadorResueltoDto>> Resolver([FromQuery] decimal montoCRC, CancellationToken ct) =>
        Ok(await _servicio.ResolverAprobadorAsync(new ResolverAprobadorRequest(montoCRC), ct));
}
