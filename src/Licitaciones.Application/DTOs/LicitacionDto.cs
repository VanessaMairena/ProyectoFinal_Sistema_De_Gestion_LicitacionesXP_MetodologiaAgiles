using Licitaciones.Domain.Enumeraciones;

namespace Licitaciones.Application.DTOs;

public sealed record LicitacionDto(
    Guid Id,
    string Codigo,
    string Titulo,
    string Estado,
    DateTimeOffset FechaCierre,
    decimal PresupuestoEstimadoCRC);

public sealed record CrearLicitacionRequest(
    string Codigo,
    string Titulo,
    DateTimeOffset FechaCierre,
    decimal PresupuestoEstimadoCRC);

public sealed record EditarLicitacionRequest(
    string Titulo,
    DateTimeOffset FechaCierre,
    decimal PresupuestoEstimadoCRC);

public sealed record CambiarEstadoLicitacionRequest(EstadoLicitacion NuevoEstado);
