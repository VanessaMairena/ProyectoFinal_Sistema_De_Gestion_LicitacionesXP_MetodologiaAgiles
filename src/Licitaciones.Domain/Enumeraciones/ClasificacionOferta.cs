namespace Licitaciones.Domain.Enumeraciones;

/// <summary>
/// Clasificación de la mejor oferta respecto al presupuesto estimado,
/// según la regla de negocio 8.6 del enunciado.
/// </summary>
public enum ClasificacionOferta
{
    SinOfertasValidas,
    OfertaConveniente,      // ahorro >= 10%
    OfertaAceptable,        // 0% < ahorro < 10%
    OfertaValidaSinAhorro   // ahorro == 0% (oferta == presupuesto)
}
