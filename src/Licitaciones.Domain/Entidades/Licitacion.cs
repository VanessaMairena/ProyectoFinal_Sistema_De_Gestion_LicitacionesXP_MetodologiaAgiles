using Licitaciones.Domain.Enumeraciones;
using Licitaciones.Domain.Excepciones;
using Licitaciones.Domain.ValueObjects;

namespace Licitaciones.Domain.Entidades;

public class Licitacion
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = default!;
    public string CodigoNormalizado { get; private set; } = default!;
    public string Titulo { get; private set; } = default!;
    public EstadoLicitacion Estado { get; private set; }
    public DateTimeOffset FechaCierre { get; private set; }
    public decimal PresupuestoEstimadoCRC { get; private set; }
    public bool Eliminada { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public uint Version { get; private set; }

    private Licitacion() { } // EF Core

    private Licitacion(
        CodigoLicitacion codigo,
        string titulo,
        DateTimeOffset fechaCierre,
        decimal presupuestoEstimadoCRC,
        DateTimeOffset ahora)
    {
        Id = Guid.NewGuid();
        Codigo = codigo.Valor;
        CodigoNormalizado = codigo.ValorNormalizado;
        Titulo = titulo;
        Estado = EstadoLicitacion.Borrador;
        FechaCierre = fechaCierre;
        PresupuestoEstimadoCRC = presupuestoEstimadoCRC;
        Eliminada = false;
        CreatedAt = ahora;
        UpdatedAt = ahora;
    }

    public static Licitacion Crear(
        string codigoIngresado,
        string titulo,
        DateTimeOffset fechaCierre,
        decimal presupuestoEstimadoCRC,
        DateTimeOffset ahora)
    {
        if (presupuestoEstimadoCRC <= 0)
            throw new MontoInvalidoException(nameof(PresupuestoEstimadoCRC));

        if (fechaCierre <= ahora)
            throw new DomainException(
                "licitacion.fecha_cierre_invalida",
                "La fecha de cierre debe ser futura respecto al momento de creación.");

        var codigo = CodigoLicitacion.Crear(codigoIngresado);
        return new Licitacion(codigo, titulo, fechaCierre, presupuestoEstimadoCRC, ahora);
    }

    /// <summary>
    /// Una licitación se considera cerrada funcionalmente si su fecha de
    /// cierre ya pasó, aunque el campo Estado todavía indique Publicada
    /// (regla del enunciado, sección 8.1).
    /// </summary>
    public bool EstaCerradaFuncionalmente(DateTimeOffset ahora) =>
        Estado == EstadoLicitacion.Cerrada || (Estado == EstadoLicitacion.Publicada && ahora >= FechaCierre);

    public void Publicar(DateTimeOffset ahora)
    {
        AsegurarTransicionPermitida(EstadoLicitacion.Publicada);
        Estado = EstadoLicitacion.Publicada;
        UpdatedAt = ahora;
    }

    public void Cerrar(DateTimeOffset ahora)
    {
        AsegurarTransicionPermitida(EstadoLicitacion.Cerrada);
        Estado = EstadoLicitacion.Cerrada;
        UpdatedAt = ahora;
    }

    private void AsegurarTransicionPermitida(EstadoLicitacion destino)
    {
        var permitida = (Estado, destino) switch
        {
            (EstadoLicitacion.Borrador, EstadoLicitacion.Publicada) => true,
            (EstadoLicitacion.Borrador, EstadoLicitacion.Cerrada) => true,
            (EstadoLicitacion.Publicada, EstadoLicitacion.Cerrada) => true,
            _ => false
        };

        if (!permitida)
            throw new TransicionEstadoInvalidaException(Estado.ToString(), destino.ToString());
    }

    public void MarcarComoEliminada(DateTimeOffset ahora)
    {
        Eliminada = true;
        UpdatedAt = ahora;
    }

    public void ReducirPresupuesto(decimal nuevoPresupuesto, decimal? montoOfertaMasAlta)
    {
        if (nuevoPresupuesto <= 0)
            throw new MontoInvalidoException(nameof(PresupuestoEstimadoCRC));

        if (montoOfertaMasAlta.HasValue && nuevoPresupuesto < montoOfertaMasAlta.Value)
            throw new DomainException(
                "licitacion.presupuesto_menor_a_oferta",
                "No puede reducirse el presupuesto por debajo de una oferta existente.");

        PresupuestoEstimadoCRC = nuevoPresupuesto;
    }
}
