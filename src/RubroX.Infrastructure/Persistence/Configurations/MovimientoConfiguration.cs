using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.ValueObjects;

namespace RubroX.Infrastructure.Persistence.Configurations;

public sealed class MovimientoConfiguration : IEntityTypeConfiguration<MovimientoPresupuestal>
{
    public void Configure(EntityTypeBuilder<MovimientoPresupuestal> builder)
    {
        builder.ToTable("movimientos");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(id => id.Value, v => MovimientoId.From(v))
            .HasColumnName("id");

        builder.Property(m => m.RubroId)
            .HasConversion(id => id.Value, v => RubroId.From(v))
            .HasColumnName("rubro_id");

        builder.Property(m => m.PadreId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                v => v.HasValue ? MovimientoId.From(v.Value) : null)
            .HasColumnName("movimiento_padre_id");

        builder.Property(m => m.Monto)
            .HasConversion(s => s.Valor, v => Saldo.Parse(v))
            .HasColumnName("monto")
            .HasPrecision(18, 2);

        builder.Property(m => m.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30);
        builder.Property(m => m.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Concepto).HasColumnName("concepto").IsRequired();
        builder.Property(m => m.Numeracion).HasColumnName("numeracion").HasMaxLength(50).IsRequired();
        builder.Property(m => m.UsuarioId).HasColumnName("usuario_id").HasMaxLength(100).IsRequired();
        builder.Property(m => m.FechaRegistro).HasColumnName("fecha_registro");
        builder.Property(m => m.FechaVencimiento).HasColumnName("fecha_vencimiento");
        builder.Property(m => m.Observacion).HasColumnName("observacion");
        builder.Property(m => m.CreadoEn).HasColumnName("created_at");
        builder.Property(m => m.ActualizadoEn).HasColumnName("updated_at");

        builder.HasIndex(m => m.RubroId).HasDatabaseName("idx_movimientos_rubro");
        builder.HasIndex(m => m.Tipo).HasDatabaseName("idx_movimientos_tipo");
        builder.HasIndex(m => m.Estado).HasDatabaseName("idx_movimientos_estado");
        builder.HasIndex(m => m.Numeracion).IsUnique().HasDatabaseName("uq_movimiento_numeracion");

        builder.Ignore(m => m.DomainEvents);
    }
}
