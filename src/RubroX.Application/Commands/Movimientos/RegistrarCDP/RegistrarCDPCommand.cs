using RubroX.Application.Abstractions;

namespace RubroX.Application.Commands.Movimientos.RegistrarCDP;

public sealed record RegistrarCDPCommand(
    Guid RubroId,
    decimal Monto,
    string Concepto,
    string UsuarioId,
    DateTimeOffset? FechaVencimiento = null) : ICommand<Guid>;
