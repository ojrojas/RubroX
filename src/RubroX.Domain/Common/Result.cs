namespace RubroX.Domain.Common;

/// <summary>
/// Resultado tipado que evita el uso de excepciones para flujo de control.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error = null)
    {
        if (isSuccess && error is not null) throw new InvalidOperationException("Success cannot have error.");
        if (!isSuccess && error is null) throw new InvalidOperationException("Failure must have error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true);
    public static Result<TValue> Failure<TValue>(string error) => new(default!, false, error);
}

/// <summary>Result tipado con valor de retorno.</summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue _value;

    internal Result(TValue value, bool isSuccess, string? error = null)
        : base(isSuccess, error) => _value = value;

    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access Value on a failed result.");

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}
