namespace Licitaciones.Domain.Enumeraciones;

/// <summary>
/// Estados posibles de una licitación dentro de su ciclo de vida.
/// Transiciones permitidas: Borrador -> Publicada, Borrador -> Cerrada,
/// Publicada -> Cerrada. Cualquier otra transición requiere una regla de
/// reapertura explícita y no forma parte del flujo estándar.
/// </summary>
public enum EstadoLicitacion
{
    Borrador = 0,
    Publicada = 1,
    Cerrada = 2
}
