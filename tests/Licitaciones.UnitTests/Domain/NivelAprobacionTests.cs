using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;
using Licitaciones.Domain.Servicios;
using Xunit;

namespace Licitaciones.UnitTests.Domain;

public class NivelAprobacionTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    [Fact]
    public void Rangos_traslapados_lanzan_excepcion()
    {
        var existente = NivelAprobacion.Crear(0.01m, 999_999.99m, "Encargado de área",
            Array.Empty<NivelAprobacion>(), Ahora);

        Assert.Throws<RangoAprobacionTraslapadoException>(() =>
            NivelAprobacion.Crear(500_000m, 1_500_000m, "Gerencia", new[] { existente }, Ahora));
    }

    [Fact]
    public void Solo_puede_existir_un_rango_abierto()
    {
        var existente = NivelAprobacion.Crear(10_000_000m, null, "Junta Directiva",
            Array.Empty<NivelAprobacion>(), Ahora);

        Assert.Throws<DomainException>(() =>
            NivelAprobacion.Crear(20_000_000m, null, "Otro aprobador", new[] { existente }, Ahora));
    }

    [Fact]
    public void ResolverAprobador_devuelve_el_nivel_que_cubre_el_monto()
    {
        var nivel1 = NivelAprobacion.Crear(0.01m, 999_999.99m, "Encargado de área",
            Array.Empty<NivelAprobacion>(), Ahora);
        var nivel2 = NivelAprobacion.Crear(1_000_000m, 9_999_999.99m, "Gerencia", new[] { nivel1 }, Ahora);
        var nivel3 = NivelAprobacion.Crear(10_000_000m, null, "Junta Directiva",
            new[] { nivel1, nivel2 }, Ahora);

        var niveles = new[] { nivel1, nivel2, nivel3 };

        var resultado = ServicioNivelAprobacion.ResolverAprobador(5_000_000m, niveles);

        Assert.Equal("Gerencia", resultado.Aprobador);
    }

    [Fact]
    public void ResolverAprobador_sin_nivel_configurado_lanza_excepcion()
    {
        Assert.Throws<DomainException>(() =>
            ServicioNivelAprobacion.ResolverAprobador(100m, Array.Empty<NivelAprobacion>()));
    }
}
