using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Domain;

public class OfertaTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    private static Licitacion LicitacionPublicada(decimal presupuesto, DateTimeOffset fechaCierre)
    {
        var licitacion = Licitacion.Crear("LIC-100", "Título", fechaCierre, presupuesto, Ahora);
        licitacion.Publicar(Ahora.AddMinutes(1));
        return licitacion;
    }

    [Fact]
    public void Oferta_igual_al_presupuesto_es_valida()
    {
        var licitacion = LicitacionPublicada(1_000_000m, Ahora.AddDays(1));

        var oferta = Oferta.Registrar(licitacion, Guid.NewGuid(), 1_000_000m, Ahora.AddHours(1));

        Assert.Equal(1_000_000m, oferta.MontoOfertadoCRC);
    }

    [Fact]
    public void Oferta_superior_al_presupuesto_es_rechazada()
    {
        var licitacion = LicitacionPublicada(1_000_000m, Ahora.AddDays(1));

        Assert.Throws<OfertaSuperaPresupuestoException>(() =>
            Oferta.Registrar(licitacion, Guid.NewGuid(), 1_000_001m, Ahora.AddHours(1)));
    }

    [Fact]
    public void Oferta_sobre_licitacion_vencida_es_rechazada()
    {
        var fechaCierre = Ahora.AddHours(1);
        var licitacion = LicitacionPublicada(1_000_000m, fechaCierre);

        Assert.Throws<LicitacionNoDisponibleParaOfertasException>(() =>
            Oferta.Registrar(licitacion, Guid.NewGuid(), 500_000m, fechaCierre.AddMinutes(1)));
    }

    [Fact]
    public void Oferta_sobre_licitacion_en_Borrador_es_rechazada()
    {
        var licitacion = Licitacion.Crear("LIC-101", "Título", Ahora.AddDays(1), 1_000_000m, Ahora);

        Assert.Throws<LicitacionNoDisponibleParaOfertasException>(() =>
            Oferta.Registrar(licitacion, Guid.NewGuid(), 500_000m, Ahora.AddMinutes(5)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Monto_no_positivo_lanza_excepcion(decimal monto)
    {
        var licitacion = LicitacionPublicada(1_000_000m, Ahora.AddDays(1));

        Assert.Throws<MontoInvalidoException>(() =>
            Oferta.Registrar(licitacion, Guid.NewGuid(), monto, Ahora.AddHours(1)));
    }
}
