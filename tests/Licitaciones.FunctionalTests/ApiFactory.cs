using Licitaciones.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Licitaciones.FunctionalTests;

/// <summary>
/// Arranca la Api completa en memoria (con su Program.cs real, sus
/// controladores reales y su base de datos PostgreSQL real), pero
/// apuntando a una base de datos exclusiva para pruebas
/// (<c>licitaciones_test</c>) que se recrea desde cero antes de correr
/// las pruebas, para no afectar los datos reales del sistema.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var rutaConfigPrueba = Path.Combine(AppContext.BaseDirectory, "appsettings.Test.json");
            configBuilder.AddJsonFile(rutaConfigPrueba, optional: false, reloadOnChange: false);
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LicitacionesDbContext>();

        // Base de datos limpia y fresca en cada corrida de pruebas E2E.
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Task.CompletedTask;
    }
}

[CollectionDefinition("Api E2E")]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
}
