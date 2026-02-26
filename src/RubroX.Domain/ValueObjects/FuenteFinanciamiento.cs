using RubroX.Domain.Common;

namespace RubroX.Domain.ValueObjects;

/// <summary>
/// Fuente de financiamiento del rubro presupuestal.
/// </summary>
public sealed class FuenteFinanciamiento : ValueObject
{
    public static readonly FuenteFinanciamiento Nacion = new("Nacion");
    public static readonly FuenteFinanciamiento PropiosRecursos = new("PropiosRecursos");
    public static readonly FuenteFinanciamiento SGP = new("SGP");          // Sistema General de Participaciones
    public static readonly FuenteFinanciamiento SGR = new("SGR");          // Sistema General de Regalías
    public static readonly FuenteFinanciamiento Credito = new("Credito");
    public static readonly FuenteFinanciamiento Cofinanciacion = new("Cofinanciacion");

    private static readonly HashSet<string> _valoresValidos =
    [
        "Nacion", "PropiosRecursos", "SGP", "SGR", "Credito", "Cofinanciacion"
    ];

    private FuenteFinanciamiento(string valor) => Valor = valor;

    public string Valor { get; }

    public static Result<FuenteFinanciamiento> Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return Result.Failure<FuenteFinanciamiento>("La fuente de financiamiento no puede estar vacía.");

        if (!_valoresValidos.Contains(valor))
            return Result.Failure<FuenteFinanciamiento>(
                $"'{valor}' no es una fuente de financiamiento válida. Valores permitidos: {string.Join(", ", _valoresValidos)}.");

        return Result.Success(new FuenteFinanciamiento(valor));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;
}
