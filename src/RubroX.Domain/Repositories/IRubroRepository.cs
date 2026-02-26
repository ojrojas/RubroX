using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.ValueObjects;

namespace RubroX.Domain.Repositories;

public interface IRubroRepository
{
    Task<Rubro?> ObtenerPorIdAsync(RubroId id, CancellationToken ct = default);
    Task<Rubro?> ObtenerPorCodigoAsync(CodigoPresupuestal codigo, int anoFiscal, CancellationToken ct = default);
    Task<IReadOnlyList<Rubro>> ListarPorVigenciaAsync(int anoFiscal, CancellationToken ct = default);
    Task<IReadOnlyList<Rubro>> ListarHijosAsync(RubroId padreId, CancellationToken ct = default);
    Task<bool> ExisteCodigoAsync(CodigoPresupuestal codigo, int anoFiscal, CancellationToken ct = default);
    void Agregar(Rubro rubro);
    void Actualizar(Rubro rubro);
}
