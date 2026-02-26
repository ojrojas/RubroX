using RubroX.Application.Abstractions;

namespace RubroX.Application.Commands.Rubros.AsignarPresupuesto;

public sealed record AsignarPresupuestoCommand(
    Guid RubroId,
    decimal Monto,
    string UsuarioId) : ICommand;
