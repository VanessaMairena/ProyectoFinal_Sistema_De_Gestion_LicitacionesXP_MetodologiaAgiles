using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;

namespace Licitaciones.Domain.Servicios;

public sealed record ResultadoMejorOferta(
    Oferta? MejorOferta,
    ClasificacionOferta Clasificacion,
    decimal PorcentajeAhorro);

/// <summary>
/// Determina la mejor oferta de una licitación y su clasificación,
/// según las reglas de negocio de la sección 8.6 del enunciado.
/// </summary>
public static class ServicioMejorOferta
{
    public static ResultadoMejorOferta Calcular(Licitacion licitacion, IReadOnlyCollection<Oferta> ofertas)
    {
        if (ofertas.Count == 0)
            return new ResultadoMejorOferta(null, ClasificacionOferta.SinOfertasValidas, 0m);

        // Menor monto; en empate, la registrada primero (orden estable por FechaRegistro).
        var mejor = ofertas
            .OrderBy(o => o.MontoOfertadoCRC)
            .ThenBy(o => o.FechaRegistro)
            .First();

        var porcentajeAhorro =
            (licitacion.PresupuestoEstimadoCRC - mejor.MontoOfertadoCRC) / licitacion.PresupuestoEstimadoCRC * 100m;

        var clasificacion = porcentajeAhorro switch
        {
            0m => ClasificacionOferta.OfertaValidaSinAhorro,
            >= 10m => ClasificacionOferta.OfertaConveniente,
            > 0m => ClasificacionOferta.OfertaAceptable,
            _ => ClasificacionOferta.OfertaValidaSinAhorro
        };

        return new ResultadoMejorOferta(mejor, clasificacion, porcentajeAhorro);
    }
}
