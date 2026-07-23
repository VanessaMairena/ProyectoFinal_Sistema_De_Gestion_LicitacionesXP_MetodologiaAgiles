using Licitaciones.Application.DTOs;
using Licitaciones.Application.TipoCambio;
using Licitaciones.Domain.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public class TipoCambioController : Controller
{
    private readonly TipoCambioAppService _servicio;

    public TipoCambioController(TipoCambioAppService servicio)
    {
        _servicio = servicio;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var lista = await _servicio.ListarAsync(ct);
        return View(lista);
    }

    public IActionResult Create() => View(new CrearTipoCambioRequest(0, DateTimeOffset.Now));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearTipoCambioRequest request, CancellationToken ct)
    {
        try
        {
            await _servicio.RegistrarYActivarAsync(request, ct);
            TempData["Mensaje"] = "Tipo de cambio registrado y activado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Convertir(decimal montoCRC, CancellationToken ct)
    {
        try
        {
            var resultado = await _servicio.ConvertirAsync(montoCRC, ct);
            TempData["Mensaje"] =
                $"₡{resultado.MontoCRC:N2} equivale a ${resultado.MontoUSD:N2} " +
                $"(tipo de cambio: {resultado.CRCporUSD:N4}, vigente desde {resultado.FechaVigencia:dd/MM/yyyy})";
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
