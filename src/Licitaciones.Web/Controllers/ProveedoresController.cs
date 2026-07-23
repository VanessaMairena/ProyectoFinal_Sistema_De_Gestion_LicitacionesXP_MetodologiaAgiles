using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Proveedores;
using Licitaciones.Domain.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public class ProveedoresController : Controller
{
    private readonly ProveedorAppService _servicio;

    public ProveedoresController(ProveedorAppService servicio)
    {
        _servicio = servicio;
    }

    public async Task<IActionResult> Index(string? filtro, int pagina = 1, CancellationToken ct = default)
    {
        var resultado = await _servicio.ListarAsync(filtro, pagina, 20, ct);
        ViewBag.Filtro = filtro;
        return View(resultado);
    }

    public IActionResult Create() => View(new RegistrarProveedorRequest(string.Empty));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegistrarProveedorRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            await _servicio.RegistrarAsync(request, ct);
            TempData["Mensaje"] = "Proveedor registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        try
        {
            var proveedor = await _servicio.ObtenerAsync(id, ct);
            return View(proveedor);
        }
        catch (RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditarProveedorRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            await _servicio.EditarAsync(id, request, ct);
            TempData["Mensaje"] = "Proveedor actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
        catch (RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            var proveedor = await _servicio.ObtenerAsync(id, ct);
            return View(proveedor);
        }
        catch (RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        TempData["Mensaje"] = "Proveedor eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
