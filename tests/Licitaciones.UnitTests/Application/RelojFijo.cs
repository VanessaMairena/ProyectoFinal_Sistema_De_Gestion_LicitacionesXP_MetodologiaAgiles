namespace Licitaciones.UnitTests.Application;

public sealed class RelojFijo : TimeProvider
{
    private readonly DateTimeOffset _ahora;

    public RelojFijo(DateTimeOffset ahora) => _ahora = ahora;

    public override DateTimeOffset GetUtcNow() => _ahora;
}
