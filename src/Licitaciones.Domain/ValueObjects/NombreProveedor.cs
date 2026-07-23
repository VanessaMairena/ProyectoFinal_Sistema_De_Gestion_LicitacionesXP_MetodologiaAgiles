using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Licitaciones.Domain.Excepciones;

namespace Licitaciones.Domain.ValueObjects;

/// <summary>
/// Encapsula el nombre de un proveedor junto con su forma normalizada,
/// usada para validar unicidad ignorando espacios, mayúsculas/minúsculas
/// y variaciones Unicode (reglas 8.3 y 8.4 del enunciado).
/// </summary>
public sealed class NombreProveedor
{
    // Letras y números Unicode, espacio, punto, coma y paréntesis.
    private static readonly Regex CaracteresPermitidos =
        new(@"^[\p{L}\p{N} .,\(\)]+$", RegexOptions.Compiled);

    public string Valor { get; }
    public string ValorNormalizado { get; }

    private NombreProveedor(string valor, string valorNormalizado)
    {
        Valor = valor;
        ValorNormalizado = valorNormalizado;
    }

    public static NombreProveedor Crear(string nombreIngresado)
    {
        if (string.IsNullOrWhiteSpace(nombreIngresado))
            throw new NombreProveedorInvalidoException();

        var recortado = nombreIngresado.Trim();

        if (!CaracteresPermitidos.IsMatch(recortado))
            throw new NombreProveedorInvalidoException();

        return new NombreProveedor(recortado, Normalizar(recortado));
    }

    /// <summary>
    /// Trim + colapso de espacios repetidos + normalización Unicode (FormKC)
    /// + minúsculas invariantes, para comparar unicidad sin distinguir
    /// mayúsculas/minúsculas ni variantes de acentuación.
    /// </summary>
    private static string Normalizar(string valor)
    {
        var sinEspaciosRepetidos = Regex.Replace(valor.Trim(), @"\s+", " ");
        var formaNormalizada = sinEspaciosRepetidos.Normalize(NormalizationForm.FormKC);
        return formaNormalizada.ToLowerInvariant();
    }

    public override string ToString() => Valor;
}
