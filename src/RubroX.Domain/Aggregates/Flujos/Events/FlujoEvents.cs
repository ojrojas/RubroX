using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;
using RubroX.Domain.Events;

namespace RubroX.Domain.Aggregates.Flujos.Events;

public sealed record FlujoIniciadoEvent(
    FlujoId FlujoId,
    TipoFlujo TipoFlujo,
    string IniciadorId,
    RubroId? RubroId,
    MovimientoId? MovimientoId) : DomainEvent;

public sealed record PasoAprobadoEvent(
    FlujoId FlujoId,
    int PasoAprobado,
    string AprobadorId,
    int NuevoPasoActual) : DomainEvent;

public sealed record FlujoAprobadoEvent(
    FlujoId FlujoId,
    TipoFlujo TipoFlujo,
    string AprobadorFinalId,
    RubroId? RubroId,
    MovimientoId? MovimientoId) : DomainEvent;

public sealed record FlujoRechazadoEvent(
    FlujoId FlujoId,
    TipoFlujo TipoFlujo,
    string AprobadorId,
    string Motivo,
    RubroId? RubroId,
    MovimientoId? MovimientoId) : DomainEvent;

public sealed record FlujoDevoltoEvent(
    FlujoId FlujoId,
    TipoFlujo TipoFlujo,
    string AprobadorId,
    string Motivo,
    int PasoActual) : DomainEvent;
