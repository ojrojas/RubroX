using Microsoft.EntityFrameworkCore;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;
using RubroX.Domain.Repositories;

namespace RubroX.Infrastructure.Persistence.Repositories;

public sealed class MovimientoRepository(RubroXDbContext dbContext) : IMovimientoRepository
{
    public async Task<MovimientoPresupuestal?> ObtenerPorIdAsync(MovimientoId id, CancellationToken ct = default) =>
        await dbContext.Movimientos.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<MovimientoPresupuestal?> ObtenerPorNumeracionAsync(string numeracion, CancellationToken ct = default) =>
        await dbContext.Movimientos.FirstOrDefaultAsync(m => m.Numeracion == numeracion, ct);

    public async Task<IReadOnlyList<MovimientoPresupuestal>> ListarPorRubroAsync(RubroId rubroId, CancellationToken ct = default) =>
        await dbContext.Movimientos
            .Where(m => m.RubroId == rubroId)
            .OrderByDescending(m => m.FechaRegistro)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<MovimientoPresupuestal>> ListarPorTipoAsync(
        TipoMovimiento tipo, int anoFiscal, CancellationToken ct = default) =>
        await dbContext.Movimientos
            .Where(m => m.Tipo == tipo)
            .OrderByDescending(m => m.FechaRegistro)
            .ToListAsync(ct);

    public async Task<bool> ExisteNumeracionAsync(string numeracion, CancellationToken ct = default) =>
        await dbContext.Movimientos.AnyAsync(m => m.Numeracion == numeracion, ct);

    public async Task<string> GenerarNumeracionAsync(TipoMovimiento tipo, int anoFiscal, CancellationToken ct = default)
    {
        var prefijo = tipo switch
        {
            TipoMovimiento.CDP => "CDP",
            TipoMovimiento.CRP => "CRP",
            TipoMovimiento.Pago => "PAG",
            TipoMovimiento.Reduccion => "RED",
            TipoMovimiento.Traslado => "TRX",
            _ => "MOV"
        };

        var ultimoNumero = await dbContext.Movimientos
            .Where(m => m.Tipo == tipo && m.Numeracion.StartsWith($"{prefijo}-{anoFiscal}-"))
            .CountAsync(ct);

        return $"{prefijo}-{anoFiscal}-{(ultimoNumero + 1):D4}";
    }

    public void Agregar(MovimientoPresupuestal movimiento) => dbContext.Movimientos.Add(movimiento);

    public void Actualizar(MovimientoPresupuestal movimiento) => dbContext.Movimientos.Update(movimiento);
}
