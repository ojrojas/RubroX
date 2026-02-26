using RubroX.Domain.Aggregates.Movimientos.Events;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Common;
using RubroX.Domain.Enums;
using RubroX.Domain.ValueObjects;

namespace RubroX.Domain.Aggregates.Movimientos;

/// <summary>
/// Aggregate Root — Movimiento Presupuestal.
/// Representa CDP, CRP, Pago, Reducción, etc.
/// </summary>
public sealed class MovimientoPresupuestal : Entity<MovimientoId>
{
    private MovimientoPresupuestal() : base(MovimientoId.New()) { }

    private MovimientoPresupuestal(
        MovimientoId id,
        RubroId rubroId,
        TipoMovimiento tipo,
        Saldo monto,
        string concepto,
        string numeracion,
        string usuarioId,
        MovimientoId? padreId,
        DateTimeOffset? fechaVencimiento) : base(id)
    {
        RubroId = rubroId;
        Tipo = tipo;
        Monto = monto;
        Concepto = concepto;
        Numeracion = numeracion;
        UsuarioId = usuarioId;
        PadreId = padreId;
        FechaVencimiento = fechaVencimiento;
        Estado = EstadoMovimiento.Activo;
        FechaRegistro = DateTimeOffset.UtcNow;
        CreadoEn = DateTimeOffset.UtcNow;
    }

    public RubroId RubroId { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public Saldo Monto { get; private set; } = null!;
    public string Concepto { get; private set; } = string.Empty;
    public string Numeracion { get; private set; } = string.Empty;
    public string UsuarioId { get; private set; } = string.Empty;
    public MovimientoId? PadreId { get; private set; }   // CRP apunta al CDP que lo originó
    public DateTimeOffset FechaRegistro { get; private init; }
    public DateTimeOffset? FechaVencimiento { get; private set; }
    public EstadoMovimiento Estado { get; private set; }
    public string? Observacion { get; private set; }
    public DateTimeOffset CreadoEn { get; private init; }
    public DateTimeOffset ActualizadoEn { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Result<MovimientoPresupuestal> Crear(
        RubroId rubroId,
        TipoMovimiento tipo,
        Saldo monto,
        string concepto,
        string numeracion,
        string usuarioId,
        MovimientoId? padreId = null,
        DateTimeOffset? fechaVencimiento = null)
    {
        if (string.IsNullOrWhiteSpace(concepto))
            return Result.Failure<MovimientoPresupuestal>("El concepto del movimiento es requerido.");

        if (string.IsNullOrWhiteSpace(numeracion))
            return Result.Failure<MovimientoPresupuestal>("La numeración del movimiento es requerida.");

        if (monto.EsCero)
            return Result.Failure<MovimientoPresupuestal>("El monto del movimiento debe ser mayor a cero.");

        var id = MovimientoId.New();
        var movimiento = new MovimientoPresupuestal(
            id, rubroId, tipo, monto, concepto.Trim(), numeracion.Trim(),
            usuarioId, padreId, fechaVencimiento);

        movimiento.AddDomainEvent(new MovimientoRegistradoEvent(id, rubroId, tipo, monto.Valor, numeracion, usuarioId));

        return Result.Success(movimiento);
    }

    // =========================================================================
    // Comandos de dominio
    // =========================================================================

    /// <summary>Anula el movimiento. Solo posible si está Activo.</summary>
    public Result Anular(string motivo, string usuarioId)
    {
        if (Estado != EstadoMovimiento.Activo)
            return Result.Failure($"El movimiento '{Numeracion}' ya está en estado '{Estado}'. No se puede anular.");

        if (string.IsNullOrWhiteSpace(motivo))
            return Result.Failure("El motivo de anulación es requerido.");

        Estado = EstadoMovimiento.Anulado;
        Observacion = motivo.Trim();
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new MovimientoAnuladoEvent(Id, RubroId, Monto.Valor, motivo, usuarioId));
        return Result.Success();
    }

    /// <summary>Marca el movimiento como vencido (proceso batch).</summary>
    public Result MarcarVencido()
    {
        if (Estado != EstadoMovimiento.Activo)
            return Result.Failure($"Solo se pueden vencer movimientos activos.");

        Estado = EstadoMovimiento.Vencido;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new MovimientoVencidoEvent(Id, RubroId, Monto.Valor));
        return Result.Success();
    }
}
