using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;

namespace RubroX.Domain.Repositories;

public interface IMovimientoRepository
{
    Task<MovimientoPresupuestal?> ObtenerPorIdAsync(MovimientoId id, CancellationToken ct = default);
    Task<MovimientoPresupuestal?> ObtenerPorNumeracionAsync(string numeracion, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoPresupuestal>> ListarPorRubroAsync(RubroId rubroId, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoPresupuestal>> ListarPorTipoAsync(TipoMovimiento tipo, int anoFiscal, CancellationToken ct = default);
    Task<bool> ExisteNumeracionAsync(string numeracion, CancellationToken ct = default);
    Task<string> GenerarNumeracionAsync(TipoMovimiento tipo, int anoFiscal, CancellationToken ct = default);
    void Agregar(MovimientoPresupuestal movimiento);
    void Actualizar(MovimientoPresupuestal movimiento);
}
