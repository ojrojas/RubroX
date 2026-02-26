using Microsoft.Extensions.DependencyInjection;
using RubroX.Application.Abstractions;
using RubroX.Application.Commands.Flujos.AprobarPaso;
using RubroX.Application.Commands.Movimientos.RegistrarCDP;
using RubroX.Application.Commands.Rubros.AsignarPresupuesto;
using RubroX.Application.Commands.Rubros.CrearRubro;
using RubroX.Application.DTOs;
using RubroX.Application.Queries.Flujos;
using RubroX.Application.Queries.Rubros;

namespace RubroX.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Dispatchers
        services.AddScoped<CommandDispatcher>();
        services.AddScoped<QueryDispatcher>();

        // Command Handlers — Rubros
        services.AddScoped<ICommandHandler<CrearRubroCommand, Guid>, CrearRubroCommandHandler>();
        services.AddScoped<ICommandHandler<AsignarPresupuestoCommand>, AsignarPresupuestoCommandHandler>();

        // Command Handlers — Movimientos
        services.AddScoped<ICommandHandler<RegistrarCDPCommand, Guid>, RegistrarCDPCommandHandler>();

        // Command Handlers — Flujos
        services.AddScoped<ICommandHandler<AprobarPasoCommand>, AprobarPasoCommandHandler>();

        // Query Handlers — Rubros
        services.AddScoped<IQueryHandler<ObtenerRubroPorIdQuery, RubroDto>, ObtenerRubroPorIdQueryHandler>();
        services.AddScoped<IQueryHandler<ListarRubrosQuery, IReadOnlyList<RubroResumenDto>>, ListarRubrosQueryHandler>();

        // Query Handlers — Flujos
        services.AddScoped<IQueryHandler<ListarFlujosPendientesPorRolQuery, IReadOnlyList<FlujoDto>>, ListarFlujosPendientesPorRolQueryHandler>();

        return services;
    }
}
