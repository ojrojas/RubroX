using RubroX.Domain.Aggregates.Flujos.Events;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Common;
using RubroX.Domain.Enums;

namespace RubroX.Domain.Aggregates.Flujos;

/// <summary>Entidad — Paso de un flujo de aprobación.</summary>
public sealed class PasoAprobacion : Entity<PasoId>
{
    private PasoAprobacion() : base(PasoId.New()) { }

    internal PasoAprobacion(PasoId id, FlujoId flujoId, int orden, string rolRequerido) : base(id)
    {
        FlujoId = flujoId;
        Orden = orden;
        RolRequerido = rolRequerido;
        Estado = EstadoPaso.Pendiente;
        CreadoEn = DateTimeOffset.UtcNow;
    }

    public FlujoId FlujoId { get; private set; }
    public int Orden { get; private set; }
    public string RolRequerido { get; private set; } = string.Empty;
    public string? AprobadorId { get; private set; }
    public EstadoPaso Estado { get; private set; }
    public string? Comentario { get; private set; }
    public DateTimeOffset? FechaAccion { get; private set; }
    public DateTimeOffset CreadoEn { get; private init; }

    internal void Aprobar(string aprobadorId, string? comentario)
    {
        AprobadorId = aprobadorId;
        Estado = EstadoPaso.Aprobado;
        Comentario = comentario;
        FechaAccion = DateTimeOffset.UtcNow;
    }

    internal void Rechazar(string aprobadorId, string motivo)
    {
        AprobadorId = aprobadorId;
        Estado = EstadoPaso.Rechazado;
        Comentario = motivo;
        FechaAccion = DateTimeOffset.UtcNow;
    }

    internal void Devolver(string aprobadorId, string motivo)
    {
        AprobadorId = aprobadorId;
        Estado = EstadoPaso.Devuelto;
        Comentario = motivo;
        FechaAccion = DateTimeOffset.UtcNow;
    }
}

/// <summary>
/// Aggregate Root — Flujo de Aprobación.
/// Gestiona el workflow de aprobación multi-paso con control de roles.
/// </summary>
public sealed class FlujoAprobacion : Entity<FlujoId>
{
    private readonly List<PasoAprobacion> _pasos = [];

    private FlujoAprobacion() : base(FlujoId.New()) { }

    private FlujoAprobacion(
        FlujoId id,
        RubroId? rubroId,
        MovimientoId? movimientoId,
        TipoFlujo tipoFlujo,
        string iniciadorId,
        string payload,
        IEnumerable<(int orden, string rol)> pasos) : base(id)
    {
        RubroId = rubroId;
        MovimientoId = movimientoId;
        TipoFlujo = tipoFlujo;
        IniciadorId = iniciadorId;
        Payload = payload;
        Estado = EstadoFlujo.Pendiente;
        PasoActual = 1;
        FechaInicio = DateTimeOffset.UtcNow;
        CreadoEn = DateTimeOffset.UtcNow;

        foreach (var (orden, rol) in pasos)
            _pasos.Add(new PasoAprobacion(PasoId.New(), id, orden, rol));
    }

    public RubroId? RubroId { get; private set; }
    public MovimientoId? MovimientoId { get; private set; }
    public TipoFlujo TipoFlujo { get; private set; }
    public EstadoFlujo Estado { get; private set; }
    public int PasoActual { get; private set; }
    public string IniciadorId { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;     // JSON snapshot
    public DateTimeOffset FechaInicio { get; private init; }
    public DateTimeOffset? FechaFin { get; private set; }
    public DateTimeOffset CreadoEn { get; private init; }

    public IReadOnlyList<PasoAprobacion> Pasos => _pasos.AsReadOnly();

    public PasoAprobacion? PasoActualEntidad =>
        _pasos.FirstOrDefault(p => p.Orden == PasoActual && p.Estado == EstadoPaso.Pendiente);

    // =========================================================================
    // Factory
    // =========================================================================

    public static Result<FlujoAprobacion> Crear(
        TipoFlujo tipoFlujo,
        string iniciadorId,
        string payload,
        IEnumerable<(int orden, string rol)> pasos,
        RubroId? rubroId = null,
        MovimientoId? movimientoId = null)
    {
        var listaPasos = pasos.ToList();

        if (listaPasos.Count == 0)
            return Result.Failure<FlujoAprobacion>("Un flujo de aprobación debe tener al menos un paso.");

        if (string.IsNullOrWhiteSpace(payload))
            return Result.Failure<FlujoAprobacion>("El payload del flujo no puede estar vacío.");

        var id = FlujoId.New();
        var flujo = new FlujoAprobacion(id, rubroId, movimientoId, tipoFlujo, iniciadorId, payload, listaPasos);

        flujo.AddDomainEvent(new FlujoIniciadoEvent(id, tipoFlujo, iniciadorId, rubroId, movimientoId));
        return Result.Success(flujo);
    }

    // =========================================================================
    // Comandos de dominio
    // =========================================================================

    /// <summary>Aprueba el paso actual. Si es el último, cierra el flujo como aprobado.</summary>
    public Result Aprobar(string aprobadorId, string rolAprobador, string? comentario = null)
    {
        if (Estado != EstadoFlujo.Pendiente && Estado != EstadoFlujo.EnRevision)
            return Result.Failure($"El flujo no está en estado aprobable. Estado actual: {Estado}.");

        var paso = PasoActualEntidad;
        if (paso is null)
            return Result.Failure("No hay paso pendiente en el flujo.");

        if (paso.RolRequerido != rolAprobador)
            return Result.Failure($"El rol '{rolAprobador}' no tiene permisos para aprobar este paso. Rol requerido: '{paso.RolRequerido}'.");

        paso.Aprobar(aprobadorId, comentario);
        Estado = EstadoFlujo.EnRevision;

        var siguientePaso = _pasos.FirstOrDefault(p => p.Orden == PasoActual + 1);

        if (siguientePaso is null)
        {
            // Último paso aprobado → flujo completo
            Estado = EstadoFlujo.Aprobado;
            FechaFin = DateTimeOffset.UtcNow;
            AddDomainEvent(new FlujoAprobadoEvent(Id, TipoFlujo, aprobadorId, RubroId, MovimientoId));
        }
        else
        {
            PasoActual++;
            AddDomainEvent(new PasoAprobadoEvent(Id, paso.Orden, aprobadorId, PasoActual));
        }

        return Result.Success();
    }

    /// <summary>Rechaza el flujo. Cierra definitivamente con estado Rechazado.</summary>
    public Result Rechazar(string aprobadorId, string rolAprobador, string motivo)
    {
        if (Estado != EstadoFlujo.Pendiente && Estado != EstadoFlujo.EnRevision)
            return Result.Failure($"El flujo no está en estado rechazable. Estado actual: {Estado}.");

        if (string.IsNullOrWhiteSpace(motivo))
            return Result.Failure("El motivo de rechazo es requerido.");

        var paso = PasoActualEntidad;
        if (paso is null)
            return Result.Failure("No hay paso pendiente en el flujo.");

        if (paso.RolRequerido != rolAprobador)
            return Result.Failure($"El rol '{rolAprobador}' no tiene permisos para rechazar este paso.");

        paso.Rechazar(aprobadorId, motivo);
        Estado = EstadoFlujo.Rechazado;
        FechaFin = DateTimeOffset.UtcNow;

        AddDomainEvent(new FlujoRechazadoEvent(Id, TipoFlujo, aprobadorId, motivo, RubroId, MovimientoId));
        return Result.Success();
    }

    /// <summary>Devuelve el flujo al paso anterior para corrección.</summary>
    public Result Devolver(string aprobadorId, string rolAprobador, string motivo)
    {
        if (Estado != EstadoFlujo.Pendiente && Estado != EstadoFlujo.EnRevision)
            return Result.Failure($"El flujo no puede ser devuelto en estado '{Estado}'.");

        if (PasoActual <= 1)
            return Result.Failure("No se puede devolver el primer paso del flujo.");

        if (string.IsNullOrWhiteSpace(motivo))
            return Result.Failure("El motivo de devolución es requerido.");

        var paso = PasoActualEntidad;
        if (paso is null)
            return Result.Failure("No hay paso pendiente en el flujo.");

        if (paso.RolRequerido != rolAprobador)
            return Result.Failure($"El rol '{rolAprobador}' no tiene permisos para devolver este paso.");

        paso.Devolver(aprobadorId, motivo);

        PasoActual--;
        Estado = EstadoFlujo.Devuelto;

        // Reactivar el paso anterior
        var pasoAnterior = _pasos.First(p => p.Orden == PasoActual);
        // El paso anterior queda marcado como pendiente nuevamente mediante nueva entidad (simplificación EF)

        AddDomainEvent(new FlujoDevoltoEvent(Id, TipoFlujo, aprobadorId, motivo, PasoActual));
        return Result.Success();
    }
}
