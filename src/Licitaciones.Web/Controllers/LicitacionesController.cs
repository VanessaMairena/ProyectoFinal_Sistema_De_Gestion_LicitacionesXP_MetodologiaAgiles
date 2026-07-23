using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Ofertas;
using Licitaciones.Domain.Enumeraciones;
using Licitaciones.Domain.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public class LicitacionesController : Controller
{
    private readonly LicitacionAppService _servicio;
    private readonly OfertaAppService _ofertas;

    public LicitacionesController(LicitacionAppService servicio, OfertaAppService ofertas)
    {
        _servicio = servicio;
        _ofertas = ofertas;
    }

    public async Task<IActionResult> Index(EstadoLicitacion? estado, int pagina = 1, CancellationToken ct = default)
    {
        var resultado = await _servicio.ListarAsync(estado, pagina, 20, ct);
        ViewBag.Estado = estado;
        return View(resultado);
    }

    public IActionResult Create() =>
    View(new CrearLicitacionRequest(string.Empty, string.Empty, TruncarAMinuto(DateTimeOffset.Now.AddDays(7)), 0));

private static DateTimeOffset TruncarAMinuto(DateTimeOffset fecha) =>
    new(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, 0, fecha.Offset);
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearLicitacionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            await _servicio.CrearAsync(request, ct);
            TempData["Mensaje"] = "Licitación creada correctamente en estado Borrador.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        try
        {
            var licitacion = await _servicio.ObtenerAsync(id, ct);
            ViewBag.Ofertas = await _ofertas.ListarPorLicitacionAsync(id, ct);
            ViewBag.MejorOferta = await _ofertas.ObtenerMejorOfertaAsync(id, ct);
            return View(licitacion);
        }
        catch (RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        try
        {
            return View(await _servicio.ObtenerAsync(id, ct));
        }
        catch (RecursoNoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditarLicitacionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            await _servicio.EditarAsync(id, request, ct);
            TempData["Mensaje"] = "Licitación actualizada correctamente.";
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(Guid id, EstadoLicitacion nuevoEstado, CancellationToken ct)
    {
        try
        {
            await _servicio.CambiarEstadoAsync(id, new CambiarEstadoLicitacionRequest(nuevoEstado), ct);
            TempData["Mensaje"] = "Estado actualizado correctamente.";
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            return View(await _servicio.ObtenerAsync(id, ct));
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
        TempData["Mensaje"] = "Licitación eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
