using RubroX.Application.Abstractions;
using RubroX.Application.DTOs;
using RubroX.Domain.Common;
using RubroX.Domain.Repositories;

namespace RubroX.Application.Queries.Rubros;

// ===== Queries =====

public sealed record ObtenerRubroPorIdQuery(Guid Id) : IQuery<RubroDto>;

public sealed record ListarRubrosQuery(int AnoFiscal, string? Estado = null) : IQuery<IReadOnlyList<RubroResumenDto>>;

public sealed record ObtenerJerarquiaRubrosQuery(int AnoFiscal) : IQuery<IReadOnlyList<RubroDto>>;

// ===== Handlers =====

public sealed class ObtenerRubroPorIdQueryHandler(IRubroRepository rubroRepo)
    : IQueryHandler<ObtenerRubroPorIdQuery, RubroDto>
{
    public async Task<Result<RubroDto>> HandleAsync(ObtenerRubroPorIdQuery query, CancellationToken ct = default)
    {
        var rubro = await rubroRepo.ObtenerPorIdAsync(
            Domain.Aggregates.Rubros.RubroId.From(query.Id), ct);

        if (rubro is null)
            return Result.Failure<RubroDto>($"No se encontró el rubro con ID '{query.Id}'.");

        var dto = new RubroDto(
            rubro.Id.Value, rubro.Codigo.Valor, rubro.Nombre, rubro.Descripcion,
            rubro.AnoFiscal.Valor, rubro.Tipo.ToString(), rubro.Fuente.Valor,
            rubro.SaldoInicial.Valor, rubro.SaldoComprometido.Valor, rubro.SaldoEjecutado.Valor,
            rubro.SaldoDisponible, rubro.PorcentajeEjecucion,
            rubro.Estado.ToString(), rubro.PadreId?.Value,
            rubro.CreadoEn, rubro.ActualizadoEn, rubro.CreadoPor);

        return Result.Success(dto);
    }
}

public sealed class ListarRubrosQueryHandler(IRubroRepository rubroRepo)
    : IQueryHandler<ListarRubrosQuery, IReadOnlyList<RubroResumenDto>>
{
    public async Task<Result<IReadOnlyList<RubroResumenDto>>> HandleAsync(
        ListarRubrosQuery query, CancellationToken ct = default)
    {
        var rubros = await rubroRepo.ListarPorVigenciaAsync(query.AnoFiscal, ct);

        var dtos = rubros
            .Where(r => query.Estado is null || r.Estado.ToString() == query.Estado)
            .Select(r => new RubroResumenDto(
                r.Id.Value, r.Codigo.Valor, r.Nombre, r.Estado.ToString(),
                r.SaldoDisponible, r.PorcentajeEjecucion, r.HijosIds.Count))
            .ToList();

        return Result.Success<IReadOnlyList<RubroResumenDto>>(dtos);
    }
}
