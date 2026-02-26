namespace RubroX.Domain.Exceptions;

public sealed class DomainException(string message) : Exception(message);

public sealed class SaldoInsuficienteException(decimal disponible, decimal solicitado)
    : Exception($"Saldo insuficiente. Disponible: {disponible:N2}, Solicitado: {solicitado:N2}.");

public sealed class RubroNoEncontradoException(Guid id)
    : Exception($"No se encontró el rubro con ID '{id}'.");

public sealed class RubroInvalidoException(string motivo)
    : Exception($"Operación inválida sobre el rubro: {motivo}");

public sealed class FlujoInvalidoException(string motivo)
    : Exception($"Operación de flujo inválida: {motivo}");

public sealed class MovimientoInvalidoException(string motivo)
    : Exception($"Operación de movimiento inválida: {motivo}");

public sealed class PresupuestoVigenciaException(int anoFiscal)
    : Exception($"El rubro de la vigencia {anoFiscal} no puede ser modificado después de cerrado.");
