using RubroX.Application.Abstractions;

namespace RubroX.Application.Commands.Flujos.AprobarPaso;

public sealed record AprobarPasoCommand(
    Guid FlujoId,
    string AprobadorId,
    string RolAprobador,
    string? Comentario = null) : ICommand;
