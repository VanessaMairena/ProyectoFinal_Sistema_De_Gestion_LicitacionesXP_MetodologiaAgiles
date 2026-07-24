using Licitaciones.Application.DTOs;
using Licitaciones.Application.TipoCambio;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

/// <summary>
/// Administra el tipo de cambio CRC/USD usado para mostrar montos de
/// forma referencial en dólares. Los valores oficiales del sistema
/// siempre se guardan en colones (CRC).
/// </summary>
[ApiController]
[Route("api/v1/tipo-cambio")]
[Produces("application/json")]
public class TipoCambioController : ControllerBase
{
    private readonly TipoCambioAppService _servicio;

    public TipoCambioController(TipoCambioAppService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista todos los tipos de cambio registrados (histórico + activo).</summary>
    /// <response code="200">Listado de tipos de cambio, del más reciente al más antiguo.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TipoCambioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TipoCambioDto>>> Listar(CancellationToken ct) =>
        Ok(await _servicio.ListarAsync(ct));

    /// <summary>Registra un nuevo tipo de cambio y lo activa de inmediato.</summary>
    /// <remarks>
    /// Al registrar un tipo de cambio nuevo, el anterior se desactiva
    /// automáticamente. Solo puede haber un tipo de cambio activo a la vez.
    /// </remarks>
    /// <response code="200">Tipo de cambio registrado y activado correctamente.</response>
    /// <response code="422">El valor del tipo de cambio no es válido (debe ser mayor que cero).</response>
    [HttpPost]
    [ProducesResponseType(typeof(TipoCambioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TipoCambioDto>> RegistrarYActivar(
        [FromBody] CrearTipoCambioRequest request, CancellationToken ct) =>
        Ok(await _servicio.RegistrarYActivarAsync(request, ct));

    /// <summary>Convierte un monto en colones a su equivalente en dólares.</summary>
    /// <param name="montoCRC">Monto en colones a convertir.</param>
    /// <remarks>
    /// Usa el tipo de cambio activo en este momento. La conversión es
    /// puramente referencial: los montos oficiales del sistema siguen en CRC.
    /// </remarks>
    /// <response code="200">El monto convertido, junto con el tipo de cambio utilizado.</response>
    /// <response code="422">No hay ningún tipo de cambio activo configurado todavía.</response>
    [HttpGet("convertir")]
    [ProducesResponseType(typeof(ConversionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ConversionDto>> Convertir([FromQuery] decimal montoCRC, CancellationToken ct) =>
        Ok(await _servicio.ConvertirAsync(montoCRC, ct));
}
