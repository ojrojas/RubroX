using RubroX.Domain.Events;

namespace RubroX.Domain.Aggregates.Rubros.Events;

public sealed record RubroCreatedEvent(
    RubroId RubroId,
    string Codigo,
    string Nombre,
    int AnoFiscal,
    string CreadoPor) : DomainEvent;

public sealed record PresupuestoAsignadoEvent(
    RubroId RubroId,
    decimal Monto,
    string UsuarioId) : DomainEvent;

public sealed record SaldoReservadoEvent(
    RubroId RubroId,
    decimal Monto,
    string MovimientoNumero) : DomainEvent;

public sealed record SaldoLiberadoEvent(
    RubroId RubroId,
    decimal Monto,
    string Motivo) : DomainEvent;

public sealed record SaldoEjecutadoEvent(
    RubroId RubroId,
    decimal Monto,
    string MovimientoNumero) : DomainEvent;

public sealed record RubroCerradoEvent(
    RubroId RubroId,
    string UsuarioId) : DomainEvent;

public sealed record RubloBloqueadoEvent(
    RubroId RubroId,
    string Motivo,
    string UsuarioId) : DomainEvent;
