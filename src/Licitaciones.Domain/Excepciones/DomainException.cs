namespace Licitaciones.Domain.Excepciones;

/// <summary>
/// Excepción base para violaciones de reglas de negocio del dominio.
/// La capa Application/API la traduce a respuestas HTTP controladas
/// (ProblemDetails), nunca se expone el stack trace al cliente.
/// </summary>
public class DomainException : Exception
{
    public string Codigo { get; }

    public DomainException(string codigo, string mensaje) : base(mensaje)
    {
        Codigo = codigo;
    }
}

public sealed class TransicionEstadoInvalidaException : DomainException
{
    public TransicionEstadoInvalidaException(string estadoActual, string estadoDestino)
        : base("licitacion.transicion_invalida",
               $"No se permite transicionar de '{estadoActual}' a '{estadoDestino}'.")
    {
    }
}

public sealed class LicitacionNoDisponibleParaOfertasException : DomainException
{
    public LicitacionNoDisponibleParaOfertasException(string motivo)
        : base("oferta.licitacion_no_disponible", motivo)
    {
    }
}

public sealed class OfertaSuperaPresupuestoException : DomainException
{
    public OfertaSuperaPresupuestoException()
        : base("oferta.supera_presupuesto",
               "La oferta no puede superar el presupuesto estimado de la licitación.")
    {
    }
}

public sealed class OfertaDuplicadaException : DomainException
{
    public OfertaDuplicadaException()
        : base("oferta.duplicada",
               "El proveedor ya registró una oferta para esta licitación.")
    {
    }
}

public sealed class MontoInvalidoException : DomainException
{
    public MontoInvalidoException(string campo)
        : base("valor.monto_invalido", $"El campo '{campo}' debe ser mayor que cero.")
    {
    }
}

public sealed class NombreProveedorInvalidoException : DomainException
{
    public NombreProveedorInvalidoException()
        : base("proveedor.nombre_invalido",
               "El nombre del proveedor contiene caracteres no permitidos o está vacío.")
    {
    }
}

public sealed class RangoAprobacionTraslapadoException : DomainException
{
    public RangoAprobacionTraslapadoException()
        : base("aprobacion.rango_traslapado",
               "El rango de aprobación se traslapa con uno existente.")
    {
    }
}
