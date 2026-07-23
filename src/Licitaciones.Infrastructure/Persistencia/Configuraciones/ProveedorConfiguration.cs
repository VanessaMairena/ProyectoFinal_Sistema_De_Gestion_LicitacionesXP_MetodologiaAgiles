using Licitaciones.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistencia.Configuraciones;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("proveedores");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.NombreNormalizado)
            .HasMaxLength(300)
            .IsRequired();

        builder.HasIndex(p => p.NombreNormalizado)
            .IsUnique()
            .HasDatabaseName("ix_proveedores_nombre_normalizado");

        builder.Property(p => p.Version)
            .IsRowVersion();

        // Índice para acelerar el filtro de "no eliminados" en listados.
        builder.HasIndex(p => p.Eliminado);
    }
}
