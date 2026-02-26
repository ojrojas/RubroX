using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Enums;
using RubroX.Domain.Events;

namespace RubroX.Domain.Aggregates.Movimientos.Events;

public sealed record MovimientoRegistradoEvent(
    MovimientoId MovimientoId,
    RubroId RubroId,
    TipoMovimiento Tipo,
    decimal Monto,
    string Numeracion,
    string UsuarioId) : DomainEvent;

public sealed record MovimientoAnuladoEvent(
    MovimientoId MovimientoId,
    RubroId RubroId,
    decimal Monto,
    string Motivo,
    string UsuarioId) : DomainEvent;

public sealed record MovimientoVencidoEvent(
    MovimientoId MovimientoId,
    RubroId RubroId,
    decimal Monto) : DomainEvent;
