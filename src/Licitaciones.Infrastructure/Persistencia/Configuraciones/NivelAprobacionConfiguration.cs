using Licitaciones.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistencia.Configuraciones;

public class NivelAprobacionConfiguration : IEntityTypeConfiguration<NivelAprobacion>
{
    public void Configure(EntityTypeBuilder<NivelAprobacion> builder)
    {
        builder.ToTable("niveles_aprobacion");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.MontoMinimoCRC).HasColumnType("numeric(18,2)");
        builder.Property(n => n.MontoMaximoCRC).HasColumnType("numeric(18,2)");

        builder.Property(n => n.Aprobador)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_niveles_aprobacion_monto_minimo_positivo", "\"MontoMinimoCRC\" > 0");

        // Refuerzo a nivel de base de datos: a lo sumo un rango abierto (sin monto máximo).
        builder.HasIndex(n => n.MontoMaximoCRC)
            .IsUnique()
            .HasFilter("\"MontoMaximoCRC\" IS NULL")
            .HasDatabaseName("ix_niveles_aprobacion_unico_rango_abierto");
    }
}
