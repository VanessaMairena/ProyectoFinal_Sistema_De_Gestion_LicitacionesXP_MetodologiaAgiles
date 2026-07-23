using Licitaciones.Application.DTOs;
using Licitaciones.Application.TipoCambio;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/tipo-cambio")]
public class TipoCambioController : ControllerBase
{
    private readonly TipoCambioAppService _servicio;

    public TipoCambioController(TipoCambioAppService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TipoCambioDto>>> Listar(CancellationToken ct) =>
        Ok(await _servicio.ListarAsync(ct));

    [HttpPost]
    public async Task<ActionResult<TipoCambioDto>> RegistrarYActivar(
        [FromBody] CrearTipoCambioRequest request, CancellationToken ct) =>
        Ok(await _servicio.RegistrarYActivarAsync(request, ct));

    [HttpGet("convertir")]
    public async Task<ActionResult<ConversionDto>> Convertir([FromQuery] decimal montoCRC, CancellationToken ct) =>
        Ok(await _servicio.ConvertirAsync(montoCRC, ct));
}
