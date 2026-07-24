using Licitaciones.Application.DTOs;
using Licitaciones.Application.Proveedores;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

/// <summary>
/// Administra el catálogo de proveedores que pueden presentar ofertas
/// en las licitaciones.
/// </summary>
[ApiController]
[Route("api/v1/proveedores")]
[Produces("application/json")]
public class ProveedoresController : ControllerBase
{
    private readonly ProveedorAppService _servicio;

    public ProveedoresController(ProveedorAppService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista proveedores activos, con filtro por nombre y paginación.</summary>
    /// <param name="filtro">Texto a buscar dentro del nombre del proveedor (opcional).</param>
    /// <param name="pagina">Número de página, empezando en 1.</param>
    /// <param name="tamanoPagina">Cantidad de resultados por página (máximo 100).</param>
    /// <response code="200">Listado paginado de proveedores.</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? filtro, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var resultado = await _servicio.ListarAsync(filtro, pagina, tamanoPagina, ct);
        return Ok(resultado);
    }

    /// <summary>Obtiene un proveedor por su identificador.</summary>
    /// <param name="id">Identificador único (GUID) del proveedor.</param>
    /// <response code="200">El proveedor encontrado.</response>
    /// <response code="404">No existe un proveedor activo con ese Id.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var proveedor = await _servicio.ObtenerAsync(id, ct);
        return Ok(proveedor);
    }

    /// <summary>Registra un nuevo proveedor.</summary>
    /// <remarks>
    /// El nombre solo admite letras, números, espacios, punto, coma y paréntesis.
    /// La unicidad se valida ignorando mayúsculas/minúsculas, tildes y espacios repetidos
    /// (por ejemplo, "Empresa Central" y "empresa   central" se consideran el mismo proveedor).
    /// </remarks>
    /// <response code="201">Proveedor creado correctamente.</response>
    /// <response code="422">El nombre es inválido o ya existe un proveedor equivalente.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ProveedorDto>> Registrar(
        [FromBody] RegistrarProveedorRequest request, CancellationToken ct)
    {
        var proveedor = await _servicio.RegistrarAsync(request, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = proveedor.Id }, proveedor);
    }

    /// <summary>Edita el nombre de un proveedor existente.</summary>
    /// <param name="id">Identificador único (GUID) del proveedor.</param>
    /// <response code="200">Proveedor actualizado correctamente.</response>
    /// <response code="404">No existe un proveedor activo con ese Id.</response>
    /// <response code="422">El nombre es inválido o ya existe otro proveedor equivalente.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ProveedorDto>> Editar(
        Guid id, [FromBody] EditarProveedorRequest request, CancellationToken ct)
    {
        var proveedor = await _servicio.EditarAsync(id, request, ct);
        return Ok(proveedor);
    }

    /// <summary>Elimina (lógicamente) un proveedor.</summary>
    /// <remarks>
    /// El borrado es siempre lógico: el registro se marca como eliminado pero
    /// se conserva en la base de datos junto con el historial de sus ofertas.
    /// </remarks>
    /// <param name="id">Identificador único (GUID) del proveedor.</param>
    /// <response code="204">Proveedor eliminado correctamente.</response>
    /// <response code="404">No existe un proveedor activo con ese Id.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        return NoContent();
    }
}
