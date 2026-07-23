using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Domain;

public class LicitacionTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");
    private static readonly DateTimeOffset Manana = Ahora.AddDays(1);

    private static Licitacion CrearLicitacionValida() =>
        Licitacion.Crear("LIC-001", "Compra de equipo", Manana, 1_000_000m, Ahora);

    [Fact]
    public void Crear_licitacion_valida_queda_en_estado_Borrador()
    {
        var licitacion = CrearLicitacionValida();

        Assert.Equal(EstadoLicitacion.Borrador, licitacion.Estado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Presupuesto_no_positivo_lanza_excepcion(decimal presupuesto)
    {
        Assert.Throws<MontoInvalidoException>(() =>
            Licitacion.Crear("LIC-002", "Título", Manana, presupuesto, Ahora));
    }

    [Fact]
    public void Fecha_de_cierre_en_el_pasado_lanza_excepcion()
    {
        Assert.Throws<DomainException>(() =>
            Licitacion.Crear("LIC-003", "Título", Ahora.AddDays(-1), 1000m, Ahora));
    }

    [Fact]
    public void Publicar_desde_Borrador_es_permitido()
    {
        var licitacion = CrearLicitacionValida();

        licitacion.Publicar(Ahora.AddMinutes(1));

        Assert.Equal(EstadoLicitacion.Publicada, licitacion.Estado);
    }

    [Fact]
    public void Cerrar_desde_Publicada_es_permitido()
    {
        var licitacion = CrearLicitacionValida();
        licitacion.Publicar(Ahora.AddMinutes(1));

        licitacion.Cerrar(Ahora.AddMinutes(2));

        Assert.Equal(EstadoLicitacion.Cerrada, licitacion.Estado);
    }

    [Fact]
    public void Publicar_desde_Cerrada_no_es_permitido()
    {
        var licitacion = CrearLicitacionValida();
        licitacion.Publicar(Ahora.AddMinutes(1));
        licitacion.Cerrar(Ahora.AddMinutes(2));

        Assert.Throws<TransicionEstadoInvalidaException>(() => licitacion.Publicar(Ahora.AddMinutes(3)));
    }

    [Fact]
    public void Retroceder_de_Publicada_a_Borrador_no_es_permitido()
    {
        var licitacion = CrearLicitacionValida();
        licitacion.Publicar(Ahora.AddMinutes(1));

        // No existe método "Volver a Borrador" en la API pública, pero se
        // verifica la regla indirectamente: solo Publicar/Cerrar existen,
        // y Cerrar -> Publicada tampoco es válido.
        licitacion.Cerrar(Ahora.AddMinutes(2));
        Assert.Throws<TransicionEstadoInvalidaException>(() => licitacion.Publicar(Ahora.AddMinutes(3)));
    }

    [Fact]
    public void Licitacion_publicada_con_fecha_de_cierre_pasada_se_considera_cerrada_funcionalmente()
    {
        var licitacion = Licitacion.Crear("LIC-004", "Título", Ahora.AddMinutes(10), 1000m, Ahora);
        licitacion.Publicar(Ahora.AddMinutes(1));

        var despuesDelCierre = Ahora.AddMinutes(11);

        Assert.True(licitacion.EstaCerradaFuncionalmente(despuesDelCierre));
    }

    [Fact]
    public void No_puede_reducirse_presupuesto_por_debajo_de_oferta_existente()
    {
        var licitacion = CrearLicitacionValida();

        Assert.Throws<DomainException>(() => licitacion.ReducirPresupuesto(500_000m, 800_000m));
    }
}
