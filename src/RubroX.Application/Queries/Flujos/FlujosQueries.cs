using RubroX.Application.Abstractions;
using RubroX.Application.DTOs;
using RubroX.Domain.Common;
using RubroX.Domain.Repositories;

namespace RubroX.Application.Queries.Flujos;

public sealed record ListarFlujosPendientesPorRolQuery(string Rol) : IQuery<IReadOnlyList<FlujoDto>>;

public sealed class ListarFlujosPendientesPorRolQueryHandler(IFlujoAprobacionRepository flujoRepo)
    : IQueryHandler<ListarFlujosPendientesPorRolQuery, IReadOnlyList<FlujoDto>>
{
    public async Task<Result<IReadOnlyList<FlujoDto>>> HandleAsync(
        ListarFlujosPendientesPorRolQuery query, CancellationToken ct = default)
    {
        var flujos = await flujoRepo.ListarActivosPorRolAsync(query.Rol, ct);

        var dtos = flujos.Select(f => new FlujoDto(
            f.Id.Value,
            f.TipoFlujo.ToString(),
            f.Estado.ToString(),
            f.PasoActual,
            f.Pasos.Count,
            f.IniciadorId,
            f.RubroId?.Value,
            null, // RubroCodigo se resuelve en join/include
            f.MovimientoId?.Value,
            null, // MovimientoNumeracion idem
            f.FechaInicio,
            f.FechaFin,
            f.Pasos.Select(p => new PasoAprobacionDto(
                p.Id.Value, p.Orden, p.RolRequerido, p.Estado.ToString(),
                p.AprobadorId, p.Comentario, p.FechaAccion)).ToList()
        )).ToList();

        return Result.Success<IReadOnlyList<FlujoDto>>(dtos);
    }
}
