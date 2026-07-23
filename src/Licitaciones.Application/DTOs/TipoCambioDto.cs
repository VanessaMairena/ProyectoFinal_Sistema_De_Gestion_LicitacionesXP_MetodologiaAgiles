namespace Licitaciones.Application.DTOs;

public sealed record TipoCambioDto(Guid Id, decimal CRCporUSD, DateTimeOffset FechaVigencia, bool Activo);

public sealed record CrearTipoCambioRequest(decimal CRCporUSD, DateTimeOffset FechaVigencia);

public sealed record ConversionDto(decimal MontoCRC, decimal MontoUSD, decimal CRCporUSD, DateTimeOffset FechaVigencia);
