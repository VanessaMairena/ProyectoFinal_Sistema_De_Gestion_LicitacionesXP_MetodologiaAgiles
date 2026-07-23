using Licitaciones.Application.Comun;
using Licitaciones.Application.DTOs;
using Licitaciones.Application.Interfaces;
using Licitaciones.Application.Ofertas;
using Licitaciones.Domain.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public class OfertasController : Controller
{
    private readonly OfertaAppService _servicio;
    private readonly IProveedorRepository _proveedores;

    public OfertasController(OfertaAppService servicio, IProveedorRepository proveedores)
    {
        _servicio = servicio;
        _proveedores = proveedores;
    }

    public async Task<IActionResult> Create(Guid licitacionId, CancellationToken ct)
    {
        var proveedores = await _proveedores.ListarActivosParaSeleccionAsync(ct);
        ViewBag.Proveedores = proveedores;
        ViewBag.LicitacionId = licitacionId;
        return View(new RegistrarOfertaRequest(Guid.Empty, 0));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid licitacionId, RegistrarOfertaRequest request, CancellationToken ct)
    {
        try
        {
            await _servicio.RegistrarAsync(licitacionId, request, ct);
            TempData["Mensaje"] = "Oferta registrada correctamente.";
            return RedirectToAction("Details", "Licitaciones", new { id = licitacionId });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Proveedores = await _proveedores.ListarActivosParaSeleccionAsync(ct);
            ViewBag.LicitacionId = licitacionId;
            return View(request);
        }
        catch (RecursoNoEncontradoException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Proveedores = await _proveedores.ListarActivosParaSeleccionAsync(ct);
            ViewBag.LicitacionId = licitacionId;
            return View(request);
        }
    }
}
