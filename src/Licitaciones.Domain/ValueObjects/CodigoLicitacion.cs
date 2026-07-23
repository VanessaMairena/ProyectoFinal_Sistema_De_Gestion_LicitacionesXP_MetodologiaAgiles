using Licitaciones.Domain.Excepciones;

namespace Licitaciones.Domain.ValueObjects;

/// <summary>
/// Código único de una licitación. La unicidad se valida ignorando espacios
/// laterales y diferencias entre mayúsculas y minúsculas (regla 8.3).
/// </summary>
public sealed class CodigoLicitacion
{
    public string Valor { get; }
    public string ValorNormalizado { get; }

    private CodigoLicitacion(string valor, string valorNormalizado)
    {
        Valor = valor;
        ValorNormalizado = valorNormalizado;
    }

    public static CodigoLicitacion Crear(string codigoIngresado)
    {
        if (string.IsNullOrWhiteSpace(codigoIngresado))
            throw new DomainException("licitacion.codigo_invalido", "El código de licitación es obligatorio.");

        var recortado = codigoIngresado.Trim();
        return new CodigoLicitacion(recortado, recortado.ToLowerInvariant());
    }

    public override string ToString() => Valor;
}
