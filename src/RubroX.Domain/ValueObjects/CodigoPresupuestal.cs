using RubroX.Domain.Common;
using System.Text.RegularExpressions;

namespace RubroX.Domain.ValueObjects;

/// <summary>
/// Código presupuestal jerárquico con formato XX.XX.XXXX.XX
/// Ejemplo: 01.01.1000.01 — Funcionamiento > Servicios Personales > Planta Personal > Básico
/// </summary>
public sealed class CodigoPresupuestal : ValueObject
{
    private static readonly Regex _pattern = new(@"^\d{2}(\.\d{2,4}){0,3}$", RegexOptions.Compiled);

    private CodigoPresupuestal(string valor) => Valor = valor;

    public string Valor { get; }

    /// <summary>Nivel jerárquico (1-4) según segmentos del código.</summary>
    public int Nivel => Valor.Split('.').Length;

    /// <summary>Código del rubro padre (null si es raíz).</summary>
    public CodigoPresupuestal? Padre => Nivel > 1
        ? new(string.Join('.', Valor.Split('.')[..^1]))
        : null;

    public static Result<CodigoPresupuestal> Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return Result.Failure<CodigoPresupuestal>("El código presupuestal no puede estar vacío.");

        var normalizado = valor.Trim();

        if (!_pattern.IsMatch(normalizado))
            return Result.Failure<CodigoPresupuestal>(
                $"El código presupuestal '{normalizado}' no tiene el formato válido (ej: 01.01.1000.01).");

        return Result.Success(new CodigoPresupuestal(normalizado));
    }

    public static CodigoPresupuestal Parse(string valor)
    {
        var result = Create(valor);
        if (result.IsFailure) throw new ArgumentException(result.Error, nameof(valor));
        return result.Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;
}
