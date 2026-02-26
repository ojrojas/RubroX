using RubroX.Domain.Common;

namespace RubroX.Domain.ValueObjects;

/// <summary>
/// Año fiscal presupuestal. Válido en rango 2000–2099.
/// </summary>
public sealed class AnoFiscal : ValueObject
{
    private const int AnoMinimo = 2000;
    private const int AnoMaximo = 2099;

    private AnoFiscal(int valor) => Valor = valor;

    public int Valor { get; }

    public static Result<AnoFiscal> Create(int valor)
    {
        if (valor < AnoMinimo || valor > AnoMaximo)
            return Result.Failure<AnoFiscal>(
                $"El año fiscal debe estar entre {AnoMinimo} y {AnoMaximo}. Valor recibido: {valor}.");

        return Result.Success(new AnoFiscal(valor));
    }

    public static AnoFiscal Actual() => new(DateTime.UtcNow.Year);

    public static AnoFiscal Parse(int valor)
    {
        var result = Create(valor);
        if (result.IsFailure) throw new ArgumentException(result.Error, nameof(valor));
        return result.Value;
    }

    public bool EsVigenciaActual() => Valor == DateTime.UtcNow.Year;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor.ToString();
}
