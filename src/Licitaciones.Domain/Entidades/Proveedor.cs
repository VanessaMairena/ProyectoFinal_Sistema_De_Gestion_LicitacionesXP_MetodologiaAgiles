using Licitaciones.Domain.ValueObjects;

namespace Licitaciones.Domain.Entidades;

public class Proveedor
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = default!;
    public string NombreNormalizado { get; private set; } = default!;
    public bool Eliminado { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public uint Version { get; private set; }

    private Proveedor() { } // EF Core

    private Proveedor(NombreProveedor nombre, DateTimeOffset ahora)
    {
        Id = Guid.NewGuid();
        Nombre = nombre.Valor;
        NombreNormalizado = nombre.ValorNormalizado;
        Eliminado = false;
        CreatedAt = ahora;
        UpdatedAt = ahora;
    }

    public static Proveedor Registrar(string nombreIngresado, DateTimeOffset ahora)
    {
        var nombre = NombreProveedor.Crear(nombreIngresado);
        return new Proveedor(nombre, ahora);
    }

    public void Editar(string nuevoNombre, DateTimeOffset ahora)
    {
        var nombre = NombreProveedor.Crear(nuevoNombre);
        Nombre = nombre.Valor;
        NombreNormalizado = nombre.ValorNormalizado;
        UpdatedAt = ahora;
    }

    public void MarcarComoEliminado(DateTimeOffset ahora)
    {
        Eliminado = true;
        UpdatedAt = ahora;
    }
}
