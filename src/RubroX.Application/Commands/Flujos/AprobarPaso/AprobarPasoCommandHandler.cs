using RubroX.Application.Abstractions;
using RubroX.Domain.Aggregates.Flujos;
using RubroX.Domain.Common;
using RubroX.Domain.Events;
using RubroX.Domain.Repositories;

namespace RubroX.Application.Commands.Flujos.AprobarPaso;

public sealed class AprobarPasoCommandHandler(
    IFlujoAprobacionRepository flujoRepo,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher) : ICommandHandler<AprobarPasoCommand>
{
    public async Task<Result> HandleAsync(AprobarPasoCommand command, CancellationToken ct = default)
    {
        var flujo = await flujoRepo.ObtenerPorIdAsync(FlujoId.From(command.FlujoId), ct);
        if (flujo is null)
            return Result.Failure($"No se encontró el flujo de aprobación con ID '{command.FlujoId}'.");

        var result = flujo.Aprobar(command.AprobadorId, command.RolAprobador, command.Comentario);
        if (result.IsFailure) return result;

        flujoRepo.Actualizar(flujo);
        await unitOfWork.CommitAsync(ct);
        await eventDispatcher.DispatchAsync(flujo.DomainEvents, ct);
        flujo.ClearDomainEvents();

        return Result.Success();
    }
}
