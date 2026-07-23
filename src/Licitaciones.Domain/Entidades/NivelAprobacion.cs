using Licitaciones.Domain.Excepciones;

namespace Licitaciones.Domain.Entidades;

public class NivelAprobacion
{
    public Guid Id { get; private set; }
    public decimal MontoMinimoCRC { get; private set; }
    public decimal? MontoMaximoCRC { get; private set; } // null = rango abierto
    public string Aprobador { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private NivelAprobacion() { } // EF Core

    private NivelAprobacion(decimal montoMinimoCRC, decimal? montoMaximoCRC, string aprobador, DateTimeOffset ahora)
    {
        Id = Guid.NewGuid();
        MontoMinimoCRC = montoMinimoCRC;
        MontoMaximoCRC = montoMaximoCRC;
        Aprobador = aprobador;
        CreatedAt = ahora;
        UpdatedAt = ahora;
    }

    public static NivelAprobacion Crear(
        decimal montoMinimoCRC,
        decimal? montoMaximoCRC,
        string aprobador,
        IEnumerable<NivelAprobacion> nivelesExistentes,
        DateTimeOffset ahora)
    {
        if (montoMinimoCRC <= 0)
            throw new MontoInvalidoException(nameof(MontoMinimoCRC));

        if (montoMaximoCRC.HasValue && montoMaximoCRC.Value <= montoMinimoCRC)
            throw new DomainException(
                "aprobacion.rango_invalido",
                "El monto máximo debe ser mayor que el monto mínimo.");

        if (montoMaximoCRC is null && nivelesExistentes.Any(n => n.MontoMaximoCRC is null))
            throw new DomainException(
                "aprobacion.rango_abierto_duplicado",
                "Ya existe un rango abierto (sin monto máximo).");

        var nuevo = new NivelAprobacion(montoMinimoCRC, montoMaximoCRC, aprobador, ahora);

        if (nivelesExistentes.Any(n => n.SeTraslapaCon(nuevo)))
            throw new RangoAprobacionTraslapadoException();

        return nuevo;
    }

    public void Editar(
        decimal montoMinimoCRC,
        decimal? montoMaximoCRC,
        string aprobador,
        IEnumerable<NivelAprobacion> otrosNiveles,
        DateTimeOffset ahora)
    {
        if (montoMinimoCRC <= 0)
            throw new MontoInvalidoException(nameof(MontoMinimoCRC));

        if (montoMaximoCRC.HasValue && montoMaximoCRC.Value <= montoMinimoCRC)
            throw new DomainException(
                "aprobacion.rango_invalido",
                "El monto máximo debe ser mayor que el monto mínimo.");

        if (montoMaximoCRC is null && otrosNiveles.Any(n => n.MontoMaximoCRC is null))
            throw new DomainException(
                "aprobacion.rango_abierto_duplicado",
                "Ya existe un rango abierto (sin monto máximo).");

        var propuesto = new NivelAprobacion(montoMinimoCRC, montoMaximoCRC, aprobador, ahora);

        if (otrosNiveles.Any(n => n.SeTraslapaCon(propuesto)))
            throw new RangoAprobacionTraslapadoException();

        MontoMinimoCRC = montoMinimoCRC;
        MontoMaximoCRC = montoMaximoCRC;
        Aprobador = aprobador;
        UpdatedAt = ahora;
    }

    public bool SeTraslapaCon(NivelAprobacion otro)
    {
        var esteMax = MontoMaximoCRC ?? decimal.MaxValue;
        var otroMax = otro.MontoMaximoCRC ?? decimal.MaxValue;

        return MontoMinimoCRC <= otroMax && otro.MontoMinimoCRC <= esteMax;
    }

    public bool Cubre(decimal monto) =>
        monto >= MontoMinimoCRC && (MontoMaximoCRC is null || monto <= MontoMaximoCRC.Value);
}
