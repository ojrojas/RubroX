using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RubroX.Domain.Aggregates.Flujos;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;

namespace RubroX.Infrastructure.Persistence.Configurations;

public sealed class FlujoAprobacionConfiguration : IEntityTypeConfiguration<FlujoAprobacion>
{
    public void Configure(EntityTypeBuilder<FlujoAprobacion> builder)
    {
        builder.ToTable("flujos_aprobacion");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasConversion(id => id.Value, v => FlujoId.From(v))
            .HasColumnName("id");

        builder.Property(f => f.RubroId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                v => v.HasValue ? RubroId.From(v.Value) : null)
            .HasColumnName("rubro_id");

        builder.Property(f => f.MovimientoId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                v => v.HasValue ? MovimientoId.From(v.Value) : null)
            .HasColumnName("movimiento_id");

        builder.Property(f => f.TipoFlujo).HasColumnName("tipo_flujo").HasConversion<string>().HasMaxLength(50);
        builder.Property(f => f.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
        builder.Property(f => f.PasoActual).HasColumnName("paso_actual");
        builder.Property(f => f.IniciadorId).HasColumnName("iniciador_id").HasMaxLength(100);
        builder.Property(f => f.Payload).HasColumnName("payload").HasColumnType("jsonb");
        builder.Property(f => f.FechaInicio).HasColumnName("fecha_inicio");
        builder.Property(f => f.FechaFin).HasColumnName("fecha_fin");
        builder.Property(f => f.CreadoEn).HasColumnName("created_at");

        // Relación con pasos (owned collection via navigation)
        builder.HasMany(f => f.Pasos)
            .WithOne()
            .HasForeignKey(p => p.FlujoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => f.Estado).HasDatabaseName("idx_flujos_estado");
        builder.HasIndex(f => f.RubroId).HasDatabaseName("idx_flujos_rubro");

        builder.Ignore(f => f.DomainEvents);
        builder.Ignore(f => f.PasoActualEntidad);
    }
}

public sealed class PasoAprobacionConfiguration : IEntityTypeConfiguration<PasoAprobacion>
{
    public void Configure(EntityTypeBuilder<PasoAprobacion> builder)
    {
        builder.ToTable("pasos_aprobacion");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, v => PasoId.From(v))
            .HasColumnName("id");

        builder.Property(p => p.FlujoId)
            .HasConversion(id => id.Value, v => FlujoId.From(v))
            .HasColumnName("flujo_id");

        builder.Property(p => p.Orden).HasColumnName("orden");
        builder.Property(p => p.RolRequerido).HasColumnName("rol_requerido").HasMaxLength(100);
        builder.Property(p => p.AprobadorId).HasColumnName("aprobador_id").HasMaxLength(100);
        builder.Property(p => p.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Comentario).HasColumnName("comentario");
        builder.Property(p => p.FechaAccion).HasColumnName("fecha_accion");
        builder.Property(p => p.CreadoEn).HasColumnName("created_at");

        builder.HasIndex(p => new { p.FlujoId, p.Orden })
            .IsUnique()
            .HasDatabaseName("uq_flujo_orden");

        builder.Ignore(p => p.DomainEvents);
    }
}
