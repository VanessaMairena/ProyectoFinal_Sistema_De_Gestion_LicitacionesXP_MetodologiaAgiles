using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;
using Licitaciones.Domain.Servicios;
using Xunit;

namespace Licitaciones.UnitTests.Domain;

public class ServicioMejorOfertaTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    private static Licitacion LicitacionPublicada(decimal presupuesto) =>
        CrearYPublicar(presupuesto);

    private static Licitacion CrearYPublicar(decimal presupuesto)
    {
        var licitacion = Licitacion.Crear("LIC-200", "Título", Ahora.AddDays(1), presupuesto, Ahora);
        licitacion.Publicar(Ahora.AddMinutes(1));
        return licitacion;
    }

    [Fact]
    public void Sin_ofertas_clasifica_como_SinOfertasValidas()
    {
        var licitacion = LicitacionPublicada(1_000_000m);

        var resultado = ServicioMejorOferta.Calcular(licitacion, Array.Empty<Oferta>());

        Assert.Equal(ClasificacionOferta.SinOfertasValidas, resultado.Clasificacion);
        Assert.Null(resultado.MejorOferta);
    }

    [Fact]
    public void Ahorro_igual_o_mayor_a_10_por_ciento_es_OfertaConveniente()
    {
        var licitacion = LicitacionPublicada(1_000_000m);
        var oferta = Oferta.Registrar(licitacion, Guid.NewGuid(), 900_000m, Ahora.AddHours(1)); // 10% ahorro

        var resultado = ServicioMejorOferta.Calcular(licitacion, new[] { oferta });

        Assert.Equal(ClasificacionOferta.OfertaConveniente, resultado.Clasificacion);
        Assert.Equal(10m, resultado.PorcentajeAhorro);
    }

    [Fact]
    public void Ahorro_entre_0_y_10_por_ciento_es_OfertaAceptable()
    {
        var licitacion = LicitacionPublicada(1_000_000m);
        var oferta = Oferta.Registrar(licitacion, Guid.NewGuid(), 950_000m, Ahora.AddHours(1)); // 5% ahorro

        var resultado = ServicioMejorOferta.Calcular(licitacion, new[] { oferta });

        Assert.Equal(ClasificacionOferta.OfertaAceptable, resultado.Clasificacion);
    }

    [Fact]
    public void Oferta_igual_al_presupuesto_es_OfertaValidaSinAhorro()
    {
        var licitacion = LicitacionPublicada(1_000_000m);
        var oferta = Oferta.Registrar(licitacion, Guid.NewGuid(), 1_000_000m, Ahora.AddHours(1));

        var resultado = ServicioMejorOferta.Calcular(licitacion, new[] { oferta });

        Assert.Equal(ClasificacionOferta.OfertaValidaSinAhorro, resultado.Clasificacion);
        Assert.Equal(0m, resultado.PorcentajeAhorro);
    }

    [Fact]
    public void En_empate_gana_la_oferta_registrada_primero()
    {
        var licitacion = LicitacionPublicada(1_000_000m);
        var primera = Oferta.Registrar(licitacion, Guid.NewGuid(), 800_000m, Ahora.AddHours(1));
        var segunda = Oferta.Registrar(licitacion, Guid.NewGuid(), 800_000m, Ahora.AddHours(2));

        var resultado = ServicioMejorOferta.Calcular(licitacion, new[] { segunda, primera });

        Assert.Equal(primera.Id, resultado.MejorOferta!.Id);
    }
}
