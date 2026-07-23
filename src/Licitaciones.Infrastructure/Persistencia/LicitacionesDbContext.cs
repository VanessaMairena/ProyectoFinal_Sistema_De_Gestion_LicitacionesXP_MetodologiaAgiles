using Licitaciones.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Licitaciones.Infrastructure.Persistencia;

public class LicitacionesDbContext : DbContext
{
    public LicitacionesDbContext(DbContextOptions<LicitacionesDbContext> options) : base(options)
    {
    }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Licitacion> Licitaciones => Set<Licitacion>();
    public DbSet<Oferta> Ofertas => Set<Oferta>();
    public DbSet<NivelAprobacion> NivelesAprobacion => Set<NivelAprobacion>();
    public DbSet<TipoCambio> TiposCambio => Set<TipoCambio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LicitacionesDbContext).Assembly);

        // PostgreSQL (vía Npgsql) exige que las columnas "timestamp with time
        // zone" se escriban con offset UTC. Convertimos automáticamente todas
        // las propiedades DateTimeOffset del modelo a UTC al guardar; al leer
        // se devuelven tal cual (ya en UTC), y la interfaz las convierte a
        // hora local con ToLocalTime() donde corresponde.
        var conversorNoNulo = new ValueConverter<DateTimeOffset, DateTimeOffset>(
            v => v.ToUniversalTime(),
            v => v);

        var conversorNulo = new ValueConverter<DateTimeOffset?, DateTimeOffset?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                    property.SetValueConverter(conversorNoNulo);
                else if (property.ClrType == typeof(DateTimeOffset?))
                    property.SetValueConverter(conversorNulo);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}