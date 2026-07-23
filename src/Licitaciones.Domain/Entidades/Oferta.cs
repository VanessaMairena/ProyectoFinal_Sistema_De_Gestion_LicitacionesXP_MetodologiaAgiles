using Licitaciones.Domain.Excepciones;

namespace Licitaciones.Domain.Entidades;

public class Oferta
{
    public Guid Id { get; private set; }
    public Guid LicitacionId { get; private set; }
    public Guid ProveedorId { get; private set; }
    public decimal MontoOfertadoCRC { get; private set; }
    public DateTimeOffset FechaRegistro { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public uint Version { get; private set; }

    private Oferta() { } // EF Core

    private Oferta(Guid licitacionId, Guid proveedorId, decimal montoOfertadoCRC, DateTimeOffset ahora)
    {
        Id = Guid.NewGuid();
        LicitacionId = licitacionId;
        ProveedorId = proveedorId;
        MontoOfertadoCRC = montoOfertadoCRC;
        FechaRegistro = ahora;
        UpdatedAt = ahora;
    }

    /// <summary>
    /// Registra una oferta validando las reglas de negocio del enunciado
    /// (secciones 8.2 y 8.5). La verificación de duplicidad por proveedor
    /// se delega al índice único de base de datos y a la capa Application,
    /// que consulta antes de invocar este método.
    /// </summary>
    public static Oferta Registrar(
        Licitacion licitacion,
        Guid proveedorId,
        decimal montoOfertadoCRC,
        DateTimeOffset ahora)
    {
        if (licitacion.Estado != Enumeraciones.EstadoLicitacion.Publicada)
            throw new LicitacionNoDisponibleParaOfertasException(
                "Solo se aceptan ofertas para licitaciones en estado Publicada.");

        if (ahora >= licitacion.FechaCierre)
            throw new LicitacionNoDisponibleParaOfertasException(
                "La licitación ya alcanzó su fecha y hora de cierre.");

        if (montoOfertadoCRC <= 0)
            throw new MontoInvalidoException(nameof(MontoOfertadoCRC));

        if (montoOfertadoCRC > licitacion.PresupuestoEstimadoCRC)
            throw new OfertaSuperaPresupuestoException();

        return new Oferta(licitacion.Id, proveedorId, montoOfertadoCRC, ahora);
    }
}
