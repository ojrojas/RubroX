using RubroX.Application.Abstractions;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Common;
using RubroX.Domain.Events;
using RubroX.Domain.Repositories;
using RubroX.Domain.ValueObjects;

namespace RubroX.Application.Commands.Rubros.AsignarPresupuesto;

public sealed class AsignarPresupuestoCommandHandler(
    IRubroRepository rubroRepo,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher) : ICommandHandler<AsignarPresupuestoCommand>
{
    public async Task<Result> HandleAsync(AsignarPresupuestoCommand command, CancellationToken ct = default)
    {
        var rubro = await rubroRepo.ObtenerPorIdAsync(RubroId.From(command.RubroId), ct);
        if (rubro is null)
            return Result.Failure($"No se encontró el rubro con ID '{command.RubroId}'.");

        var saldoResult = Saldo.Create(command.Monto);
        if (saldoResult.IsFailure) return Result.Failure(saldoResult.Error!);

        var result = rubro.AsignarPresupuesto(saldoResult.Value, command.UsuarioId);
        if (result.IsFailure) return result;

        rubroRepo.Actualizar(rubro);
        await unitOfWork.CommitAsync(ct);
        await eventDispatcher.DispatchAsync(rubro.DomainEvents, ct);
        rubro.ClearDomainEvents();

        return Result.Success();
    }
}
