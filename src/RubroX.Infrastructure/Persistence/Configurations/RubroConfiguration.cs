using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;
using RubroX.Domain.ValueObjects;

namespace RubroX.Infrastructure.Persistence.Configurations;

public sealed class RubroConfiguration : IEntityTypeConfiguration<Rubro>
{
    public void Configure(EntityTypeBuilder<Rubro> builder)
    {
        builder.ToTable("rubros");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, v => RubroId.From(v))
            .HasColumnName("id");

        // Value Objects
        builder.Property(r => r.Codigo)
            .HasConversion(
                c => c.Valor,
                v => CodigoPresupuestal.Parse(v))
            .HasColumnName("codigo")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.AnoFiscal)
            .HasConversion(
                a => a.Valor,
                v => AnoFiscal.Parse(v))
            .HasColumnName("ano_fiscal")
            .IsRequired();

        builder.Property(r => r.Fuente)
            .HasConversion(
                f => f.Valor,
                v => FuenteFinanciamiento.Create(v).Value)
            .HasColumnName("fuente")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(r => r.SaldoInicial)
            .HasConversion(s => s.Valor, v => Saldo.Parse(v))
            .HasColumnName("saldo_inicial")
            .HasPrecision(18, 2);

        builder.Property(r => r.SaldoComprometido)
            .HasConversion(s => s.Valor, v => Saldo.Parse(v))
            .HasColumnName("saldo_comprometido")
            .HasPrecision(18, 2);

        builder.Property(r => r.SaldoEjecutado)
            .HasConversion(s => s.Valor, v => Saldo.Parse(v))
            .HasColumnName("saldo_ejecutado")
            .HasPrecision(18, 2);

        // Campos simples
        builder.Property(r => r.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        builder.Property(r => r.Descripcion).HasColumnName("descripcion");
        builder.Property(r => r.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.CreadoEn).HasColumnName("created_at");
        builder.Property(r => r.ActualizadoEn).HasColumnName("updated_at");
        builder.Property(r => r.CreadoPor).HasColumnName("created_by").HasMaxLength(100);

        // Padre (self-join)
        builder.Property(r => r.PadreId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                v => v.HasValue ? RubroId.From(v.Value) : null)
            .HasColumnName("padre_id");

        builder.HasOne<Rubro>()
            .WithMany()
            .HasForeignKey(r => r.PadreId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(r => new { r.AnoFiscal }).HasDatabaseName("idx_rubros_ano_fiscal");
        builder.HasIndex(r => r.Estado).HasDatabaseName("idx_rubros_estado");

        // Ignorar colecciones de dominio (los hijosIds se resuelven via query)
        builder.Ignore(r => r.HijosIds);
        builder.Ignore(r => r.DomainEvents);
        // SaldoDisponible y PorcentajeEjecucion son calculados, no mapeados
        builder.Ignore("SaldoDisponible");
        builder.Ignore("PorcentajeEjecucion");

        // Constraint único
        builder.HasIndex(r => new { r.Codigo, r.AnoFiscal })
            .IsUnique()
            .HasDatabaseName("uq_rubro_codigo_ano");
    }
}
