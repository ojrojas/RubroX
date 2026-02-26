namespace RubroX.Application.DTOs;

public sealed record FlujoDto(
    Guid Id,
    string TipoFlujo,
    string Estado,
    int PasoActual,
    int TotalPasos,
    string IniciadorId,
    Guid? RubroId,
    string? RubroCodigo,
    Guid? MovimientoId,
    string? MovimientoNumeracion,
    DateTimeOffset FechaInicio,
    DateTimeOffset? FechaFin,
    List<PasoAprobacionDto> Pasos);

public sealed record PasoAprobacionDto(
    Guid Id,
    int Orden,
    string RolRequerido,
    string Estado,
    string? AprobadorId,
    string? Comentario,
    DateTimeOffset? FechaAccion);
