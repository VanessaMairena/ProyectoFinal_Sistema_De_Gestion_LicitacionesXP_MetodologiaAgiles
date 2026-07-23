using Licitaciones.Domain.Entidades;
using Licitaciones.Domain.Enumeraciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistencia.Configuraciones;

public class LicitacionConfiguration : IEntityTypeConfiguration<Licitacion>
{
    public void Configure(EntityTypeBuilder<Licitacion> builder)
    {
        builder.ToTable("licitaciones");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.CodigoNormalizado)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.Titulo)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(l => l.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.PresupuestoEstimadoCRC)
            .HasColumnType("numeric(18,2)");

        builder.HasIndex(l => l.CodigoNormalizado)
            .IsUnique()
            .HasDatabaseName("ix_licitaciones_codigo_normalizado");

        builder.HasIndex(l => l.Estado);
        builder.HasIndex(l => l.Eliminada);

        builder.Property(l => l.Version)
            .IsRowVersion();

        builder.HasCheckConstraint(
            "ck_licitaciones_presupuesto_positivo", "\"PresupuestoEstimadoCRC\" > 0");
    }
}
