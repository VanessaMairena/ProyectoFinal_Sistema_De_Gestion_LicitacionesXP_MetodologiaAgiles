using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;

namespace Licitaciones.Domain.Servicios;

/// <summary>
/// Resuelve el aprobador correspondiente a un monto consultando la tabla
/// parametrizable de niveles de aprobación (regla 8.7 del enunciado).
/// Nunca debe implementarse mediante una cadena de if/else fija.
/// </summary>
public static class ServicioNivelAprobacion
{
    public static NivelAprobacion ResolverAprobador(decimal montoCRC, IEnumerable<NivelAprobacion> niveles)
    {
        var nivel = niveles.FirstOrDefault(n => n.Cubre(montoCRC));

        if (nivel is null)
            throw new DomainException(
                "aprobacion.sin_nivel_configurado",
                "No existe un nivel de aprobación configurado para el monto indicado.");

        return nivel;
    }
}
