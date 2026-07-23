using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entidad = Licitaciones.Domain.Entidades.TipoCambio;

namespace Licitaciones.Infrastructure.Persistencia.Configuraciones;

public class TipoCambioConfiguration : IEntityTypeConfiguration<Entidad>
{
    public void Configure(EntityTypeBuilder<Entidad> builder)
    {
        builder.ToTable("tipos_cambio");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.CRCporUSD).HasColumnType("numeric(18,4)");

        builder.HasCheckConstraint("ck_tipos_cambio_valor_positivo", "\"CRCporUSD\" > 0");

        // Refuerzo a nivel de base de datos: a lo sumo un tipo de cambio activo.
        builder.HasIndex(t => t.Activo)
            .IsUnique()
            .HasFilter("\"Activo\" = true")
            .HasDatabaseName("ix_tipos_cambio_unico_activo");
    }
}
