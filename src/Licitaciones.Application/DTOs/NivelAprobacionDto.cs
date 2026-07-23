namespace Licitaciones.Application.DTOs;

public sealed record NivelAprobacionDto(
    Guid Id, decimal MontoMinimoCRC, decimal? MontoMaximoCRC, string Aprobador);

public sealed record CrearNivelAprobacionRequest(
    decimal MontoMinimoCRC, decimal? MontoMaximoCRC, string Aprobador);

public sealed record EditarNivelAprobacionRequest(
    decimal MontoMinimoCRC, decimal? MontoMaximoCRC, string Aprobador);

public sealed record ResolverAprobadorRequest(decimal MontoCRC);

public sealed record AprobadorResueltoDto(string Aprobador, decimal MontoMinimoCRC, decimal? MontoMaximoCRC);
