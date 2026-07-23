using Licitaciones.Application.DTOs;
using Licitaciones.Application.NivelesAprobacion;
using Licitaciones.Domain.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public class NivelesAprobacionController : Controller
{
    private readonly NivelAprobacionAppService _servicio;

    public NivelesAprobacionController(NivelAprobacionAppService servicio)
    {
        _servicio = servicio;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var niveles = await _servicio.ListarAsync(ct);
        return View(niveles);
    }

    public IActionResult Create() => View(new CrearNivelAprobacionRequest(0, null, string.Empty));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearNivelAprobacionRequest request, CancellationToken ct)
    {
        try
        {
            await _servicio.CrearAsync(request, ct);
            TempData["Mensaje"] = "Nivel de aprobación creado correctamente.";
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
            return View(await _servicio.ObtenerAsync(id, ct));
        }
        catch (Application.Comun.RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditarNivelAprobacionRequest request, CancellationToken ct)
    {
        try
        {
            await _servicio.EditarAsync(id, request, ct);
            TempData["Mensaje"] = "Nivel de aprobación actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
        catch (Application.Comun.RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _servicio.EliminarAsync(id, ct);
        TempData["Mensaje"] = "Nivel de aprobación eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Resolver(decimal montoCRC, CancellationToken ct)
    {
        try
        {
            var resultado = await _servicio.ResolverAprobadorAsync(new ResolverAprobadorRequest(montoCRC), ct);
            TempData["Mensaje"] = $"Para ₡{montoCRC:N2} el aprobador es: {resultado.Aprobador}";
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
