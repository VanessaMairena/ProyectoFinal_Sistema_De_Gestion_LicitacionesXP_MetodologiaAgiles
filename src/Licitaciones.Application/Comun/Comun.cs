namespace Licitaciones.Application.Comun;

/// <summary>
/// Se lanza cuando un caso de uso busca una entidad por Id y no la encuentra
/// (o está marcada como eliminada lógicamente). Se traduce a HTTP 404.
/// </summary>
public sealed class RecursoNoEncontradoException : Exception
{
    public RecursoNoEncontradoException(string entidad, Guid id)
        : base($"No se encontró {entidad} con Id '{id}'.")
    {
    }
}

/// <summary>
/// Parámetros de paginación validados con valores por defecto razonables.
/// </summary>
public sealed record ParametrosPaginacion
{
    public int Pagina { get; }
    public int TamanoPagina { get; }

    public ParametrosPaginacion(int pagina, int tamanoPagina)
    {
        Pagina = pagina < 1 ? 1 : pagina;
        TamanoPagina = tamanoPagina is < 1 or > 100 ? 20 : tamanoPagina;
    }
}

public sealed record ResultadoPaginado<T>(IReadOnlyList<T> Items, int Total, int Pagina, int TamanoPagina)
{
    public int TotalPaginas => (int)Math.Ceiling(Total / (double)TamanoPagina);
}
