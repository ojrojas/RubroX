using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RubroX.Application.Abstractions;
using RubroX.Domain.Events;
using RubroX.Domain.Repositories;
using RubroX.Infrastructure.Events;
using RubroX.Infrastructure.Persistence;
using RubroX.Infrastructure.Persistence.Repositories;

namespace RubroX.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core + PostgreSQL
        services.AddDbContext<RubroXDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("RubroXDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "presupuesto")));

        // UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositorios
        services.AddScoped<IRubroRepository, RubroRepository>();
        services.AddScoped<IMovimientoRepository, MovimientoRepository>();
        services.AddScoped<IFlujoAprobacionRepository, FlujoAprobacionRepository>();

        // Domain Events
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
