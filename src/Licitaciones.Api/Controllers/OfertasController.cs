using Licitaciones.Application.DTOs;
using Licitaciones.Application.Ofertas;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/licitaciones/{licitacionId:guid}/ofertas")]
public class OfertasController : ControllerBase
{
    private readonly OfertaAppService _servicio;

    public OfertasController(OfertaAppService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OfertaDto>>> Listar(Guid licitacionId, CancellationToken ct)
    {
        var ofertas = await _servicio.ListarPorLicitacionAsync(licitacionId, ct);
        return Ok(ofertas);
    }

    [HttpGet("mejor-oferta")]
    public async Task<ActionResult<MejorOfertaDto>> ObtenerMejorOferta(Guid licitacionId, CancellationToken ct)
    {
        var resultado = await _servicio.ObtenerMejorOfertaAsync(licitacionId, ct);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<OfertaDto>> Registrar(
        Guid licitacionId, [FromBody] RegistrarOfertaRequest request, CancellationToken ct)
    {
        var oferta = await _servicio.RegistrarAsync(licitacionId, request, ct);
        return CreatedAtAction(nameof(Listar), new { licitacionId }, oferta);
    }
}
