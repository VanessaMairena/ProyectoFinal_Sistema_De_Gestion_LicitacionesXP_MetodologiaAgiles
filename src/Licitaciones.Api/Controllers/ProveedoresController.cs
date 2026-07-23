using Licitaciones.Application.DTOs;
using Licitaciones.Application.Proveedores;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/proveedores")]
public class ProveedoresController : ControllerBase
{
    private readonly ProveedorAppService _servicio;

    public ProveedoresController(ProveedorAppService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? filtro, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var resultado = await _servicio.ListarAsync(filtro, pagina, tamanoPagina, ct);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProveedorDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var proveedor = await _servicio.ObtenerAsync(id, ct);
        return Ok(proveedor);
    }

    [HttpPost]
    public async Task<ActionResult<ProveedorDto>> Registrar(
        [FromBody] RegistrarProveedorRequest request, CancellationToken ct)
    {
        var proveedor = await _servicio.RegistrarAsync(request, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = proveedor.Id }, proveedor);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProveedorDto>> Editar(
        Guid id, [FromBody] EditarProveedorRequest request, CancellationToken ct)
    {
        var proveedor = await _servicio.EditarAsync(id, request, ct);
        return Ok(proveedor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        return NoContent();
    }
}
