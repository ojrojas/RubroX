using RubroX.Application.Abstractions;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Common;
using RubroX.Domain.Enums;
using RubroX.Domain.Events;
using RubroX.Domain.Repositories;
using RubroX.Domain.ValueObjects;

namespace RubroX.Application.Commands.Movimientos.RegistrarCDP;

public sealed class RegistrarCDPCommandHandler(
    IRubroRepository rubroRepo,
    IMovimientoRepository movimientoRepo,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher) : ICommandHandler<RegistrarCDPCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(RegistrarCDPCommand command, CancellationToken ct = default)
    {
        // 1. Obtener el rubro
        var rubro = await rubroRepo.ObtenerPorIdAsync(RubroId.From(command.RubroId), ct);
        if (rubro is null)
            return Result.Failure<Guid>($"No se encontró el rubro con ID '{command.RubroId}'.");

        // 2. Crear value object del monto
        var montoResult = Saldo.Create(command.Monto);
        if (montoResult.IsFailure) return Result.Failure<Guid>(montoResult.Error!);

        // 3. Generar numeración automática
        var numeracion = await movimientoRepo.GenerarNumeracionAsync(TipoMovimiento.CDP, rubro.AnoFiscal.Valor, ct);

        // 4. Reservar saldo en el rubro (valida disponibilidad)
        var reservaResult = rubro.ReservarSaldo(montoResult.Value, numeracion);
        if (reservaResult.IsFailure) return Result.Failure<Guid>(reservaResult.Error!);

        // 5. Crear el movimiento
        var movimientoResult = MovimientoPresupuestal.Crear(
            RubroId.From(command.RubroId),
            TipoMovimiento.CDP,
            montoResult.Value,
            command.Concepto,
            numeracion,
            command.UsuarioId,
            fechaVencimiento: command.FechaVencimiento);

        if (movimientoResult.IsFailure) return Result.Failure<Guid>(movimientoResult.Error!);

        var movimiento = movimientoResult.Value;

        // 6. Persistir todo
        movimientoRepo.Agregar(movimiento);
        rubroRepo.Actualizar(rubro);
        await unitOfWork.CommitAsync(ct);

        // 7. Despachar eventos
        await eventDispatcher.DispatchAsync([.. rubro.DomainEvents, .. movimiento.DomainEvents], ct);
        rubro.ClearDomainEvents();
        movimiento.ClearDomainEvents();

        return Result.Success(movimiento.Id.Value);
    }
}
