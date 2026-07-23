namespace Licitaciones.Application.DTOs;

public sealed record ProveedorDto(Guid Id, string Nombre, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record RegistrarProveedorRequest(string Nombre);

public sealed record EditarProveedorRequest(string Nombre);
