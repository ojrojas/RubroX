using RubroX.Domain.Common;

namespace RubroX.Domain.ValueObjects;

/// <summary>
/// Valor monetario presupuestal en pesos colombianos (COP).
/// Siempre ≥ 0, precisión de 2 decimales.
/// </summary>
public sealed class Saldo : ValueObject
{
    private Saldo(decimal valor) => Valor = valor;

    public decimal Valor { get; }

    public static readonly Saldo Cero = new(0m);

    public static Result<Saldo> Create(decimal valor)
    {
        if (valor < 0)
            return Result.Failure<Saldo>($"El saldo no puede ser negativo. Valor recibido: {valor:N2}");

        return Result.Success(new Saldo(Math.Round(valor, 2)));
    }

    public static Saldo Parse(decimal valor)
    {
        var result = Create(valor);
        if (result.IsFailure) throw new ArgumentException(result.Error, nameof(valor));
        return result.Value;
    }

    public static Saldo operator +(Saldo a, Saldo b) => new(a.Valor + b.Valor);

    public static Result<Saldo> operator -(Saldo a, Saldo b)
    {
        var diferencia = a.Valor - b.Valor;
        return Create(diferencia);
    }

    public bool EsSuficientePara(Saldo monto) => Valor >= monto.Valor;

    public bool EsCero => Valor == 0m;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor.ToString("C2", new System.Globalization.CultureInfo("es-CO"));
}
