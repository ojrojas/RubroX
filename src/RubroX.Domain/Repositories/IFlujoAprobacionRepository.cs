using RubroX.Domain.Aggregates.Flujos;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;

namespace RubroX.Domain.Repositories;

public interface IFlujoAprobacionRepository
{
    Task<FlujoAprobacion?> ObtenerPorIdAsync(FlujoId id, CancellationToken ct = default);
    Task<IReadOnlyList<FlujoAprobacion>> ListarActivosPorRolAsync(string rol, CancellationToken ct = default);
    Task<IReadOnlyList<FlujoAprobacion>> ListarPorRubroAsync(RubroId rubroId, CancellationToken ct = default);
    Task<FlujoAprobacion?> ObtenerActivoPorMovimientoAsync(MovimientoId movimientoId, CancellationToken ct = default);
    Task<IReadOnlyList<FlujoAprobacion>> ListarPorEstadoAsync(EstadoFlujo estado, CancellationToken ct = default);
    void Agregar(FlujoAprobacion flujo);
    void Actualizar(FlujoAprobacion flujo);
}
