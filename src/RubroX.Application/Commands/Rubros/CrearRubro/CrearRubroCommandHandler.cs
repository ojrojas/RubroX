using RubroX.Application.Abstractions;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Common;
using RubroX.Domain.Events;
using RubroX.Domain.Repositories;
using RubroX.Domain.ValueObjects;

namespace RubroX.Application.Commands.Rubros.CrearRubro;

public sealed class CrearRubroCommandHandler(
    IRubroRepository rubroRepo,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher) : ICommandHandler<CrearRubroCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CrearRubroCommand command, CancellationToken ct = default)
    {
        // 1. Validar y crear Value Objects
        var codigoResult = CodigoPresupuestal.Create(command.Codigo);
        if (codigoResult.IsFailure) return Result.Failure<Guid>(codigoResult.Error!);

        var anoFiscalResult = AnoFiscal.Create(command.AnoFiscal);
        if (anoFiscalResult.IsFailure) return Result.Failure<Guid>(anoFiscalResult.Error!);

        var fuenteResult = FuenteFinanciamiento.Create(command.Fuente);
        if (fuenteResult.IsFailure) return Result.Failure<Guid>(fuenteResult.Error!);

        if (!Enum.TryParse<Domain.Enums.TipoRubro>(command.Tipo, out var tipo))
            return Result.Failure<Guid>($"Tipo de rubro inválido: '{command.Tipo}'.");

        // 2. Verificar unicidad del código en la vigencia
        var existe = await rubroRepo.ExisteCodigoAsync(codigoResult.Value, command.AnoFiscal, ct);
        if (existe)
            return Result.Failure<Guid>($"Ya existe un rubro con código '{command.Codigo}' en la vigencia {command.AnoFiscal}.");

        // 3. Validar padre si se especifica
        RubroId? padreId = null;
        if (command.PadreId.HasValue)
        {
            padreId = RubroId.From(command.PadreId.Value);
            var padre = await rubroRepo.ObtenerPorIdAsync(padreId.Value, ct);
            if (padre is null)
                return Result.Failure<Guid>($"No se encontró el rubro padre con ID '{command.PadreId}'.");
        }

        // 4. Crear el agregado
        var result = Rubro.Crear(
            codigoResult.Value,
            command.Nombre,
            command.Descripcion,
            anoFiscalResult.Value,
            tipo,
            fuenteResult.Value,
            padreId,
            command.UsuarioId);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error!);

        var rubro = result.Value;

        // 5. Persistir
        rubroRepo.Agregar(rubro);
        await unitOfWork.CommitAsync(ct);

        // 6. Despachar eventos de dominio
        await eventDispatcher.DispatchAsync(rubro.DomainEvents, ct);
        rubro.ClearDomainEvents();

        return Result.Success(rubro.Id.Value);
    }
}
