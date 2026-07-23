using Licitaciones.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistencia.Configuraciones;

public class OfertaConfiguration : IEntityTypeConfiguration<Oferta>
{
    public void Configure(EntityTypeBuilder<Oferta> builder)
    {
        builder.ToTable("ofertas");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.MontoOfertadoCRC)
            .HasColumnType("numeric(18,2)");

        // Un proveedor no puede presentar más de una oferta por licitación.
        builder.HasIndex(o => new { o.LicitacionId, o.ProveedorId })
            .IsUnique()
            .HasDatabaseName("ix_ofertas_licitacion_proveedor");

        builder.HasOne<Licitacion>()
            .WithMany()
            .HasForeignKey(o => o.LicitacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Proveedor>()
            .WithMany()
            .HasForeignKey(o => o.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(o => o.Version)
            .IsRowVersion();

        builder.HasCheckConstraint("ck_ofertas_monto_positivo", "\"MontoOfertadoCRC\" > 0");
    }
}
