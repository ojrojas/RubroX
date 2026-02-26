namespace RubroX.Application.DTOs;

public sealed record RubroDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string Descripcion,
    int AnoFiscal,
    string Tipo,
    string Fuente,
    decimal SaldoInicial,
    decimal SaldoComprometido,
    decimal SaldoEjecutado,
    decimal SaldoDisponible,
    decimal PorcentajeEjecucion,
    string Estado,
    Guid? PadreId,
    DateTimeOffset CreadoEn,
    DateTimeOffset ActualizadoEn,
    string? CreadoPor,
    List<RubroDto>? Hijos = null);

public sealed record RubroResumenDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string Estado,
    decimal SaldoDisponible,
    decimal PorcentajeEjecucion,
    int TotalHijos);
