using RubroX.Domain.Aggregates.Rubros.Events;
using RubroX.Domain.Common;
using RubroX.Domain.Enums;
using RubroX.Domain.Exceptions;
using RubroX.Domain.ValueObjects;

namespace RubroX.Domain.Aggregates.Rubros;

/// <summary>
/// Aggregate Root — Rubro Presupuestal.
/// Controla el ciclo de vida del rubro, su saldo y su jerarquía.
/// </summary>
public sealed class Rubro : Entity<RubroId>
{
    private readonly List<RubroId> _hijosIds = [];

    // EF Core constructor
    private Rubro() : base(RubroId.New()) { }

    private Rubro(
        RubroId id,
        CodigoPresupuestal codigo,
        string nombre,
        string descripcion,
        AnoFiscal anoFiscal,
        TipoRubro tipo,
        FuenteFinanciamiento fuente,
        RubroId? padreId) : base(id)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        AnoFiscal = anoFiscal;
        Tipo = tipo;
        Fuente = fuente;
        PadreId = padreId;
        SaldoInicial = Saldo.Cero;
        SaldoComprometido = Saldo.Cero;
        SaldoEjecutado = Saldo.Cero;
        Estado = EstadoRubro.Activo;
        CreadoEn = DateTimeOffset.UtcNow;
        ActualizadoEn = DateTimeOffset.UtcNow;
    }

    public CodigoPresupuestal Codigo { get; private set; } = null!;
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public AnoFiscal AnoFiscal { get; private set; } = null!;
    public TipoRubro Tipo { get; private set; }
    public FuenteFinanciamiento Fuente { get; private set; } = null!;
    public Saldo SaldoInicial { get; private set; } = null!;
    public Saldo SaldoComprometido { get; private set; } = null!;
    public Saldo SaldoEjecutado { get; private set; } = null!;
    public EstadoRubro Estado { get; private set; }
    public RubroId? PadreId { get; private set; }
    public DateTimeOffset CreadoEn { get; private init; }
    public DateTimeOffset ActualizadoEn { get; private set; }
    public string? CreadoPor { get; private set; }

    public IReadOnlyList<RubroId> HijosIds => _hijosIds.AsReadOnly();

    /// <summary>Saldo disponible = Inicial - Comprometido - Ejecutado</summary>
    public decimal SaldoDisponible => SaldoInicial.Valor - SaldoComprometido.Valor - SaldoEjecutado.Valor;

    /// <summary>Porcentaje de ejecución = (Ejecutado / Inicial) * 100</summary>
    public decimal PorcentajeEjecucion => SaldoInicial.Valor > 0
        ? Math.Round(SaldoEjecutado.Valor / SaldoInicial.Valor * 100, 2)
        : 0m;

    // =========================================================================
    // Factory
    // =========================================================================

    public static Result<Rubro> Crear(
        CodigoPresupuestal codigo,
        string nombre,
        string descripcion,
        AnoFiscal anoFiscal,
        TipoRubro tipo,
        FuenteFinanciamiento fuente,
        RubroId? padreId,
        string creadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure<Rubro>("El nombre del rubro no puede estar vacío.");

        if (nombre.Length > 200)
            return Result.Failure<Rubro>("El nombre del rubro no puede superar 200 caracteres.");

        var id = RubroId.New();
        var rubro = new Rubro(id, codigo, nombre.Trim(), descripcion?.Trim() ?? string.Empty,
            anoFiscal, tipo, fuente, padreId)
        {
            CreadoPor = creadoPor
        };

        rubro.AddDomainEvent(new RubroCreatedEvent(id, codigo.Valor, nombre, anoFiscal.Valor, creadoPor));

        return Result.Success(rubro);
    }

    // =========================================================================
    // Comandos de dominio
    // =========================================================================

    /// <summary>Asigna (o reemplaza) la apropiación presupuestal inicial del rubro.</summary>
    public Result AsignarPresupuesto(Saldo monto, string usuarioId)
    {
        if (Estado != EstadoRubro.Activo)
            return Result.Failure($"No se puede asignar presupuesto a un rubro en estado '{Estado}'.");

        SaldoInicial = monto;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new PresupuestoAsignadoEvent(Id, monto.Valor, usuarioId));
        return Result.Success();
    }

    /// <summary>Reserva saldo comprometido (CDP). Valida disponibilidad.</summary>
    public Result ReservarSaldo(Saldo monto, string movimientoNumero)
    {
        if (Estado != EstadoRubro.Activo)
            return Result.Failure($"El rubro '{Codigo}' no está activo.");

        if (!Saldo.Parse(SaldoDisponible).EsSuficientePara(monto))
            return Result.Failure(
                $"Saldo insuficiente en rubro '{Codigo}'. Disponible: {SaldoDisponible:N2}, Solicitado: {monto.Valor:N2}.");

        SaldoComprometido = SaldoComprometido + monto;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new SaldoReservadoEvent(Id, monto.Valor, movimientoNumero));
        return Result.Success();
    }

    /// <summary>Libera saldo comprometido (anulación de CDP).</summary>
    public Result LiberarSaldo(Saldo monto, string motivo)
    {
        var nuevoComprometido = SaldoComprometido - monto;
        if (nuevoComprometido.IsFailure)
            return Result.Failure($"No se puede liberar más saldo del que está comprometido.");

        SaldoComprometido = nuevoComprometido.Value;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new SaldoLiberadoEvent(Id, monto.Valor, motivo));
        return Result.Success();
    }

    /// <summary>Registra ejecución presupuestal (CRP → Pago). Mueve de comprometido a ejecutado.</summary>
    public Result EjecutarSaldo(Saldo monto, string movimientoNumero)
    {
        if (Estado != EstadoRubro.Activo)
            return Result.Failure($"El rubro '{Codigo}' no está activo.");

        var nuevoComprometido = SaldoComprometido - monto;
        if (nuevoComprometido.IsFailure)
            return Result.Failure("El monto a ejecutar supera el saldo comprometido disponible.");

        SaldoComprometido = nuevoComprometido.Value;
        SaldoEjecutado = SaldoEjecutado + monto;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new SaldoEjecutadoEvent(Id, monto.Valor, movimientoNumero));
        return Result.Success();
    }

    /// <summary>Cierra el rubro al final de la vigencia.</summary>
    public Result Cerrar(string usuarioId)
    {
        if (Estado == EstadoRubro.Cerrado)
            return Result.Failure("El rubro ya está cerrado.");

        if (SaldoComprometido.Valor > 0)
            return Result.Failure($"No se puede cerrar el rubro con saldo comprometido pendiente: {SaldoComprometido}.");

        Estado = EstadoRubro.Cerrado;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new RubroCerradoEvent(Id, usuarioId));
        return Result.Success();
    }

    /// <summary>Bloquea el rubro por decisión administrativa.</summary>
    public Result Bloquear(string motivo, string usuarioId)
    {
        if (Estado is EstadoRubro.Cerrado or EstadoRubro.Bloqueado)
            return Result.Failure($"El rubro no puede ser bloqueado en estado '{Estado}'.");

        Estado = EstadoRubro.Bloqueado;
        ActualizadoEn = DateTimeOffset.UtcNow;

        AddDomainEvent(new RubloBloqueadoEvent(Id, motivo, usuarioId));
        return Result.Success();
    }

    /// <summary>Registra un rubro hijo (llamado desde el hijo al establecer padre).</summary>
    internal void AgregarHijo(RubroId hijoId)
    {
        if (!_hijosIds.Contains(hijoId))
            _hijosIds.Add(hijoId);
    }
}
