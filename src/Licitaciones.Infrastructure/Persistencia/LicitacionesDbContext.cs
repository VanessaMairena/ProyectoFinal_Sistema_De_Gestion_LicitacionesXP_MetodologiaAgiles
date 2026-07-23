using Licitaciones.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

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
        base.OnModelCreating(modelBuilder);
    }
}
