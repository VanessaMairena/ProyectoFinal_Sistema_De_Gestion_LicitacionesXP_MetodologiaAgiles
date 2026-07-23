namespace Licitaciones.Application.DTOs;

public sealed record OfertaDto(
    Guid Id,
    Guid LicitacionId,
    Guid ProveedorId,
    string NombreProveedor,
    decimal MontoOfertadoCRC,
    DateTimeOffset FechaRegistro);

public sealed record RegistrarOfertaRequest(Guid ProveedorId, decimal MontoOfertadoCRC);

public sealed record MejorOfertaDto(
    OfertaDto? MejorOferta,
    string Clasificacion,
    decimal PorcentajeAhorro);
