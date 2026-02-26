namespace RubroX.Application.DTOs;

public sealed record MovimientoDto(
    Guid Id,
    Guid RubroId,
    string RubroCodigo,
    string Tipo,
    decimal Monto,
    string Concepto,
    string Numeracion,
    string Estado,
    string UsuarioId,
    Guid? PadreId,
    DateTimeOffset FechaRegistro,
    DateTimeOffset? FechaVencimiento,
    string? Observacion);
