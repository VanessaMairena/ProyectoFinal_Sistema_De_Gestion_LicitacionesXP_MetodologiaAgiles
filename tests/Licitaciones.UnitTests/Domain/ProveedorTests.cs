using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Excepciones;
using Xunit;

namespace Licitaciones.UnitTests.Domain;

public class ProveedorTests
{
    private static readonly DateTimeOffset Ahora = DateTimeOffset.Parse("2026-07-18T10:00:00-06:00");

    [Theory]
    [InlineData("Empresa Central")]
    [InlineData("empresa central")]
    [InlineData("EMPRESA CENTRAL")]
    [InlineData("  Empresa   Central  ")]
    public void Nombres_equivalentes_producen_la_misma_forma_normalizada(string nombre)
    {
        var proveedor = Proveedor.Registrar(nombre, Ahora);

        Assert.Equal("empresa central", proveedor.NombreNormalizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Proveedor#Inválido")]
    [InlineData("Proveedor@2026")]
    public void Nombre_con_caracteres_no_permitidos_o_vacio_lanza_excepcion(string nombreInvalido)
    {
        Assert.Throws<NombreProveedorInvalidoException>(() => Proveedor.Registrar(nombreInvalido, Ahora));
    }

    [Fact]
    public void Nombre_con_parentesis_punto_y_coma_es_valido()
    {
        var proveedor = Proveedor.Registrar("Distribuidora Sur, S.A. (Sucursal 2)", Ahora);

        Assert.Equal("Distribuidora Sur, S.A. (Sucursal 2)", proveedor.Nombre);
    }

    [Fact]
    public void MarcarComoEliminado_establece_bandera_de_borrado_logico()
    {
        var proveedor = Proveedor.Registrar("Proveedor Uno", Ahora);

        proveedor.MarcarComoEliminado(Ahora.AddMinutes(5));

        Assert.True(proveedor.Eliminado);
    }
}
