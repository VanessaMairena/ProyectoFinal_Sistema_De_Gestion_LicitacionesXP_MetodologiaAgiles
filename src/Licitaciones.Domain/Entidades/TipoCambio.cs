using Licitaciones.Domain.Excepciones;

namespace Licitaciones.Domain.Entidades;

public class TipoCambio
{
    public Guid Id { get; private set; }
    public decimal CRCporUSD { get; private set; }
    public DateTimeOffset FechaVigencia { get; private set; }
    public bool Activo { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private TipoCambio() { } // EF Core

    private TipoCambio(decimal crcPorUsd, DateTimeOffset fechaVigencia, DateTimeOffset ahora)
    {
        Id = Guid.NewGuid();
        CRCporUSD = crcPorUsd;
        FechaVigencia = fechaVigencia;
        Activo = false;
        CreatedAt = ahora;
        UpdatedAt = ahora;
    }

    public static TipoCambio Crear(decimal crcPorUsd, DateTimeOffset fechaVigencia, DateTimeOffset ahora)
    {
        if (crcPorUsd <= 0)
            throw new MontoInvalidoException(nameof(CRCporUSD));

        return new TipoCambio(crcPorUsd, fechaVigencia, ahora);
    }

    public void Activar(DateTimeOffset ahora)
    {
        Activo = true;
        UpdatedAt = ahora;
    }

    public void Desactivar(DateTimeOffset ahora)
    {
        Activo = false;
        UpdatedAt = ahora;
    }

    public decimal ConvertirCrcAUsd(decimal montoCRC) => montoCRC / CRCporUSD;
}
