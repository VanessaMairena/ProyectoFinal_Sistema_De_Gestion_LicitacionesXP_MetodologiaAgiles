using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Domain;

public class TipoCambioTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    [Fact]
    public void ConvertirCrcAUsd_divide_el_monto_entre_el_tipo_de_cambio()
    {
        var tipoCambio = TipoCambio.Crear(520m, Ahora, Ahora);

        var resultado = tipoCambio.ConvertirCrcAUsd(52_000m);

        Assert.Equal(100m, resultado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Tipo_de_cambio_no_positivo_lanza_excepcion(decimal valor)
    {
        Assert.Throws<MontoInvalidoException>(() => TipoCambio.Crear(valor, Ahora, Ahora));
    }

    [Fact]
    public void Activar_marca_el_tipo_de_cambio_como_activo()
    {
        var tipoCambio = TipoCambio.Crear(520m, Ahora, Ahora);

        tipoCambio.Activar(Ahora.AddMinutes(1));

        Assert.True(tipoCambio.Activo);
    }
}
