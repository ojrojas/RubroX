using Microsoft.EntityFrameworkCore;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Repositories;
using RubroX.Domain.ValueObjects;

namespace RubroX.Infrastructure.Persistence.Repositories;

public sealed class RubroRepository(RubroXDbContext dbContext) : IRubroRepository
{
    public async Task<Rubro?> ObtenerPorIdAsync(RubroId id, CancellationToken ct = default) =>
        await dbContext.Rubros.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<Rubro?> ObtenerPorCodigoAsync(CodigoPresupuestal codigo, int anoFiscal, CancellationToken ct = default) =>
        await dbContext.Rubros.FirstOrDefaultAsync(r => r.Codigo == codigo && r.AnoFiscal == AnoFiscal.Parse(anoFiscal), ct);

    public async Task<IReadOnlyList<Rubro>> ListarPorVigenciaAsync(int anoFiscal, CancellationToken ct = default)
    {
        var ano = AnoFiscal.Parse(anoFiscal);
        return await dbContext.Rubros
            .Where(r => r.AnoFiscal == ano)
            .OrderBy(r => r.Codigo)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Rubro>> ListarHijosAsync(RubroId padreId, CancellationToken ct = default) =>
        await dbContext.Rubros
            .Where(r => r.PadreId == padreId)
            .OrderBy(r => r.Codigo)
            .ToListAsync(ct);

    public async Task<bool> ExisteCodigoAsync(CodigoPresupuestal codigo, int anoFiscal, CancellationToken ct = default)
    {
        var ano = AnoFiscal.Parse(anoFiscal);
        return await dbContext.Rubros.AnyAsync(r => r.Codigo == codigo && r.AnoFiscal == ano, ct);
    }

    public void Agregar(Rubro rubro) => dbContext.Rubros.Add(rubro);

    public void Actualizar(Rubro rubro) => dbContext.Rubros.Update(rubro);
}
