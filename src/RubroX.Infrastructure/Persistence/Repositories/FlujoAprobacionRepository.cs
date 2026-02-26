using Microsoft.EntityFrameworkCore;
using RubroX.Domain.Aggregates.Flujos;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;
using RubroX.Domain.Repositories;

namespace RubroX.Infrastructure.Persistence.Repositories;

public sealed class FlujoAprobacionRepository(RubroXDbContext dbContext) : IFlujoAprobacionRepository
{
    public async Task<FlujoAprobacion?> ObtenerPorIdAsync(FlujoId id, CancellationToken ct = default) =>
        await dbContext.Flujos
            .Include(f => f.Pasos)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IReadOnlyList<FlujoAprobacion>> ListarActivosPorRolAsync(string rol, CancellationToken ct = default) =>
        await dbContext.Flujos
            .Include(f => f.Pasos)
            .Where(f => (f.Estado == EstadoFlujo.Pendiente || f.Estado == EstadoFlujo.EnRevision)
                && f.Pasos.Any(p => p.Orden == f.PasoActual && p.RolRequerido == rol && p.Estado == EstadoPaso.Pendiente))
            .OrderBy(f => f.FechaInicio)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<FlujoAprobacion>> ListarPorRubroAsync(RubroId rubroId, CancellationToken ct = default) =>
        await dbContext.Flujos
            .Include(f => f.Pasos)
            .Where(f => f.RubroId == rubroId)
            .OrderByDescending(f => f.FechaInicio)
            .ToListAsync(ct);

    public async Task<FlujoAprobacion?> ObtenerActivoPorMovimientoAsync(MovimientoId movimientoId, CancellationToken ct = default) =>
        await dbContext.Flujos
            .Include(f => f.Pasos)
            .FirstOrDefaultAsync(f => f.MovimientoId == movimientoId
                && (f.Estado == EstadoFlujo.Pendiente || f.Estado == EstadoFlujo.EnRevision), ct);

    public async Task<IReadOnlyList<FlujoAprobacion>> ListarPorEstadoAsync(EstadoFlujo estado, CancellationToken ct = default) =>
        await dbContext.Flujos
            .Include(f => f.Pasos)
            .Where(f => f.Estado == estado)
            .OrderByDescending(f => f.FechaInicio)
            .ToListAsync(ct);

    public void Agregar(FlujoAprobacion flujo) => dbContext.Flujos.Add(flujo);

    public void Actualizar(FlujoAprobacion flujo) => dbContext.Flujos.Update(flujo);
}
