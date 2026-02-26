using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RubroX.Domain.Aggregates.Flujos;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;

namespace RubroX.Infrastructure.Persistence;

/// <summary>DbContext principal de RubroX — esquema 'presupuesto'.</summary>
public sealed class RubroXDbContext(DbContextOptions<RubroXDbContext> options) : DbContext(options)
{
    public DbSet<Rubro> Rubros => Set<Rubro>();
    public DbSet<MovimientoPresupuestal> Movimientos => Set<MovimientoPresupuestal>();
    public DbSet<FlujoAprobacion> Flujos => Set<FlujoAprobacion>();
    public DbSet<PasoAprobacion> Pasos => Set<PasoAprobacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("presupuesto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RubroXDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Guardar todos los DateTimeOffset como UTC en PostgreSQL
        configurationBuilder.Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }
}

/// <summary>Convierte DateTimeOffset a UTC para almacenamiento PostgreSQL.</summary>
public class DateTimeOffsetConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(v => v.ToUniversalTime(), v => v.ToUniversalTime()) { }
}
